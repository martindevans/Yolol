namespace YololAssembler.Grammar.AST
{
    internal class Import
        : BaseStatement
    {
        public string Path { get; }

        public Import(string path)
        {
            Path = path;
        }
    }
}
