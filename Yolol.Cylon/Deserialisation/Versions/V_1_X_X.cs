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
using Type = Yolol.Execution.Type;

namespace Yolol.Cylon.Versions
{
    // ReSharper disable once InconsistentNaming
    internal class V_1_X_X
    {
        private readonly bool _typeExtension;

        public V_1_X_X(bool typeExtension = false)
        {
            _typeExtension = typeExtension;
        }

        [NotNull] public Program Parse([NotNull] string json)
        {
            var jobj = JObject.Parse(json);

            var version = Semver.SemVersion.Parse(jobj["version"].Value<string>());

            if (version < "1.0.0")
                throw new InvalidOperationException("AST version is too low (must be >= 1.0.0)");
            if (version >= "2.0.0")
                throw new InvalidOperationException("AST version is too high (must be < 2.0.0)");

            var program = jobj["program"];
            if (program.Type != JTokenType.Object)
                throw new InvalidOperationException("Cannot parse: `program` field is not an object");

            return new Program(((JArray)program["lines"]).Select(ParseLine));
        }
        [NotNull] private Line ParseLine([NotNull] JToken jtok)
            {
                return new Line(new StatementList(((JArray)jtok["code"]).Select(ParseStatement)));
            }

        [NotNull] private BaseStatement ParseStatement([NotNull] JToken jtok)
        {
            var type = jtok["type"].Value<string>().Split(new[] { ':' }, StringSplitOptions.RemoveEmptyEntries);

            if (type[0] != "statement")
                throw new InvalidOperationException($"Expected `statement`, found `{type[0]}`");

            switch (type[1])
            {
                case "goto":
                    return new Goto(ParseExpression(jtok["expression"]));

                case "if":
                    var @if = ParseExpression(jtok["condition"]);
                    var @true = new StatementList(((JArray)jtok["body"]).Select(ParseStatement));
                    var @false = new StatementList(((JArray)jtok["else_body"]).Select(ParseStatement));
                    return new If(@if, @true, @false);

                case "assignment":
                    return ParseAssignment(jtok, type[2]);

                case "expression":
                    return new ExpressionWrapper(ParseExpression(jtok["expression"]));

                default:
                    throw new InvalidOperationException($"Cannot parse: Unknown statement type `{type}`");
            }
        }

        [NotNull] private BaseStatement ParseAssignment([NotNull] JToken jtok, [NotNull] string op)
        {
            var id = jtok["identifier"].Value<string>();
            var expr = ParseExpression(jtok["value"]);

            Type? type = null;
            var typeMeta = jtok["value"]?["metadata"]?["type"];
            if (_typeExtension && typeMeta != null)
            {
                var types = ((JArray)typeMeta["types"]).Values<string>().ToArray();
                var version = Semver.SemVersion.Parse(typeMeta["version"].Value<string>());
                if (version >= "1.0.0" && version <= "2.0.0")
                {
                    var num = types.Contains("number");
                    var str = types.Contains("string");
                    var err = types.Contains("error");

                    var t = Type.Unassigned;
                    t |= num ? Type.Number : Type.Unassigned;
                    t |= str ? Type.String : Type.Unassigned;
                    t |= err ? Type.Error  : Type.Unassigned;
                    type = t;
                }
            }

            if (type != null)
            {
                //todo: do something with type metadata
            }

            switch (op)
            {
                case "assign": return new Assignment(new VariableName(id), expr);

                case "assign_add": return new CompoundAssignment(new VariableName(id), YololBinaryOp.Add, expr);
                case "assign_sub": return new CompoundAssignment(new VariableName(id), YololBinaryOp.Subtract, expr);
                case "assign_mul": return new CompoundAssignment(new VariableName(id), YololBinaryOp.Multiply, expr);
                case "assign_div": return new CompoundAssignment(new VariableName(id), YololBinaryOp.Divide, expr);
                case "assign_mod": return new CompoundAssignment(new VariableName(id), YololBinaryOp.Modulo, expr);

                default:
                    throw new InvalidOperationException($"Cannot parse: Unknown op type `{op}`");
            }
        }

        [NotNull] private BaseExpression ParseExpression([NotNull] JToken jtok)
        {
            var type = jtok["type"].Value<string>().Split(new[] { ':' }, StringSplitOptions.RemoveEmptyEntries);

            if (type[0] != "expression")
                throw new InvalidOperationException($"Expected `expression`, found `{type[0]}`");

            BaseExpression ParseBinary(string op)
            {
                var left = ParseExpression(jtok["left"]);
                var right = ParseExpression(jtok["right"]);

                switch (op)
                {
                    case "multiply": return new Multiply(left, right);
                    case "divide": return new Divide(left, right);
                    case "add": return new Add(left, right);
                    case "subtract": return new Subtract(left, right);
                    case "modulo": return new Modulo(left, right);
                    case "exponent": return new Exponent(left, right);

                    case "and": return new And(left, right);
                    case "or":  return new Or(left, right);

                    case "equal_to": return new EqualTo(left, right);
                    case "not_equal_to": return new NotEqualTo(left, right);

                    case "less_than_or_equal_to": return new LessThanEqualTo(left, right);
                    case "less_than":  return new LessThan(left, right);
                    case "greater_than_or_equal_to": return new GreaterThanEqualTo(left, right);
                    case "greater_than":  return new GreaterThan(left, right);

                    default:
                        throw new InvalidOperationException($"Cannot parse: Unknown binary expression type `{op}`");
                }
            }

            BaseExpression ParseUnary(string op)
            {
                var operand = ParseExpression(jtok["operand"]);

                switch (op)
                {
                    case "factorial":
                        throw new NotSupportedException("Factorial operator is not supported");

                    case "sqrt": 
                    case "sin":
                    case "cos":
                    case "tan":
                    case "asin":
                    case "acos":
                    case "atan":
                        return new Application(new FunctionName(type[2]), operand);

                    case "not":
                        return new Not(operand);

                    case "negate":
                        return new Negate(operand);

                    case "parentheses":
                        return new Bracketed(ParseExpression(jtok["operand"]));

                    default:
                        throw new InvalidOperationException($"Cannot parse: Unknown unary expression type `{op}`");
                }
            }

            BaseExpression ParseModify(string op)
            {
                var name = new VariableName(jtok["operand"]["name"].Value<string>());

                switch (op)
                {
                    case "post_increment": return new PostIncrement(name);
                    case "pre_increment": return new PreIncrement(name);
                    case "post_decrement": return new PostDecrement(name);
                    case "pre_decrement": return new PreDecrement(name);

                    default:
                        throw new InvalidOperationException($"Cannot parse: Unknown modify expression type `{op}`");
                }
            }

            switch (type[1])
            {
                case "binary_op":
                    return ParseBinary(type[2]);

                case "unary_op":
                    return ParseUnary(type[2]);

                case "modify_op":
                    return ParseModify(type[2]);

                case "number":
                    return new ConstantNumber(decimal.Parse(jtok.Value<string>("num")));

                case "string":
                    return new ConstantString(jtok.Value<string>("str"));

                case "identifier":
                    var name = jtok.Value<string>("name");
                    return new Variable(new VariableName(name));

                default:
                    throw new InvalidOperationException($"Cannot parse: Unknown expression type `{type[1]}`");
            }
        }
    }
}
