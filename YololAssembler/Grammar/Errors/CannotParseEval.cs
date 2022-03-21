namespace YololAssembler.Grammar.Errors
{
    public class CannotParseEval
        : BaseCompileException
    {
        public string Expression { get; }
        public Yolol.Grammar.Parser.ParseError ParseError { get; }

        public CannotParseEval(string expression, Yolol.Grammar.Parser.ParseError parseError, string eval)
            : base($"Cannot parse expression as Yolol in `{eval}({expression})`.\n\n{parseError}")
        {
            Expression = expression;
            ParseError = parseError;
        }
    }
}
