using Yolol.Grammar.AST.Expressions;
using Yolol.Grammar.AST.Statements;

namespace Yolol.Analysis
{
    public interface ITreeVisitor
    {
        Line Visit(Line line);

        BaseExpression Visit(BaseExpression expression);

        BaseStatement Visit(BaseStatement statement);
    }
}
