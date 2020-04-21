using System;
using System.Linq;
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
        public JObject Serialize(Program program)
        {
            return new JObject {
                ["version"] = "1.0.0",
                ["program"] = SerializeProgram(program)
            };
        }
        
        private JToken SerializeProgram(Program program)
        {
            return new JObject {
                ["type"] = "program",
                ["lines"] = new JArray(program.Lines.Select(SerializeLine).ToArray<object>())
            };
        }

        private JToken SerializeLine(Line line)
        {
            return new JObject {
                ["type"] = "line",
                ["code"] = SerializeStatementList(line.Statements)
            };
        }

        private JArray SerializeStatementList(StatementList stmts)
        {
            return new JArray(stmts.Statements.Select(SerializeStatement).ToArray<object>());
        }

        private JToken SerializeStatement(BaseStatement stmt)
        {
            return stmt switch {
                Goto g => new JObject {["type"] = "statement::goto", ["expression"] = SerializeExpression(g.Destination)},
                If i => new JObject {["type"] = "statement::if", ["condition"] = SerializeExpression(i.Condition), ["body"] = SerializeStatementList(i.TrueBranch), ["else_body"] = SerializeStatementList(i.FalseBranch)},
                Assignment a => SerializeAssignment(a),
                ExpressionWrapper e => new JObject {["type"] = "statement::expression", ["expression"] = SerializeExpression(e.Expression)},
                StatementList _ => throw new NotSupportedException(),
                _ => throw new NotSupportedException($"Cannot serialize statement type `{stmt.GetType().Name}`")
            };
        }

        private JToken SerializeAssignment(Assignment assignment)
        {
            var result = new JObject {
                ["identifier"] = assignment.Left.Name,
            };

            var type = "assign";
            if (assignment is CompoundAssignment compound)
            {
                type = compound.Op switch {
                    YololBinaryOp.Add => "assign_add",
                    YololBinaryOp.Subtract => "assign_sub",
                    YololBinaryOp.Multiply => "assign_mul",
                    YololBinaryOp.Divide => "assign_div",
                    YololBinaryOp.Modulo => "assign_mod",
                    _ => throw new NotSupportedException($"Invalid compound assignment op `{compound.Op}`")
                };

                result["value"] = SerializeExpression(compound.Expression);
            }
            else
            {
                result["value"] = SerializeExpression(assignment.Right);
            }

            result["type"] = $"statement::assignment::{type}";

            return result;
        }

        private JToken SerializeExpression(BaseExpression expr)
        {
            return expr switch {
                ConstantNumber num => new JObject {["type"] = "expression::number", ["num"] = num.Value.ToString()},
                ConstantString str => new JObject {["type"] = "expression::string", ["str"] = str.Value},
                Variable var => SerializeIdentifier(var.Name),
                Bracketed brk => new JObject {["type"] = "expression::unary_op::parentheses", ["operand"] = SerializeExpression(brk.Parameter)},
                PostDecrement postdec => new JObject {["type"] = "expression::modify_op::post_decrement", ["operand"] = SerializeIdentifier(postdec.Name)},
                PreDecrement predec => new JObject {["type"] = "expression::modify_op::pre_decrement", ["operand"] = SerializeIdentifier(predec.Name)},
                PostIncrement postinc => new JObject {["type"] = "expression::modify_op::post_increment", ["operand"] = SerializeIdentifier(postinc.Name)},
                PreIncrement preinc => new JObject {["type"] = "expression::modify_op::pre_increment", ["operand"] = SerializeIdentifier(preinc.Name)},
                Add add => SerializeBinary(add, "add"),
                Subtract sub => SerializeBinary(sub, "subtract"),
                Multiply mul => SerializeBinary(mul, "multiply"),
                Divide div => SerializeBinary(div, "divide"),
                Exponent exp => SerializeBinary(exp, "exponent"),
                Modulo mod => SerializeBinary(mod, "modulo"),
                And and => SerializeBinary(and, "and"),
                Or or => SerializeBinary(or, "or"),
                GreaterThan cmp => SerializeBinary(cmp, "greater_than"),
                GreaterThanEqualTo cmp => SerializeBinary(cmp, "greater_than_or_equal_to"),
                LessThan cmp => SerializeBinary(cmp, "less_than"),
                LessThanEqualTo cmp => SerializeBinary(cmp, "less_than_or_equal_to"),
                EqualTo cmp => SerializeBinary(cmp, "equal_to"),
                NotEqualTo cmp => SerializeBinary(cmp, "not_equal_to"),
                Sqrt sqrt => new JObject {["type"] = "expression::unary_op::sqrt", ["operand"] = SerializeExpression(sqrt.Parameter)},
                _ => throw new NotSupportedException($"Cannot serialize expression type `{expr.GetType().Name}`")
            };
        }

        private JToken SerializeBinary(BaseBinaryExpression bin, string type)
        {
            return new JObject {
                ["type"] = $"expression::binary_op::{type}",
                ["left"] = SerializeExpression(bin.Left),
                ["right"] = SerializeExpression(bin.Right),
            };
        }

        private JToken SerializeIdentifier(VariableName name)
        {
            return new JObject {
                ["type"] = "expression::identifier",
                ["name"] = name.Name
            };
        }
    }
}
