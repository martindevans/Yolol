using JetBrains.Annotations;
using Yolol.Execution;

namespace Yolol.Grammar.AST.Expressions.Binary
{
    public class Multiply
        : BaseBinaryExpression
    {
        public Multiply([NotNull] BaseExpression left, [NotNull] BaseExpression right)
            : base(left, right)
        {
        }

        protected override Value Evaluate(string l, string r)
        {
            throw new ExecutionException("Attempted to multiply strings");
        }

        protected override Value Evaluate(Number l, Number r)
        {
            return new Value(l * r);
        }

        protected override Value Evaluate(string l, Number r)
        {
            throw new ExecutionException("Attempted to multiply mixed types");
        }

        protected override Value Evaluate(Number l, string r)
        {
            throw new ExecutionException("Attempted to multiply mixed types");
        }

        public override string ToString()
        {
            return $"{Left}*{Right}";
        }
    }
}
