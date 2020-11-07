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

        protected override string FindRegex => $"(?<body>{Identifier})";

        protected override string Replace(string part)
        {
            return Replacement;
        }
    }
}
