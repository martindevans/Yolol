using System;
using System.Linq;
using Newtonsoft.Json.Linq;
using Semver;
using Yolol.Execution;
using Yolol.Grammar;
using Yolol.Grammar.AST;
using Yolol.Grammar.AST.Expressions;
using Yolol.Grammar.AST.Expressions.Binary;
using Yolol.Grammar.AST.Expressions.Unary;
using Yolol.Grammar.AST.Statements;
using Type = Yolol.Execution.Type;
using Variable = Yolol.Grammar.AST.Expressions.Variable;

namespace Yolol.Cylon.Deserialisation.Versions
{
    // ReSharper disable once InconsistentNaming
    internal class V_1_X_X
    {
        private readonly bool _typeExtension;

        public V_1_X_X(bool typeExtension = false)
        {
            _typeExtension = typeExtension;
        }

        public Program Parse(string json)
        {
            var jobj = JObject.Parse(json);

            var version = SemVersion.Parse(jobj.Tok("version").Value<string>()!);

            if (version.LessThan(new SemVersion(1, 0, 0)))
                throw new InvalidOperationException("AST version is too low (must be >= 1.0.0)");
            if (version.GreaterThanOrEqualTo(new SemVersion(2, 0, 0)))
                throw new InvalidOperationException("AST version is too high (must be < 2.0.0)");

            var program = jobj.Tok("program");
            if (program.Type != JTokenType.Object)
                throw new InvalidOperationException("Cannot parse: `program` field is not an object");

            return new Program(((JArray)program.Tok("lines")).Select(ParseLine));
        }
        private Line ParseLine(JToken jtok)
            {
                return new Line(new StatementList(((JArray)jtok.Tok("code")).Select(ParseStatement)));
            }

        private BaseStatement ParseStatement(JToken jtok)
        {
            var maybeType = jtok["type"]?.Value<string>();
            if (maybeType is null)
                throw new InvalidCastException("Cannot parse: null `type` field");

            var type = maybeType.Split(new[] { ':' }, StringSplitOptions.RemoveEmptyEntries);

            if (type[0] != "statement")
                throw new InvalidOperationException($"Expected `statement`, found `{type[0]}`");

            switch (type[1])
            {
                case "goto":
                    return new Goto(ParseExpression(jtok.Tok("expression")));

                case "if":
                    var @if = ParseExpression(jtok.Tok("condition"));
                    var @true = new StatementList(((JArray)jtok.Tok("body")).Select(ParseStatement));
                    var @false = new StatementList(((JArray)jtok.Tok("else_body")).Select(ParseStatement));
                    return new If(@if, @true, @false);

                case "assignment":
                    return ParseAssignment(jtok, type[2]);

                case "expression":
                    return new ExpressionWrapper(ParseExpression(jtok.Tok("expression")));

                default:
                    throw new InvalidOperationException($"Cannot parse: Unknown statement type `{type}`");
            }
        }

