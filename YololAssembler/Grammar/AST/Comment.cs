namespace YololAssembler.Grammar.AST
{
    internal class Comment
        : BaseStatement
    {
        public string Value { get; }

        public Comment(string value)
        {
            Value = value;
        }
    }
}
