using System.Collections.Generic;

namespace YololAssembler.Grammar.AST
{
    internal class FindAndReplace
        : BaseDefine
    {
        public string Identifier { get; }
        public string Replacement { get; }

        public FindAndReplace(string identifier, string replacement)
        {
            Identifier = identifier;
            Replacement = replacement;
        }

        public override string Apply(string input)
        {
            var replacement = Other.Trim(Replacement);
            return input.Replace($"{Identifier}", replacement);
        }
    }
}
