using System;
using Yolol.Analysis.ControlFlowGraph.AST;
using Yolol.Execution;
using Yolol.Execution.Extensions;
using Yolol.Grammar;
using Yolol.Grammar.AST.Expressions;
using Yolol.Grammar.AST.Expressions.Binary;
using Yolol.Grammar.AST.Expressions.Unary;
using Yolol.Grammar.AST.Statements;
using Variable = Yolol.Grammar.AST.Expressions.Variable;

namespace Yolol.Analysis.TreeVisitor.Modification
{
    public class SubstituteVariable
        : BaseTreeVisitor
    {
        private readonly VariableName _var;
        private readonly BaseExpression _exp;

        public SubstituteVariable(VariableName var, BaseExpression exp)
        {
            _var = var;
            _exp = exp;
        }

        protected override BaseExpression Visit(Increment inc)
        {
            static BaseExpression HandleExpression(BaseExpression right)
            {
                return right switch {
                    Variable v => new Increment(v.Name),
                    ConstantNumber n => new ConstantNumber(new Add(n, new ConstantNumber(1)).StaticEvaluate().Number),
                    ConstantString s => new ConstantString(s.Value + " "),
                    Bracketed b => HandleExpression(b.Parameter),
                    _ => throw new InvalidOperationException(right.GetType().Name)
                };
            }

            var r = Visit(new Variable(inc.Name));
            return HandleExpression(r);
        }

        protected override BaseExpression Visit(Decrement dec)
        {
            BaseExpression HandleExpression(BaseExpression right)
            {
                switch (right)
                {
                    case Variable v:
                        return new Decrement(v.Name);

                    case ConstantNumber n:
                        return new ConstantNumber(new Subtract(n, new ConstantNumber(1)).StaticEvaluate().Number);

                    case ConstantString s:
                        if (s.Value.Length > 0)
                        {
                            var str = s.Value.ToString();
                            return new ConstantString(new YString(str.Substring(0, str.Length - 1)));
                        }
                        else
                            return new ErrorExpression();

                    case Bracketed b:
                        return HandleExpression(b.Parameter);

                    default:
                        return dec;
                }
            }

            var r = Visit(new Variable(dec.Name));
            return HandleExpression(r);
        }

        protected override BaseExpression Visit(Variable var)
        {
            if (var.Name == _var)
                return new Bracketed(_exp);
            else
                return var;
        }

        protected override BaseStatement Visit(Assignment ass)
        {
            if (ass.Left.Equals(_var))
                return new EmptyStatement();
            else
                return base.Visit(ass);
        }

        protected override BaseStatement Visit(TypedAssignment ass)
        {
            if (ass.Left.Equals(_var))
                return new EmptyStatement();
            else
                return base.Visit(ass);
        }

        protected override BaseStatement Visit(CompoundAssignment ass)
        {
            if (ass.Left.Equals(_var))
                return new EmptyStatement();
            else
                return base.Visit(ass);
        }
    }
}
