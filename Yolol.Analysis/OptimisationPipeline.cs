using System;
using JetBrains.Annotations;
using Yolol.Analysis.ControlFlowGraph;
using Yolol.Analysis.ControlFlowGraph.Extensions;
using Yolol.Analysis.TreeVisitor.Reduction;
using Yolol.Analysis.Types;
using Yolol.Grammar;
using Yolol.Grammar.AST;
using Type = Yolol.Execution.Type;

namespace Yolol.Analysis
{
    public class OptimisationPipeline
    {
        private readonly (VariableName, Type)[] _typeHints;
        private readonly int _itersLimit;
        private readonly bool _keepTypes;

        public OptimisationPipeline((VariableName, Type)[] typeHints)
        {
            _typeHints = typeHints;
            _itersLimit = int.MaxValue;
        }

        public OptimisationPipeline(int itersLimit, bool keepTypes, (VariableName, Type)[] typeHints)
        {
            _typeHints = typeHints;
            _itersLimit = itersLimit;
            _keepTypes = keepTypes;
        }

        [NotNull] public Program Apply(Program input)
        {
            var result = input.Fixpoint(_itersLimit, p => {

                // Convert input into control flow graph and optimise
                p = Optimise(new Builder(p).Build()).ToYolol();

                // Apply simple AST optimisations
                p = Optimise(p);

                return p;
            });

            // Strip types if necessary
            if (!_keepTypes)
                result = result.StripTypes();

            return result;
        }

        [NotNull] private Program Optimise(Program program)
        {
            // Calculate the value of any constant subexpressions and substitute them into place
            program = program.FoldConstants();

            // Find any constants which are repeated multiple times in the program and store them into a variable on line 1
            program = program.HoistConstants();

            // Find any compound add/sub operations which can be replaced with inc/dec
            //todo: move this into the CFG stage and exploit better typing info
            program = program.CompressCompoundIncrement();

            // Find any `goto` statements at the end of a line which goto the next line and remove them
            program = program.TrailingGotoNextLineElimination();

            // Replace:
            //      `if A then goto B end <fallthrough to next line>`
            // With:
            //      `goto B + (Next_Line * (A != 0))`
            program = program.TrailingConditionalGotoAnyLineCompression();

            // Replace:
            //     `if A then goto B else goto C end`
            // With:
            //     goto A*(B-C)+C
            // Or:
            //     x=C goto (A)*((B)-x)+x
            program = program.ConditionalGotoCompression(new RandomNameGenerator(1));

            // Remove all code after a goto which can never be executed (because the goto jumps away)
            //todo: remove this? CFG does the same with dead block elimination
            program = program.DeadPostGotoElimination();

            // Replace an if branch which assigns a number to the same variable in both branches with an expression that does the same
            //todo: move this into the CFG stage and exploit type info
            program = program.CompressConditionalAssignment();

            // Replace constant values with smaller equivalents e.g. replace `a=4294967296` with `a=2^32`
            program = program.CompressConstants();

            // Replace non-external variables with smaller alternatives
            program = program.SimplifyVariableNames();

            // Replace thing with unnecessary brackets like `(a)` with `a`
            program = program.SimpleBracketElimination();

            return program;
        }

        [NotNull] private IControlFlowGraph Optimise(IControlFlowGraph cfg)
        {
            cfg = cfg.Fixpoint(cf => {
                
                // Convert CFG into SSA form (i.e. each variable is only assigned once)
                cf = cf.StaticSingleAssignment(out var ssa);

                // Infer types for variables
                cf = cf.FlowTypingAssignment(ssa, out var types, _typeHints);

                // Fold constant expression (e.g. replace `a=2+2` with `a=4`
                cf = cf.VisitBlocks(() => new ConstantFoldingVisitor(true));

                // Fold away useless mathematics involving constants (e.g. replace `a=x*1` => `a=x` (if x is a number type)
                // This can emit `Error` expressions, which always evaluate to an error
                cf = cf.VisitBlocks(t => new OpNumByConstNumCompressor(t), types);

                // Replace any expressions involving `Error` subexpressions with `Error()` statements
                cf = cf.VisitBlocks(() => new ErrorCompressor());

                // Find all reads in the entire program and then remove any assignments to variables which are never read
                cf = cf.VisitBlocks(u => new RemoveUnreadAssignments(u, ssa), c => c.FindUnreadAssignments());

                // Reapply type finding just before we do edge trimming (it's very important we have as many types as possible here and some previous ops may have invalidated them)
                cf = cf.FlowTypingAssignment(ssa, out types, _typeHints);

                // Remove edges in the CFG which cannot happen based on type info (e.g. remove `Error` edges if an error cannot happen, or `Continue` edges if an error is guaranteed to happen)
                cf = cf.TypeDrivenEdgeTrimming(types);

                // Remove nodes of the CFG which can never be reached
                cf = cf.RemoveUnreachableBlocks();

                // Remove node of the CFG which contain no statements
                cf = cf.RemoveEmptyBlocks();

                // Merge together blocks which do not need to be separate any more (e.g. there used to be an error which has now been proven impossible)
                cf = cf.MergeAdjacentBasicBlocks();

                // Replace errors with normal control flow where possible (if a node is guaranteed to error, remove the error and just continue as normal to the next line)
                cf = cf.NormalizeErrors();

                // Remove SSA so that it does not interfere with the next iteration
                cf = cf.RemoveStaticSingleAssignment(ssa);

                return cf;
            });

            // Perform DFG optimisation of the graph. This can merge together multiple actions into one statement leaving the graph in a technically invalid state so it must be done after the other passes
            cfg = cfg.Fixpoint(cf =>
            {
                // Convert CFG into SSA form (i.e. each variable is only assigned once)
                cf = cf.StaticSingleAssignment(out var ssa);

                // Fold away unnecessary copies (e.g. replace `b = a c = b` with `b = a c = a`). This leaves useless variables
                cf = cf.FoldUnnecessaryCopies(ssa);

                // Find all reads in the entire program and then remove any assignments to variables which are never read (clean up after previous fold step)
                cf = cf.VisitBlocks(u => new RemoveUnreadAssignments(u, ssa), c => c.FindUnreadAssignments());

                // Remove SSA so that it does not interfere with the next iteration
                cf = cf.RemoveStaticSingleAssignment(ssa);

                return cf;
            });

            return cfg;
        }
    }
}
