using JetBrains.Annotations;
using Yolol.Grammar.AST.Statements;

namespace Yolol.Analysis
{
    public interface ITreeVisitor
    {
        [NotNull] Program Visit([NotNull] Program program);
    }
}
