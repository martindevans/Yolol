using Yolol.Grammar.AST;

namespace Yolol.Analysis.TreeVisitor
{
    /// <summary>
    /// Visit all all the AST nodes of a program. Visiting produces a new tree with some nodes replaced.
    /// </summary>
    public interface ITreeVisitor
    {
        Program Visit(Program program);
    }
}
