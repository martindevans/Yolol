using System;
using System.Collections.Generic;
using System.Globalization;
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
        public DatatypeExpr NumType => (DatatypeExpr)_enumType.Consts[1];

        private readonly Dictionary<VariableName, ModelVariable> _variableMapping = new Dictionary<VariableName, ModelVariable>();

        private bool _finalGoto;

        internal Model(Context context, Solver solver)
        {
            Context = context;
            Solver = solver;

            _enumType = context.MkEnumSort("types", "str", "int");
        }

        public void Dispose()
        {
            Context.Dispose();
            Solver.Dispose();
        }

        public Status Check()
        {
            return Solver.Check();
        }

        public ModelVariable GetOrCreateVariable(VariableName name)
        {
            if (!_variableMapping.TryGetValue(name, out var v))
            {
                var t = (DatatypeExpr)Context.MkConst($"typ:{name.Name}", _enumType);
                var n = (IntExpr)Context.MkConst($"num:{name.Name}", Context.IntSort);
                var s = (SeqExpr)Context.MkConst($"str:{name.Name}", Context.StringSort);
                var a = (BoolExpr)Context.MkConst($"inv:{name.Name}", Context.BoolSort);
                v = new ModelVariable(this, t, n, s, a);

                _variableMapping[name] = v;
            }

            return v;
        }

        public IModelVariable? TryGetVariable(VariableName name)
        {
            _variableMapping.TryGetValue(name, out var v);
            return v;
        }

        internal Expr MakeNumber(Number value)
        {
            return Context.MkInt(((decimal)value * 1000).ToString(CultureInfo.InvariantCulture));
        }

        public ModelVariable GetGotoVariable()
        {
            var g = GetOrCreateVariable(new VariableName("goto"));
            _finalGoto = true;
            return g;
        }

        public IModelVariable? TryGetGotoVariable()
        {
            if (_finalGoto)
                return TryGetVariable(new VariableName("goto"));
            return null;
        }

        public IModelVariable? TryGetConditionalVariable()
        {
            throw new NotImplementedException();
        }
    }
}
