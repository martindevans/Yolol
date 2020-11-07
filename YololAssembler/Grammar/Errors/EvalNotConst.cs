namespace YololAssembler.Grammar.Errors
{
    public class EvalNotConst
        : BaseCompileException
    {
        public Yolol.Grammar.AST.Expressions.BaseExpression Expression { get; }

        public EvalNotConst(Yolol.Grammar.AST.Expressions.BaseExpression expression)
            : base($"Expression pass to eval is not constant `{expression}`.")
        {
            Expression = expression;
        }
    }
}
