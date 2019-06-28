using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using Yolol.Execution;
using Yolol.Grammar;
using Yolol.Grammar.AST.Expressions;
using Yolol.Grammar.AST.Expressions.Unary;
using Yolol.Grammar.AST.Statements;

namespace Yolol.Analysis.Reduction
{
    public class RepeatConstantHoisting
        : BaseTreeVisitor
    {
        private readonly Dictionary<Value, string> _replacements = new Dictionary<Value, string>();
        private BaseStatement[] _constantInitializers;

        public override Program Visit(Program program)
        {
            var first = new FirstPass();

            // Find constants which are repeated at least once
            var finder = new FirstPass();
            finder.Visit(program);

            // Extract constats which are worth hoisting
            foreach (var (val, count) in finder.Constants)
            {
                if (count <= 1)
                    continue;

                var constLength = val.ToString().Length;
                if (count * constLength <= (3 + constLength + count))
                    continue;

                _replacements.Add(val, $"_{Guid.NewGuid().ToString().Replace("-", "_")}");
            }

            // Early exit if there are no replacements to do
            if (_replacements.Count == 0)
                return program;

            // Build set of assignments to insert
            _constantInitializers = _replacements.Select(a => new Assignment(new VariableName(a.Value), a.Key.Type == Execution.Type.Number ? (BaseExpression)new ConstantNumber(a.Key.Number) : new ConstantString(a.Key.String))).ToArray();

            return base.Visit(program);
        }

        protected override Line Visit(Line line)
        {
            if (_constantInitializers == null)
                return base.Visit(line);

            // visit line to sub constants
            var visited = base.Visit(line);

            var c = _constantInitializers;
            _constantInitializers = null;
            return new Line(new StatementList(c.Concat(visited.Statements.Statements)));
        }

        protected override BaseExpression Visit(ConstantNumber con)
        {
            if (_replacements.TryGetValue(new Value(con.Value), out var name))
                return new Grammar.AST.Expressions.Unary.Variable(new VariableName(name));
            else
                return con;
        }

        protected override BaseExpression Visit(ConstantString con)
        {
            if (_replacements.TryGetValue(new Value(con.Value), out var name))
                return new Grammar.AST.Expressions.Unary.Variable(new VariableName(name));
            else
                return con;
        }

        private class FirstPass
            : BaseTreeVisitor
        {
            private readonly ConcurrentDictionary<Value, uint> _constantCount = new ConcurrentDictionary<Value, uint>();

            protected override BaseExpression Visit(ConstantNumber con)
            {
                _constantCount.AddOrUpdate(new Value(con.Value), 1, (_, a) => a + 1);

                return base.Visit(con);
            }

            protected override BaseExpression Visit(ConstantString con)
            {
                _constantCount.AddOrUpdate(new Value(con.Value), 1, (_, a) => a + 1);

                return base.Visit(con);
            }

            [NotNull] public ConcurrentDictionary<Value, uint> Constants => _constantCount;
        }
    }
}
