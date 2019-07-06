using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using Yolol.Grammar.AST.Statements;

namespace Yolol.Analysis.TreeVisitor.Reduction
{
    public class DeadStatementAfterGotoElimination
        : BaseTreeVisitor
    {
        protected override StatementList Visit(StatementList list)
        {
            // Take all statements up until goto. We don't need anything after a goto as it will never execute
            return base.Visit(new StatementList(list.Statements.TakeUntilIncluding(a => a is Goto)));
        }

        protected override BaseStatement Visit(If @if)
        {
            var c = base.Visit(@if.Condition);
            var t = Visit(@if.TrueBranch);
            var f = Visit(@if.FalseBranch);

            if (t.Statements.Last() is Goto && f.Statements.Count > 0)
            {
                return base.Visit(new StatementList(new BaseStatement[] {
                    new If(c, t, new StatementList(Array.Empty<BaseStatement>())),
                    f
                }));
            }
            else
                return base.Visit(new If(c, t, f));
        }
    }

    internal static class Ext
    {
        public static IEnumerable<T> TakeUntilIncluding<T>([NotNull] this IEnumerable<T> list, Func<T, bool> predicate)
        {
            foreach(var el in list)
            {
                yield return el;
                if (predicate(el))
                    yield break;
            }
        }
    }
}