        private BaseStatement ParseAssignment(JToken jtok, string op)
        {
            var id = jtok.Tok("identifier").Value<string>();
            if (id is null)
                throw new InvalidCastException("Cannot parse: null `identifier` field");

            var expr = ParseExpression(jtok.Tok("value"));

            Type? type = null;
            var typeMeta = jtok["value"]?["metadata"]?["type"];
            if (_typeExtension && typeMeta != null)
            {
                var types = ((JArray)typeMeta.Tok("types")).Values<string>().ToArray();
                var version = SemVersion.Parse(typeMeta.Tok("version").Value<string>()!);
                if (version.GreaterThanOrEqualTo(new SemVersion(1, 0, 0)) && version.LessThanOrEqualTo(new SemVersion(2, 0, 0)))
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

            return op switch {
                "assign"     => new Assignment(new VariableName(id), expr),
                "assign_add" => new CompoundAssignment(new VariableName(id), YololBinaryOp.Add, expr),
                "assign_sub" => new CompoundAssignment(new VariableName(id), YololBinaryOp.Subtract, expr),
                "assign_mul" => new CompoundAssignment(new VariableName(id), YololBinaryOp.Multiply, expr),
                "assign_div" => new CompoundAssignment(new VariableName(id), YololBinaryOp.Divide, expr),
                "assign_mod" => new CompoundAssignment(new VariableName(id), YololBinaryOp.Modulo, expr),
                _ => throw new InvalidOperationException($"Cannot parse: Unknown op type `{op}`")
            };
        }

        private BaseExpression ParseExpression(JToken jtok)
        {
            var type = jtok.Tok("type").Value<string>()?.Split(new[] { ':' }, StringSplitOptions.RemoveEmptyEntries);
            if (type is null)
                throw new InvalidCastException("Cannot parse: null `type` field");

            if (type[0] != "expression")
                throw new InvalidOperationException($"Expected `expression`, found `{type[0]}`");

            BaseExpression ParseBinary(string op)
            {
                var left = ParseExpression(jtok.Tok("left"));
                var right = ParseExpression(jtok.Tok("right"));

                return op switch {
                    "multiply" => (BaseExpression)new Multiply(left, right),
                    "divide" => new Divide(left, right),
                    "add" => new Add(left, right),
                    "subtract" => new Subtract(left, right),
                    "modulo" => new Modulo(left, right),
                    "exponent" => new Exponent(left, right),
                    "and" => new And(left, right),
                    "or" => new Or(left, right),
                    "equal_to" => new EqualTo(left, right),
                    "not_equal_to" => new NotEqualTo(left, right),
                    "less_than_or_equal_to" => new LessThanEqualTo(left, right),
                    "less_than" => new LessThan(left, right),
                    "greater_than_or_equal_to" => new GreaterThanEqualTo(left, right),
                    "greater_than" => new GreaterThan(left, right),
                    _ => throw new InvalidOperationException($"Cannot parse: Unknown binary expression type `{op}`")
                };
            }

            BaseExpression ParseUnary(string op)
            {
                var operand = ParseExpression(jtok.Tok("operand"));

                return op switch {
                    "factorial" => throw new NotSupportedException("Factorial operator is not supported"),
                    "abs" => (BaseExpression)new Abs(operand),
                    "sqrt" => new Sqrt(operand),
                    "sin" => new Sine(operand),
                    "cos" => new Cosine(operand),
                    "tan" => new Tangent(operand),
                    "asin" => new ArcSine(operand),
                    "acos" => new ArcCos(operand),
                    "atan" => new ArcTan(operand),
                    "not" => new Not(operand),
                    "negate" => new Negate(operand),
                    "parentheses" => new Bracketed(ParseExpression(jtok.Tok("operand"))),
                    _ => throw new InvalidOperationException($"Cannot parse: Unknown unary expression type `{op}`")
                };
            }

            BaseExpression ParseModify(string op)
            {
                var operand = jtok.Tok("operand");
                if (operand is null)
                    throw new InvalidCastException("Cannot parse: null `operand` field");

                var name = operand.Tok("name").Value<string>();
                if (name is null)
                    throw new InvalidCastException("Cannot parse: null `name` field");

                var vname = new VariableName(name);

                return op switch {
                    "post_increment" => new PostIncrement(vname),
                    "pre_increment" => new PreIncrement(vname),
                    "post_decrement" => new PostDecrement(vname),
                    "pre_decrement" => new PreDecrement(vname),
                    _ => throw new InvalidOperationException($"Cannot parse: Unknown modify expression type `{op}`")
                };
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
                    return new ConstantNumber(Number.Parse(jtok.Value<string>("num") ?? "0"));

                case "string":
                    return new ConstantString(jtok.Value<string>("str") ?? "");

                case "identifier":
                    var name = jtok.Value<string>("name");
                    if (name is null)
                        throw new InvalidCastException("Cannot parse: null `name` field");

                    return new Variable(new VariableName(name));

                default:
                    throw new InvalidOperationException($"Cannot parse: Unknown expression type `{type[1]}`");
            }
        }
    }
}
