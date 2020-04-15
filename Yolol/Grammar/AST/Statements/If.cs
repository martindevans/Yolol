using System;
using System.Text;
using Yolol.Execution;
using Yolol.Grammar.AST.Expressions;

namespace Yolol.Grammar.AST.Statements
{
    public class If
        : BaseStatement, IEquatable<If>
    {
        public override bool CanRuntimeError => Condition.CanRuntimeError || TrueBranch.CanRuntimeError || FalseBranch.CanRuntimeError;

        public BaseExpression Condition { get; }
        public StatementList TrueBranch { get; }
        public StatementList FalseBranch { get; }

        public If(BaseExpression condition,  StatementList trueBranch, StatementList falseBranch)
        {
            Condition = condition;
            TrueBranch = trueBranch;
            FalseBranch = falseBranch;
        }

        public override ExecutionResult Evaluate(MachineState state)
        {
            var condition = Condition.Evaluate(state);
            var todo = condition.ToBool() ? TrueBranch : FalseBranch;
            return todo.Evaluate(state);
        }

        public bool Equals(If? other)
        {
            return other != null
                && other.Condition.Equals(Condition)
                && other.TrueBranch.Equals(TrueBranch)
                && other.FalseBranch.Equals(FalseBranch);
        }

        public override bool Equals(BaseStatement? other)
        {
            return other is If @if
                && @if.Equals(this);
        }

        public override string ToString()
        {
            var builder = new StringBuilder();
            builder.Append("if ");
            builder.Append(Condition);
            builder.Append(" then");

            var ts = TrueBranch.ToString();
            if (!string.IsNullOrWhiteSpace(ts))
            {
                builder.Append(" ");
                builder.Append(ts);
            }

            var fs = FalseBranch.ToString();
            if (!string.IsNullOrWhiteSpace(fs))
            {
                builder.Append(" else ");
                builder.Append(fs);
            }

            builder.Append(" end");

            return builder.ToString();
        }
    }
}
