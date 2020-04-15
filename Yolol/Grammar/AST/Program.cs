using System;
using System.Collections.Generic;
using System.Linq;

namespace Yolol.Grammar.AST
{
    public class Program
        : IEquatable<Program>
    {
        public IReadOnlyList<Line> Lines { get; }

        public Program(IEnumerable<Line> lines)
        {
            Lines = lines.ToArray();
        }

        public override bool Equals(object obj)
        {
            return obj is Program p && Equals(p);
        }

        public override int GetHashCode()
        {
            return (Lines != null ? Lines.Select(l => l.GetHashCode()).Aggregate(0, (a, b) => unchecked(a + b)) : 0);
        }

        public static bool operator ==(Program? left, Program? right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(Program? left, Program? right)
        {
            return !Equals(left, right);
        }

        public bool Equals(Program? other)
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
