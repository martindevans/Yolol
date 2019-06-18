using System.Linq;
using System.Text;
using Yolol.Execution;
using Yolol.Grammar.AST.Expressions;

namespace Yolol.Grammar.AST.Statements
{
    public class If
        : BaseStatement
    {
        private readonly BaseExpression _condition;
        private readonly BaseStatement[] _trueBranch;
        private readonly BaseStatement[] _falseBranch;

        public If(BaseExpression condition, BaseStatement[] trueBranch, BaseStatement[] falseBranch)
        {
            _condition = condition;
            _trueBranch = trueBranch;
            _falseBranch = falseBranch;
        }

        public override ExecutionResult Evaluate(MachineState state)
        {
            var condition = _condition.Evaluate(state);

            var todo = condition.Number != 0 ? _trueBranch : _falseBranch;

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
            builder.Append(_condition);
            builder.Append(" then");

            if (_trueBranch.Length > 0)
            {
                builder.Append(" ");
                builder.AppendJoin(" ", _trueBranch.Select(a => a.ToString()));
            }

            if (_falseBranch.Length > 0)
            {
                builder.Append(" else ");
                builder.AppendJoin(" ", _falseBranch.Select(a => a.ToString()));
            }

            builder.Append(" end");

            return builder.ToString();
        }
    }
}
