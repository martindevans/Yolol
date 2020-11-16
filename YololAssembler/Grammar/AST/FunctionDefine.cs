using System;
using System.Collections.Generic;
using System.Linq;
using YololAssembler.Grammar.Errors;

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

        protected override string FindRegex => $"{Identifier}\\((?<body>.*?)\\)";

        protected override string Replace(string part)
        {
            var parameters = part.Split(",").Where(a => !string.IsNullOrEmpty(a)).ToArray();

            if (parameters.Length != Arguments.Count)
                throw new IncorrectFunctionDefineArgsCount(Identifier, Arguments.Count, parameters.Length);

            // Convert args into individual find/replace defines
            var defines = new List<FindAndReplace>();
            for (var i = 0; i < Arguments.Count; i++)
            {
                var arg = Arguments[i];
                var val = parameters[i];

                defines.Add(new FindAndReplace(arg, val));
            }

            var replacement = Other.Trim(Replacement);
            var replaced = Apply(replacement, defines);

            return replaced;
        }
    }
}
