using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace YololAssembler.Grammar.AST
{
    internal class FunctionDefine
        : BaseDefine
    {
        public string Identifier { get; }
        public IReadOnlyList<string> Arguments { get; }
        public string Replacement { get; }

        public FunctionDefine(string identifier, IReadOnlyList<string> arguments, string replacement)
        {
            Identifier = identifier;
            Replacement = replacement;
            Arguments = arguments;
        }

        public override string Apply(string input)
        {
            var match = Regex.Match(input, $"{Identifier}\\((.*?)\\)");
            if (!match.Success)
                return input;

            var parameters = match.Groups[1].Value.Split(",").Where(a => !string.IsNullOrEmpty(a)).ToArray();

            if (parameters.Length != Arguments.Count)
                throw new InvalidOperationException($"Incorrect number of arguments passed to function `{Identifier}` (expected {Arguments.Count}, got {parameters.Length})");

            // Convert args into individual find/replace defines
            var defines = new List<FindAndReplace>();
            for (var i = 0; i < Arguments.Count; i++)
            {
                var arg = Arguments[i];
                var val = parameters[i];

                defines.Add(new FindAndReplace(arg, val));
            }

            var replacement = Other.Trim(Replacement);
            var replaced = BaseDefine.Apply(replacement, defines);

            var v = input.Substring(0, match.Index)
                 + replaced
                 + input.Substring(0 + match.Index + match.Length);
            return v;
        }
    }
}
