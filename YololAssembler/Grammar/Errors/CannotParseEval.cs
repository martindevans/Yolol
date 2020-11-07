namespace YololAssembler.Grammar.Errors
{
    public class CannotParseEval
        : BaseCompileException
    {
        public string Expression { get; }
        public Yolol.Grammar.Parser.ParseError ParseError { get; }

        public CannotParseEval(string expression, Yolol.Grammar.Parser.ParseError parseError)
            : base($"Cannot parse expression pass to eval `{expression}`.")
        {
            Expression = expression;
            ParseError = parseError;
        }
    }
}
