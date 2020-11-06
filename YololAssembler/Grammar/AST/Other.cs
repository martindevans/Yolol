using System;
using System.Linq;

namespace YololAssembler.Grammar.AST
{
    internal class Other
        : BaseStatement
    {
        public string Content { get; }

        public Other(string content)
        {
            Content = content;
        }

        internal static string Trim(string input)
        {
            return string.Join("",
                input
                    .Split(new[] {"\r\n", "\r", "\n"}, StringSplitOptions.RemoveEmptyEntries)
                    .Select(l => l.TrimStart(' '))
                    .Select(l => l.TrimStart('\t'))
                    .Select(l => l.Replace(";", " "))
            );
        }
    }
}
