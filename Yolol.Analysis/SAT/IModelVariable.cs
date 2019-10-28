using System;
using Microsoft.Z3;
using Yolol.Analysis.SAT.Extensions;
using Yolol.Execution;

namespace Yolol.Analysis.SAT
{
    public class ModelVariable
        : IModelVariable
    {
        private readonly Model _model;
        private readonly DatatypeExpr _type;
        private readonly IntExpr _numValue;
        private readonly SeqExpr _strValue;

        public ModelVariable(Model model, DatatypeExpr type, IntExpr num, SeqExpr str)
        {
            _model = model;
            _type = type;
            _numValue = num;
            _strValue = str;
        }

        public bool IsValue(Value v)
        {
            return CanBeValue(v) && CannotBeOtherValue(v);
        }

        public bool CanBeValue(Value v)
        {
            if (v.Type == Execution.Type.String)
            {
                return _model.Solver.IsSatisfiable(
                    _model.Context.MkAnd(
                        _model.Context.MkEq(_type, _model.StrType),
                        _model.Context.MkEq(_strValue, _model.Context.MkString(v.String))
                    )
                );
            }

            if (v.Type == Execution.Type.Number)
            {
                return _model.Solver.IsSatisfiable(
                    _model.Context.MkAnd(
                        _model.Context.MkEq(_type, _model.IntType),
                        _model.Context.MkEq(_numValue, _model.Context.MkInt((v.Number.Value * Number.Scale).ToString()))
                    )
                );
            }

            throw new NotSupportedException($"unknown value type {v.Type}");
        }

        public bool CannotBeValue(Value v)
        {
            //if (v.Type == Execution.Type.String)
                throw new NotImplementedException();

            //var value = _context.MkInt((v.Number * 1000).ToString());

            //// Check that the variable _can_ have the given value
            //_solver.Push();
            //_solver.Assert(_context.MkEq(_expr, value));
            //bool y = _solver.Check() == Status.UNSATISFIABLE;
            //_solver.Pop();

            //return y;
        }

        public bool CannotBeOtherValue(Value v)
        {
            //if (v.Type == Execution.Type.String)
                throw new NotImplementedException();

            //var value = _context.MkInt((v.Number * 1000).ToString());

            //// Check that the variable _cannot_ have any other value
            //_solver.Push();
            //_solver.Assert(_context.MkNot(_context.MkEq(_expr, value)));
            //bool y = _solver.Check() == Status.UNSATISFIABLE;
            //_solver.Pop();

            //return y;
        }

        
    }

    public interface IModelVariable
    {
        bool IsValue(Value v);
    }
}
