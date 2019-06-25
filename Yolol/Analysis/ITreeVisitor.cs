using Yolol.Grammar.AST.Expressions;
using Yolol.Grammar.AST.Statements;

namespace Yolol.Analysis
{
    public interface ITreeVisitor
    {
        Program Visit(Program program);

        BaseExpression Visit(BaseExpression expression);

        BaseStatement Visit(BaseStatement statement);
    }
}
