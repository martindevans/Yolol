using JetBrains.Annotations;
using Yolol.Grammar.AST;
using Yolol.Grammar.AST.Statements;

namespace Yolol.Analysis.TreeVisitor
{
    public interface ITreeVisitor
    {
        [NotNull] Program Visit([NotNull] Program program);
    }
}
