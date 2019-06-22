using Yolol.Execution;

namespace Yolol.Grammar.AST.Expressions.Binary
{
    public class ModuloExpression
        : BaseBinaryExpression
    {
        public ModuloExpression(BaseExpression left, BaseExpression right)
            : base(left, right)
        {
        }

        protected override Value Evaluate(string l, string r)
        {
            throw new ExecutionException("Attempted to modulo strings");
        }

        protected override Value Evaluate(Number l, Number r)
        {
            return new Value(l.Value % r.Value);
        }

        protected override Value Evaluate(string l, Number r)
        {
            throw new ExecutionException("Attempted to modulo mixed types");
        }

        protected override Value Evaluate(Number l, string r)
        {
            throw new ExecutionException("Attempted to modulo mixed types");
        }

        public override string ToString()
        {
            return $"{Left}%{Right}";
        }
    }
}
