using Yolol.Grammar;
using Yolol.Grammar.AST.Expressions;
using Yolol.Grammar.AST.Statements;

namespace Yolol.Analysis.ControlFlowGraph.AST
{
    public class TypedAssignment
        : Assignment
    {
        public Execution.Type Type { get; }

        public TypedAssignment(Execution.Type type, VariableName left, BaseExpression right)
            : base(left, right)
        {
            Type = type;
        }

        public override string ToString()
        {
            return $"{Left}({Type})={Right}";
        }
    }
}
