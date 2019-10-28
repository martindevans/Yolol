using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using Yolol.Analysis.ControlFlowGraph;
using Yolol.Analysis.ControlFlowGraph.Extensions;
using Yolol.Analysis.TreeVisitor.Reduction;
using Yolol.Grammar;
using Yolol.Grammar.AST;
using Yolol.Grammar.AST.Expressions;
using Yolol.Grammar.AST.Statements;

namespace YololEmulator.Tests.Analysis.Inspection
{
    [TestClass]
    public class FindBooleans
    {
        private void _test(Program ast, IEnumerable<VariableName> originalBooleanVariableNames)
        {
            var cfg = new Builder(ast.StripTypes()).Build();

            Console.WriteLine(cfg.ToYolol().ToString());

            cfg = cfg.StaticSingleAssignment(out var ssa);

            var assignments = cfg.FindVariableAssignments(ssa);

            var ssaBooleanVariableNames = new HashSet<VariableName>(from variableTreeNode in from originalBooleanVariableName in originalBooleanVariableNames
                                                                    from ssaBooleanVariableName in ssa.AssignedNames(originalBooleanVariableName)
                                                                    select new VariableTreeNode(ssaBooleanVariableName, assignments, ssa)
                                                                    from variableName in variableTreeNode.GetFullTree()
                                                                    select variableName);

            foreach (var variableName in ssaBooleanVariableNames)
                Console.WriteLine(variableName);

            Console.WriteLine("---");

            foreach (var variableName in cfg.FindBooleanVariables(ssa))
                Console.WriteLine(variableName);

            Assert.IsTrue(ssaBooleanVariableNames.SetEquals(cfg.FindBooleanVariables(ssa)));
        }

        private void _test(string yolol, IEnumerable<VariableName> originalBooleanVariableNames) => _test(TestExecutor.Parse(yolol), originalBooleanVariableNames);

        private void _test(string yolol, IEnumerable<string> originalBooleanVariableNames) => _test(yolol, originalBooleanVariableNames.Select(originalBooleanVariableName => new VariableName(originalBooleanVariableName)));

        [TestMethod]
        public void FindBooleansSingleLine()
        {
            var yolol = "hi=30<2 theAnswer=42 b=theAnswer==42";

            var originalBooleanVariableNames = new string[]
            {
                "hi",
                "b"
            };

            _test(yolol, originalBooleanVariableNames);
        }

        [TestMethod]
        public void FindBooleanLiterals()
        {
            var yolol = "a=1 theAnswer=42 b=theAnswer==a";

            var originalBooleanVariableNames = new string[]
            {
                "a",
                "b"
            };

            _test(yolol, originalBooleanVariableNames);
        }

        [TestMethod]
        public void FindBooleansMultiLine()
        {
            var yolol = "a=1\ntheAnswer=42\nb=theAnswer==a";

            var originalBooleanVariableNames = new string[]
            {
                "a",
                "b"
            };

            _test(yolol, originalBooleanVariableNames);
        }
    }

    class VariableTreeNode
    {
        public bool Top => _parent == null;
        private IEnumerable<Assignment> _assignments;
        private VariableTreeNode _parent;
        private readonly VariableName _variableName;
        private readonly HashSet<VariableTreeNode> _children = new HashSet<VariableTreeNode>();

        public HashSet<VariableName> GetFullTree()
        {
            var fullTree = new HashSet<VariableName>(new VariableName[] { _variableName });

            var bottomLayer = new HashSet<VariableName>(new VariableName[] { _variableName });

            while (true)
            {
                var newDescendants = (from assignment in _assignments
                                                               where assignment.Right is Variable variable && bottomLayer.Contains(variable.Name)
                                                               select assignment.Left).Except(bottomLayer).ToHashSet();

                if (newDescendants.Count() == 0)
                    return fullTree;
                else
                {
                    fullTree.UnionWith(newDescendants);

                    bottomLayer = newDescendants;
                }
            }
        }

        //private HashSet<VariableName> _descendants
        //{
        //    get => (from child in _children select child._variableName)
        //            .Union(from child in _children
        //                    from descendant in child._descendants
        //                    select descendant).ToHashSet();
        //}

        public VariableTreeNode(VariableName variableName, IEnumerable<Assignment> assignments, ISingleStaticAssignmentTable ssa)
        {
            _assignments = assignments;
            _variableName = variableName;

            while (true)
            {
                var topAssignments = assignments.Where(ass => ass.Left == _variableName && ass.Right is Variable);

                if (topAssignments.Count() > 0)
                    _variableName = (topAssignments.First().Right as Variable).Name;
                else
                    break;

                //foreach (var assignment in assignments)
                //    if (_parent == null && assignment.Left == _variableName && assignment.Right is Variable parentVariable)
                //        _parent = new VariableTreeNode(parentVariable.Name, assignments, ssa);
                //else if (assignment.Right is Variable childVariable)
                //    _children.Add(new VariableTreeNode(childVariable.Name, assignments, ssa, this));
            }
        }
    }
}
