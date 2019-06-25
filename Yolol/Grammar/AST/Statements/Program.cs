using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;

namespace Yolol.Grammar.AST.Statements
{
    public class Program
    {
        public IReadOnlyList<Line> Lines { get; }

        public Program([NotNull] IEnumerable<Line> lines)
        {
            Lines = lines.ToArray();
        }

        public override string ToString()
        {
            return string.Join("\n", Lines.Select(l => l.ToString()));
        }
    }
}
