using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;

namespace Yolol.Grammar.AST
{
    public class Program
        : IEquatable<Program>
    {
        public IReadOnlyList<Line> Lines { get; }

        public Program([NotNull] IEnumerable<Line> lines)
        {
            Lines = lines.ToArray();
        }

        public bool Equals([CanBeNull] Program other)
        {
            return other != null
                && other.Lines.Count == Lines.Count 
                && other.Lines.Zip(Lines, (a, b) => a.Equals(b)).All(a => a);
        }

        public override string ToString()
        {
            return string.Join("\n", Lines.Select(l => l.ToString())).TrimEnd('\n');
        }
    }
}
