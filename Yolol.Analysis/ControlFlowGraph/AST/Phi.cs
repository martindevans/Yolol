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

        public Phi([NotNull] ISingleStaticAssignmentTable ssa, [NotNull] IReadOnlyList<VariableName> assignedNames)
        {
            if (assignedNames.Count == 0)
                throw new ArgumentException("Must specify one or more assigned names");
            if (assignedNames.Select(ssa.BaseName).GroupBy(a => a).Count() != 1)
                throw new ArgumentException("Not all variables in Phi function share a same base variable", nameof(assignedNames));

            BaseVariable = ssa.BaseName(assignedNames[0]);
            AssignedNames = assignedNames.Distinct().ToArray();

            SSA = ssa;
        }

        public Phi([NotNull] ISingleStaticAssignmentTable ssa, [NotNull] params VariableName[] assignedNames)
            : this(ssa, (IReadOnlyList<VariableName>)assignedNames)
        {
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
            if (other == null)
                return false;

            if (!other.BaseVariable.Equals(BaseVariable))
                return false;

            if (other.AssignedNames.Count != AssignedNames.Count)
                return false;

            return other.AssignedNames.OrderBy(a => a.Name).Zip(AssignedNames.OrderBy(a => a.Name), (a, b) => a.Equals(b)).All(a => a);
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
