using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using Yolol.Analysis.ControlFlowGraph.Extensions;
using Yolol.Execution;
using Yolol.Grammar;
using Yolol.Grammar.AST.Expressions;

namespace Yolol.Analysis.ControlFlowGraph.AST
{
    /// <summary>
    /// Represents assignment to a variable from multiple other potential variables (different statically assigned names of the same var)
    /// </summary>
    public class Phi
        : BaseExpression, IEquatable<Phi>
    {
        public VariableName BaseVariable { get; }

        public IReadOnlyList<VariableName> AssignedNames { get; }

        public ISingleStaticAssignmentTable SSA { get; }

        public Phi([NotNull] ISingleStaticAssignmentTable ssa, [NotNull] params VariableName[] assignedNames)
        {
            if (assignedNames.Length == 0)
                throw new ArgumentException("Must specify one or more assigned names");
            if (assignedNames.Select(ssa.BaseName).GroupBy(a => a).Count() != 1)
                throw new ArgumentException("Not all variables in Phi function share a same base variable", nameof(assignedNames));

            BaseVariable = ssa.BaseName(assignedNames[0]);
            AssignedNames = assignedNames.Distinct().ToArray();

            SSA = ssa;
        }

        public override bool IsConstant => false;

        public override bool IsBoolean => false;

        public override bool CanRuntimeError => false;

        public override Value Evaluate(MachineState state)
        {
            throw new InvalidOperationException("Cannot execute `Phi` node");
        }

        public bool Equals([CanBeNull] Phi other)
        {
            return other != null
                && other.BaseVariable.Equals(BaseVariable)
                && other.AssignedNames.Count == AssignedNames.Count
                && other.AssignedNames.OrderBy(a => a.Name).Zip(AssignedNames.OrderBy(a => a), (a, b) => a.Equals(b)).All(a => a);
        }

        public override bool Equals(BaseExpression other)
        {
            return other is Phi a
                && a.Equals(this);
        }

        public override string ToString()
        {
            return $"φ({string.Join(",", AssignedNames)})";
        }
    }
}
