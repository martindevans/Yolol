using System;
using System.Collections.Generic;
using System.Text;
using JetBrains.Annotations;
using Yolol.Analysis.TreeVisitor;
using Yolol.Grammar;
using Yolol.Grammar.AST;
using Yolol.Grammar.AST.Statements;

namespace Yolol.Analysis
{
    public class SafeNameGenerator
        : INameGenerator
    {
        private readonly INameGenerator _inner;
        private readonly FindVarNames _taken;

        public SafeNameGenerator([NotNull] INameGenerator inner, [NotNull] Program program)
        {
            _inner = inner;
            _taken = new FindVarNames();

            _taken.Visit(program);
        }

        private class FindVarNames
            : BaseTreeVisitor
        {
            private readonly HashSet<string> _names = new HashSet<string>();

            protected override VariableName Visit([NotNull] VariableName var)
            {
                _names.Add(var.Name);

                return base.Visit(var);
            }

            public bool Contains(string name)
            {
                return _names.Contains(name);
            }
        }

        public string Name()
        {
            while (true)
            {
                var n = _inner.Name();
                if (!_taken.Contains(n))
                    return n;
            }
        }
    }

    public class RandomNameGenerator
        : INameGenerator
    {
        private readonly Random _rng;

        public RandomNameGenerator(int seed)
        {
            _rng = new Random(seed);
        }

        public string Name()
        {
            var b = new byte[16];
            _rng.NextBytes(b);

            return $"_{new Guid(b).ToString().Replace("-", "")}";
        }
    }

    public class SequentialNameGenerator
        : INameGenerator
    {
        private readonly string _prefix;
        private const string FirstChars = "abcdefghijklmnopqrstuvwxyz";
        private const string RemainingChars = FirstChars + "1234567890_";
        private int _nextId;

        public SequentialNameGenerator(string prefix)
        {
            _prefix = prefix;
        }

        public string Name()
        {
            return _prefix + GetVar(_nextId++);
        }

        private static string GetVar(int id)
        {
            var first = FirstChars[id % FirstChars.Length];
            id /= FirstChars.Length;

            if (id <= 0)
                return first.ToString();

            var result = new StringBuilder(10);
            while (id > 0)
            {
                result.Append(RemainingChars[id % RemainingChars.Length]);
                id /= RemainingChars.Length;
            }

            return first + result.ToString();
        }
    }

    public interface INameGenerator
    {
        [NotNull] string Name();
    }
}
