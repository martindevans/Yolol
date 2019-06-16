using YololEmulator.Execution;
using YololEmulator.Grammar.AST.Expressions;

namespace YololEmulator.Grammar.AST.Statements
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
    }
}
