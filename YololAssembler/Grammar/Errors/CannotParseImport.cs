namespace YololAssembler.Grammar.Errors
{
    public class CannotParseImport
        : BaseCompileException
    {
        public string Path { get; }
        public Yolol.Grammar.Parser.ParseError ParseError { get; }

        public CannotParseImport(string path, Yolol.Grammar.Parser.ParseError parseError)
            : base($"Cannot parse file import from `{path}`.")
        {
            Path = path;
            ParseError = parseError;
        }
    }
}
