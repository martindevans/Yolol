using System.Text;
using JetBrains.Annotations;
using Yolol.Execution;
using Yolol.Grammar.AST.Expressions;

namespace Yolol.Grammar.AST.Statements
{
    public class If
        : BaseStatement
    {
        [NotNull] public BaseExpression Condition { get; }
        [NotNull] public BaseStatement TrueBranch { get; }
        [NotNull] public BaseStatement FalseBranch { get; }

        public If([NotNull] BaseExpression condition, [NotNull] BaseStatement trueBranch, [NotNull] BaseStatement falseBranch)
        {
            Condition = condition;
            TrueBranch = trueBranch;
            FalseBranch = falseBranch;
        }

        public override ExecutionResult Evaluate(MachineState state)
        {
            var condition = Condition.Evaluate(state);
            var todo = condition.Number != 0 ? TrueBranch : FalseBranch;
            return todo.Evaluate(state);
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
