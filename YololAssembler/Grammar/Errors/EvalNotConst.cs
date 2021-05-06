namespace YololAssembler.Grammar.Errors
{
    public class EvalNotConst
        : BaseCompileException
    {
        public Yolol.Grammar.AST.Expressions.BaseExpression Expression { get; }

        public EvalNotConst(Yolol.Grammar.AST.Expressions.BaseExpression expression)
            : base($"Attempted to evaluate a non-constant expression: `Eval({expression})`")
        {
            Expression = expression;
        }
    }
}
