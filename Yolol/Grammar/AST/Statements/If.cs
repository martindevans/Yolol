using System.Collections.Generic;
using System.Linq;
using System.Text;
using Yolol.Execution;
using Yolol.Grammar.AST.Expressions;

namespace Yolol.Grammar.AST.Statements
{
    public class If
        : BaseStatement
    {
        public BaseExpression Condition { get; }
        public IReadOnlyList<BaseStatement> TrueBranch { get; }
        public IReadOnlyList<BaseStatement> FalseBranch { get; }

        public If(BaseExpression condition, BaseStatement[] trueBranch, BaseStatement[] falseBranch)
        {
            Condition = condition;
            TrueBranch = trueBranch;
            FalseBranch = falseBranch;
        }

        public override ExecutionResult Evaluate(MachineState state)
        {
            var condition = Condition.Evaluate(state);

            var todo = condition.Number != 0 ? TrueBranch : FalseBranch;

            if (todo != null)
            {
                foreach (var item in todo)
                {
                    var r = item.Evaluate(state);
                    if (r.Type != ExecutionResultType.None)
                        return r;
                }
            }

            return new ExecutionResult();
        }

        public override string ToString()
        {
            var builder = new StringBuilder();
            builder.Append("if ");
            builder.Append(Condition);
            builder.Append(" then");

            if (TrueBranch.Count > 0)
            {
                builder.Append(" ");
                builder.AppendJoin(" ", TrueBranch.Select(a => a.ToString()));
            }

            if (FalseBranch.Count > 0)
            {
                builder.Append(" else ");
                builder.AppendJoin(" ", FalseBranch.Select(a => a.ToString()));
            }

            builder.Append(" end");

            return builder.ToString();
        }
    }
}
