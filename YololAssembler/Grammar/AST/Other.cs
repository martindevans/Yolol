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

            // Find comments and trim them out
            var idx = Content.IndexOf("##", StringComparison.OrdinalIgnoreCase);
            if (idx != -1)
            {
                // Remove the comment
                Content = Content[..idx];

                // If there was space trailing the line leading up to the comment, remove it
                Content = Content.TrimEnd(' ');
            }
        }

        internal static string Trim(string input)
        {
            var trimmed = string.Join("",
                input
                    .Split(new[] {"\r\n", "\r", "\n"}, StringSplitOptions.RemoveEmptyEntries)
                    .Select(l => l.TrimStart(' '))
                    .Select(l => l.TrimStart('\t'))
                    .Select(l => l.Replace(";", " "))
            );

            return trimmed;
        }
    }
}
