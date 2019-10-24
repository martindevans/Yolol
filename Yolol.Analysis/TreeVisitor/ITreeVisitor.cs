using JetBrains.Annotations;
using Yolol.Grammar.AST;

namespace Yolol.Analysis.TreeVisitor
{
    public interface ITreeVisitor
    {
        [NotNull] Program Visit([NotNull] Program program);
    }
}
