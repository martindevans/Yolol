namespace YololAssembler.Grammar.AST
{
    class LineLabel
        : BaseStatement
    {
        public string Name { get; }

        public LineLabel(string name)
        {
            Name = name;
        }
    }
}
