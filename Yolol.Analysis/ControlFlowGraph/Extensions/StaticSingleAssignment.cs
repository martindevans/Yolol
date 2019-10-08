using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using Yolol.Analysis.ControlFlowGraph.AST;
using Yolol.Analysis.TreeVisitor;
using Yolol.Grammar;
using Yolol.Grammar.AST.Expressions;
using Yolol.Grammar.AST.Expressions.Unary;
using Yolol.Grammar.AST.Statements;

namespace Yolol.Analysis.ControlFlowGraph.Extensions
{
    public static class StaticSingleAssignmentExtensions
    {
        [NotNull] public static IControlFlowGraph StaticSingleAssignment([NotNull] this IControlFlowGraph graph, [NotNull] out ISingleStaticAssignmentTable ssa)
        {
            var ssaMut = new SingleStaticAssignmentTable();
            ssa = ssaMut;

            var finalNamesInBlock = new Dictionary<IBasicBlock, IReadOnlyDictionary<VariableName, VariableName>>();

            // Replace every variable assignment with a new name, keep track of the last name in each block
            var output = graph.Modify((a, b) => {

                var finalNames = new Dictionary<VariableName, VariableName>();
                finalNamesInBlock[b] = finalNames;

                foreach (var stmt in a.Statements)
                {
                    if (stmt is Assignment ass && !ass.Left.IsExternal)
                    {
                        var sname = ssaMut.Assign(ass.Left);
                        b.Add(new Assignment(sname, ass.Right));

                        // Store the final name assigned to this var in this block
                        finalNames[ass.Left] = sname;
                    }
                    else
                        b.Add(stmt);
                }
            });

            // Now track back, changing the variable _reads_ based on the last value in predecessors, If there
            // are two names available for a variable in the predecessors use the Phi function to "read both".
            output = output.Modify((a, b) => {
                var stmts = new ApplySsaPass(a, ssaMut, finalNamesInBlock).Visit(a.Statements);
                foreach (var stmt in stmts)
                    b.Add(stmt);
            });

            return output;
        }

        [NotNull]
        public static IControlFlowGraph RemoveStaticSingleAssignment([NotNull] this IControlFlowGraph graph, ISingleStaticAssignmentTable ssa)
        {
            return graph.VisitBlocks(() => new RemoveSsaPass(ssa));
        }

        private class RemoveSsaPass
            : BaseTreeVisitor
        {
            private readonly ISingleStaticAssignmentTable _ssa;

            public RemoveSsaPass(ISingleStaticAssignmentTable ssa)
            {
                _ssa = ssa;
            }

            protected override BaseExpression Visit(Phi phi)
            {
                return new Variable(phi.BaseVariable);
            }

            protected override VariableName Visit(VariableName var)
            {
                if (var.IsExternal)
                    return var;

                return _ssa.BaseName(var);
            }
        }

        private class ApplySsaPass
            : BaseTreeVisitor
        {
            private readonly IBasicBlock _block;
            private readonly SingleStaticAssignmentTable _ssa;
            private readonly Dictionary<IBasicBlock, IReadOnlyDictionary<VariableName, VariableName>> _finalNamesByBlock;

            private readonly Dictionary<VariableName, VariableName> _previouslyAssignedNamesInBlock = new Dictionary<VariableName, VariableName>();

            public ApplySsaPass(IBasicBlock block, SingleStaticAssignmentTable ssa, Dictionary<IBasicBlock, IReadOnlyDictionary<VariableName, VariableName>> finalNamesByBlock)
            {
                _block = block;
                _ssa = ssa;
                _finalNamesByBlock = finalNamesByBlock;
            }

            protected override BaseStatement Visit(Assignment ass)
            {
                var r = base.Visit(ass);

                if (!ass.Left.IsExternal)
                {
                    var bn = _ssa.BaseName(ass.Left);
                    _previouslyAssignedNamesInBlock[bn] = ass.Left;
                }

                return r;
            }

            protected override BaseExpression Visit(Variable var)
            {
                if (var.Name.IsExternal)
                    return var;

                if (_previouslyAssignedNamesInBlock.TryGetValue(var.Name, out var assigned))
                    return new Variable(assigned);

                var names = new List<VariableName>();
                var searched = new HashSet<IBasicBlock>();
                var todo = new Queue<IBasicBlock>();
                foreach (var edge in _block.Incoming)
                    todo.Enqueue(edge.Start);

                while (todo.Count > 0)
                {
                    var node = todo.Dequeue();
                    if (!searched.Add(node))
                        continue;

                    if (_finalNamesByBlock.TryGetValue(node, out var finalInBlock) && finalInBlock.ContainsKey(var.Name))
                    {
                        names.Add(finalInBlock[var.Name]);
                    }
                    else if (node.Type == BasicBlockType.Entry)
                    {
                        // We've reached the root so this is trying to read an undefined value
                        names.Add(_ssa.Assign(var.Name));
                    }
                    else
                    {
                        // Continue searching further up
                        foreach (var edge in node.Incoming)
                            todo.Enqueue(edge.Start);
                    }
                }

                // If no names were found this is accessing a previously undefined variable. Assign it a unique name now.
                if (names.Count == 0)
                    names.Add(_ssa.Assign(var.Name));

                if (names.Count == 1)
                    return new Variable(names.Single());
                else
                    return new Phi(_ssa, names.ToArray());
            }

            [NotNull] public IEnumerable<BaseStatement> Visit([NotNull] IEnumerable<BaseStatement> statements)
            {
                return base.Visit(new StatementList(statements)).Statements;
            }
        }

        private class SingleStaticAssignmentTable
            : ISingleStaticAssignmentTable
        {
            private readonly Dictionary<VariableName, List<VariableName>> _baseToAssigned = new Dictionary<VariableName, List<VariableName>>();
            private readonly Dictionary<VariableName, VariableName> _assignedToBase = new Dictionary<VariableName, VariableName>();

            [NotNull] public VariableName Assign([NotNull] VariableName baseName)
            {
                // Get ot create list of assigned names
                if (!_baseToAssigned.TryGetValue(baseName, out var l))
                {
                    l = new List<VariableName>();
                    _baseToAssigned.Add(baseName, l);
                }

                // Create unique name
                var assignedName = new VariableName($"{baseName}[{l.Count}]");

                // Save it
                l.Add(assignedName);
                _assignedToBase.Add(assignedName, baseName);

                return assignedName;
            }

            [NotNull] public IEnumerable<VariableName> AssignedNames([NotNull] VariableName baseVariable)
            {
                return _baseToAssigned[baseVariable];
            }

            [NotNull] public VariableName BaseName([NotNull] VariableName ssaName)
            {
                return _assignedToBase[ssaName];
            }

            public IEnumerable<VariableName> BaseVariables => _baseToAssigned.Keys;
        }
    }

    public interface ISingleStaticAssignmentTable
    {
        /// <summary>
        /// Get the original names of variables that have been reassigned
        /// </summary>
        IEnumerable<VariableName> BaseVariables { get; }

        /// <summary>
        /// Get the list of SSA names fora given variable
        /// </summary>
        /// <param name="baseVariable"></param>
        /// <returns></returns>
        IEnumerable<VariableName> AssignedNames(VariableName baseVariable);

        /// <summary>
        /// Get the original base name associated with an SSA name
        /// </summary>
        /// <param name="ssaName"></param>
        /// <returns></returns>
        VariableName BaseName(VariableName ssaName);
    }
}
