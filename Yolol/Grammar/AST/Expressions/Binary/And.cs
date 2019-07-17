using JetBrains.Annotations;
using Yolol.Execution;

namespace Yolol.Grammar.AST.Expressions.Binary
{
    public class And
        : BaseBinaryExpression
    {
        public override bool CanRuntimeError => Left.CanRuntimeError || Right.CanRuntimeError;

        public override bool IsBoolean => true;

        public And([NotNull] BaseExpression left, [NotNull] BaseExpression right)
            : base(left, right)
        {
        }

        public override string ToString()
        {
            return $"{Left} and {Right}";
        }

        protected override Value Evaluate(string l, string r)
        {
            return new Value(true);
        }

        protected override Value Evaluate(Number l, Number r)
        {
            return new Value(l != 0 && r != 0);
        }

        protected override Value Evaluate(string l, Number r)
        {
            return new Value(r != 0);
        }

        protected override Value Evaluate(Number l, string r)
        {
            return new Value(l != 0);
        }
    }
}
