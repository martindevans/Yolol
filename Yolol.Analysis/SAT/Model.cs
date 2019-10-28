using System;
using System.Collections.Generic;
using Microsoft.Z3;
using Yolol.Execution;
using Yolol.Grammar;

namespace Yolol.Analysis.SAT
{
    public class Model
        : IModel
    {
        public Context Context { get; }
        public Solver Solver { get; }

        private readonly EnumSort _enumType;
        public DatatypeExpr StrType => (DatatypeExpr)_enumType.Consts[0];
        public DatatypeExpr IntType => (DatatypeExpr)_enumType.Consts[1];

        private Dictionary<VariableName, ModelVariable> _variableMapping = new Dictionary<VariableName, ModelVariable>();

        internal Model(Context context, Solver solver)
        {
            Context = context;
            Solver = solver;

            _enumType = context.MkEnumSort("types", "str", "int");
        }

        public void Dispose()
        {
            Context?.Dispose();
            Solver?.Dispose();
        }

        public IModelVariable GetOrCreateVariable(VariableName name)
        {
            if (!_variableMapping.TryGetValue(name, out var v))
            {
                var t = (DatatypeExpr)Context.MkConst(name.Name, _enumType);
                var n = (IntExpr)Context.MkConst(name.Name, Context.IntSort);
                var s = (SeqExpr)Context.MkConst(name.Name, Context.StringSort);
                v = new ModelVariable(this, t, n, s);

                _variableMapping[name] = v;
            }

            return v;
        }

        public IModelVariable TryGetVariable(VariableName name)
        {
            _variableMapping.TryGetValue(name, out var v);
            return v;
        }

        public void Assert(BoolExpr expr)
        {
            Solver.Assert(expr);
        }

        internal Expr MakeNumber(Number value)
        {
            return Context.MkInt((value * 1000).ToString());
        }
    }
}
