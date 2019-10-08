using Yolol.Grammar.AST.Statements;
using System.Linq;
using Yolol.Grammar.AST.Expressions;

namespace Yolol.Analysis.TreeVisitor.Reduction
{
    /// <summary>
    /// Remove unnecessary nested statement lists
    /// </summary>
    public class FlattenStatementLists
        : BaseTreeVisitor
    {
        protected override StatementList Visit(StatementList list)
        {
            return base.Visit(new StatementList(list.Statements.SelectMany(item => item is StatementList sl ? sl.Statements : new[] { item })));
        }

        protected override BaseStatement VisitUnknown(BaseStatement statement)
        {
            return statement;
        }

        protected override BaseExpression VisitUnknown(BaseExpression expression)
        {
            return expression;
        }
    }
}
