using System;
using System.Linq;
using JetBrains.Annotations;
using Newtonsoft.Json.Linq;
using Yolol.Grammar;
using Yolol.Grammar.AST;
using Yolol.Grammar.AST.Expressions;
using Yolol.Grammar.AST.Expressions.Binary;
using Yolol.Grammar.AST.Expressions.Unary;
using Yolol.Grammar.AST.Statements;

namespace Yolol.Cylon.Serialisation
{
    public class AstSerializer
    {
        [NotNull] public JObject Serialize([NotNull] Program program)
        {
            return new JObject {
                ["version"] = "1.0.0",
                ["program"] = SerializeProgram(program)
            };
        }
        
        [NotNull] private JToken SerializeProgram([NotNull] Program program)
        {
            return new JObject {
                ["type"] = "program",
                ["lines"] = new JArray(program.Lines.Select(SerializeLine).ToArray<object>())
            };
        }

        [NotNull] private JToken SerializeLine([NotNull] Line line)
        {
            return new JObject {
                ["type"] = "line",
                ["code"] = SerializeStatementList(line.Statements)
            };
        }

        [NotNull] private JArray SerializeStatementList([NotNull] StatementList stmts)
        {
            return new JArray(stmts.Statements.Select(SerializeStatement).ToArray<object>());
        }

        [NotNull] private JToken SerializeStatement([NotNull] BaseStatement stmt)
        {
            switch (stmt)
            {
                case Goto g:
                    return new JObject {
                        ["type"] = "statement::goto",
                        ["expression"] = SerializeExpression(g.Destination)
                    };

                case If i:
                    return new JObject {
                        ["type"] = "statement::if",
                        ["condition"] = SerializeExpression(i.Condition),
                        ["body"] = SerializeStatementList(i.TrueBranch),
                        ["else_body"] = SerializeStatementList(i.FalseBranch)
                    };

                case Assignment a:
                    return SerializeAssignment(a);
                
                case ExpressionWrapper e:
                    return new JObject {
                        ["type"] = "statement::expression",
                        ["expression"] = SerializeExpression(e.Expression)
                    };

                case StatementList _:
                    throw new NotSupportedException();

                default:
                    throw new NotSupportedException($"Cannot serialize statement type `{stmt.GetType().Name}`");
            }

        }

        [NotNull] private JToken SerializeAssignment([NotNull] Assignment assignment)
        {
            var result = new JObject {
                ["identifier"] = assignment.Left.Name,
            };

            var type = "assign";
            if (assignment is CompoundAssignment compound)
            {
                switch (compound.Op)
                {
                    case YololBinaryOp.Add:
                        type = "assign_add";
                        break;
                    case YololBinaryOp.Subtract:
                        type = "assign_sub";
                        break;
                    case YololBinaryOp.Multiply:
                        type = "assign_mul";
                        break;
                    case YololBinaryOp.Divide:
                        type = "assign_div";
                        break;
                    case YololBinaryOp.Modulo:
                        type = "assign_mod";
                        break;
                    default:
                        // ReSharper disable once NotResolvedInText
                        throw new NotSupportedException($"Invalid compound assignment op `{compound.Op}`");
                }

                result["value"] = SerializeExpression(compound.Expression);
            }
            else
            {
                result["value"] = SerializeExpression(assignment.Right);
            }

            result["type"] = $"statement::assignment::{type}";

            return result;
        }

        [NotNull] private JToken SerializeExpression([NotNull] BaseExpression expr)
        {
            switch (expr)
            {
                case ConstantNumber num:
                    return new JObject {
                        ["type"] = "expression::number",
                        ["num"] = num.Value.ToString()
                    };

                case ConstantString str:
                    return new JObject {
                        ["type"] = "expression::string",
                        ["str"] = str.Value
                    };

                case Variable var:
                    return SerializeIdentifier(var.Name);

                case Bracketed brk:
                    return new JObject {
                        ["type"] = "expression::unary_op::parentheses",
                        ["operand"] = SerializeExpression(brk.Expression)
                    };

                case PostDecrement postdec:
                    return new JObject {
                        ["type"] = "expression::modify_op::post_decrement",
                        ["operand"] = SerializeIdentifier(postdec.Name)
                    };

                case PreDecrement predec:
                    return new JObject {
                        ["type"] = "expression::modify_op::pre_decrement",
                        ["operand"] = SerializeIdentifier(predec.Name)
                    };

                case PostIncrement postinc:
                    return new JObject {
                        ["type"] = "expression::modify_op::post_increment",
                        ["operand"] = SerializeIdentifier(postinc.Name)
                    };

                case PreIncrement preinc:
                    return new JObject {
                        ["type"] = "expression::modify_op::pre_increment",
                        ["operand"] = SerializeIdentifier(preinc.Name)
                    };

                case Add add: return SerializeBinary(add, "add");
                case Subtract sub: return SerializeBinary(sub, "subtract");
                case Multiply mul: return SerializeBinary(mul, "multiply");
                case Divide div: return SerializeBinary(div, "divide");

                case Exponent exp: return SerializeBinary(exp, "exponent");
                case Modulo mod: return SerializeBinary(mod, "modulo");

                case And and: return SerializeBinary(and, "and");
                case Or or: return SerializeBinary(or, "or");

                case GreaterThan cmp: return SerializeBinary(cmp, "greater_than");
                case GreaterThanEqualTo cmp: return SerializeBinary(cmp, "greater_than_or_equal_to");
                case LessThan cmp: return SerializeBinary(cmp, "less_than");
                case LessThanEqualTo cmp: return SerializeBinary(cmp, "less_than_or_equal_to");
                case EqualTo cmp: return SerializeBinary(cmp, "equal_to");
                case NotEqualTo cmp: return SerializeBinary(cmp, "not_equal_to");

                default: 
                    throw new NotSupportedException($"Cannot serialize expression type `{expr.GetType().Name}`");
            }
        }

        [NotNull] private JToken SerializeBinary([NotNull] BaseBinaryExpression bin, string type)
        {
            return new JObject {
                ["type"] = $"expression::binary_op::{type}",
                ["left"] = SerializeExpression(bin.Left),
                ["right"] = SerializeExpression(bin.Right),
            };
        }

        [NotNull] private JToken SerializeIdentifier([NotNull] VariableName name)
        {
            return new JObject {
                ["type"] = "expression::identifier",
                ["name"] = name.Name
            };
        }
    }
}
