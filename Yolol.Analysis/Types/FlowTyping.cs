using System;
using System.Collections.Generic;
using System.Linq;
using Yolol.Analysis.ControlFlowGraph;
using Yolol.Analysis.ControlFlowGraph.AST;
using Yolol.Analysis.ControlFlowGraph.Extensions;
using Yolol.Analysis.TreeVisitor;
using Yolol.Analysis.TreeVisitor.Inspection;
using Yolol.Grammar;
using Yolol.Grammar.AST.Statements;

namespace Yolol.Analysis.Types
{
    public static class FlowTypingExtensions
    {
        private static IControlFlowGraph RemoveTypeAssignments(this IControlFlowGraph graph)
        {
            return graph.Modify((a, b) => {
                foreach (var stmt in a.Statements)
                {
                    if (stmt is TypedAssignment tass)
                        b.Add(new Assignment(tass.Left, tass.Right));
                    else
                        b.Add(stmt);
                }
            });
        }

        public static IControlFlowGraph FlowTypingAssignment(
            this IControlFlowGraph graph,
            ISingleStaticAssignmentTable ssa, // We require the SSA object because SSA must be done before flow typing
            out ITypeAssignments types,
            params (VariableName, Execution.Type)[] hints)
        {
            var typesMut = new TypeAssignmentTable();
            types = typesMut;

            // Remove any existing types
            graph = graph.RemoveTypeAssignments();

            // Unconditionally assign hints
            foreach (var (name, type) in hints)
                typesMut.Assign(name, type);

            // Assign all variables which are never assigned the default type (number)
            foreach (var unassReads in UnassignedReadNames(graph))
                typesMut.Assign(unassReads, Execution.Type.Number);

            // Keep assigning types until we no longer find any additional types
            var output = graph;
            int modified;
            do
            {
                var assigner = new AssignTypes(typesMut);
                output = output.Modify((a, b) => {
                    foreach (var statement in a.Statements)
                        b.Add(assigner.Visit(statement));
                });
                modified = assigner.Modified;
            } while (modified > 0);

            return output;
        }

        private static IEnumerable<VariableName> UnassignedReadNames(IControlFlowGraph graph)
        {
            var assignedVars = new FindAssignedVariables();
            var readVars = new FindReadVariables();
            foreach (var vertex in graph.Vertices)
            foreach (var statement in vertex.Statements)
            {
                assignedVars.Visit(statement);
                readVars.Visit(statement);
            }

            return readVars.Names.Where(a => !a.IsExternal).Except(assignedVars.Names);
        }

        private class AssignTypes
            : BaseStatementVisitor<BaseStatement>
        {
            private readonly TypeAssignmentTable _types;

            public int Modified { get; private set; }

            public AssignTypes(TypeAssignmentTable types)
            {
                _types = types;
            }

            protected override BaseStatement Visit(ErrorStatement err)
            {
                return err;
            }

            protected override BaseStatement Visit(Conditional con)
            {
                return con;
            }

            protected override BaseStatement Visit(EmptyStatement empty)
            {
                return empty;
            }

            protected override BaseStatement Visit(StatementList list)
            {
                return new StatementList(list.Statements.Select(Visit));
            }

            protected override BaseStatement Visit(CompoundAssignment compAss)
            {
                throw new NotSupportedException("Cannot flow type CFG with compound expression (decompose to simpler form first)");
            }

            protected override BaseStatement Visit(TypedAssignment ass)
            {
                return ass;
            }

            protected override BaseStatement Visit(Assignment ass)
            {
                // Check type of the right hand side
                var type = new ExpressionTypeInference(_types).Visit(ass.Right);

                // If we've already assigned this type, just keep that as is
                if (ass is TypedAssignment tass && tass.Type == type)
                    return ass;

                // If type is unassigned then we can't type this yet
                if (type == Execution.Type.Unassigned)
                    return ass;

                // We found some new type information
                Modified++;
                _types.Assign(ass.Left, type);
                return new TypedAssignment(type, ass.Left, ass.Right);
            }

            protected override BaseStatement Visit(ExpressionWrapper expr)
            {
                return expr;
            }

            protected override BaseStatement Visit(Goto @goto)
            {
                return @goto;
            }

            protected override BaseStatement Visit(If @if)
            {
                var l = (StatementList)Visit(@if.TrueBranch);
                var r = (StatementList)Visit(@if.FalseBranch);
                return new If(@if.Condition, l, r);
            }
        }

        private class TypeAssignmentTable
            : ITypeAssignments
        {
            private readonly Dictionary<VariableName, Execution.Type> _types = new Dictionary<VariableName, Execution.Type>();

            public void Assign(VariableName name, Execution.Type type)
            {
                if (_types.TryGetValue(name, out var t))
                    _types[name] = t | type;
                else
                    _types[name] = type;
            }

            
            public Execution.Type? TypeOf(VariableName varName)
            {
                if (_types.TryGetValue(varName, out var type))
                    return type;
                return null;
            }
        }
    }
}
