namespace Yolol.Analysis.ControlFlowGraph
{
    public enum EdgeType
    {
        Continue,
        RuntimeError,

        ConditionalTrue,
        ConditionalFalse,
        
        GotoConstNum,
        GotoConstStr,
        GotoExpression
    }
}
