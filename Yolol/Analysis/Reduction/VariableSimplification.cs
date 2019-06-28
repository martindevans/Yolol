using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using Yolol.Grammar;
using Yolol.Grammar.AST.Statements;

namespace Yolol.Analysis.Reduction
{
    public class VariableSimplificationVisitor
        : BaseTreeVisitor
    {
        private readonly INameGenerator _names;
        private readonly Dictionary<string, string> _remap = new Dictionary<string, string>();
        
        public VariableSimplificationVisitor([CanBeNull] INameGenerator names = null)
        {
            _names = names ?? new SequentialNameGenerator();
        }

        public override Program Visit(Program program)
        {
            // Find all variables with a count of how frequent they are
            var p = new FirstPass();
            p.Visit(program);

            // Generate some unique names, order by size
            var names = Enumerable.Range(0, p.Names.Count()).Select(a => _names.Name()).OrderBy(a => a.Length).ToArray();
            var vars = p.Names.OrderByDescending(a => a.Value).ThenBy(a => a.Key.Name).Select(a => a.Key).ToArray();

            // Assign most common variables to shortest names
            foreach (var (n1, n2) in vars.Zip(names, (a, b) => (a, b)))
                _remap.Add(n1.Name, n2);

            return base.Visit(program);
        }

        protected override VariableName Visit(VariableName var)
        {
            if (var.IsExternal)
                return base.Visit(var);
            else
                return base.Visit(new VariableName(_remap[var.Name]));
        }

        private class FirstPass
            : BaseTreeVisitor
        {
            private readonly ConcurrentDictionary<VariableName, uint> _nameCount = new ConcurrentDictionary<VariableName, uint>();

            public IEnumerable<KeyValuePair<VariableName, uint>> Names => _nameCount;

            protected override VariableName Visit(VariableName var)
            {
                if (!var.IsExternal)
                    _nameCount.AddOrUpdate(var, 1, (_, a) => a + 1);

                return base.Visit(var);
            }
        }
    }
}
