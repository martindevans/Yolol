using Yolol.Grammar.AST;

namespace Yolol.Analysis.TreeVisitor
{
    public interface ITreeVisitor
    {
        Program Visit(Program program);
    }
}
