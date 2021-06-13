using System;
using System.Globalization;
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
    internal class V_0_3_0
    {
        private readonly bool _typeExtension;

        public V_0_3_0(bool typeExtension = false)
        {
            _typeExtension = typeExtension;
        }

        public Program Parse(string json)
        {
            var jobj = JObject.Parse(json);

            var version = SemVersion.Parse(jobj.Tok("version").Value<string>());

            if (version < "0.3.0")
                throw new InvalidOperationException("AST version is too low (must be >= 0.3.0)");
            if (version > "0.3.0")
                throw new InvalidOperationException("AST version is too high (must be <= 0.3.0)");

            var program = jobj.Tok("program");
            if (program.Type != JTokenType.Object)
                throw new ArgumentException("Cannot parse: `program` field is not an object");

            return new Program(((JArray)program.Tok("lines")).Select(ParseLine));
        }

        private Line ParseLine(JToken jtok)
        {
            return new Line(new StatementList(((JArray)jtok.Tok("code")).Select(ParseStatement)));
        }

        private BaseStatement ParseStatement(JToken jtok)
        {
            var type = jtok.Tok("type").Value<string>();

            switch (type)
            {
                case "statement::goto":
                    return new Goto(ParseExpression(jtok.Tok("expression")));

                case "statement::if":
                    var @if = ParseExpression(jtok.Tok("condition"));
                    var @true = new StatementList(((JArray)jtok.Tok("body")).Select(ParseStatement));
                    var @false = new StatementList(((JArray)jtok.Tok("else_body")).Select(ParseStatement));
                    return new If(@if, @true, @false);

                case "statement::assignment":
                    return ParseAssignment(jtok);

                case "statement::expression":
                    return new ExpressionWrapper(ParseExpression(jtok.Tok("expression")));

                default:
                    throw new InvalidOperationException($"Cannot parse: Unknown statement type `{type}`");
            }
        }

        private BaseStatement ParseAssignment(JToken jtok)
        {
            var id = jtok.Tok("identifier").Value<string>();
            var op = jtok.Tok("operator").Value<string>();
            var exp = ParseExpression(jtok.Tok("value"));

            Type? type = null;
            var typeMeta = jtok["value"]?["metadata"]?["type"];
            if (_typeExtension && typeMeta != null)
            {
                var types = ((JArray)typeMeta.Tok("types")).Values<string>().ToArray();
                var version = SemVersion.Parse(typeMeta.Tok("version").Value<string>());
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
                // todo: do something with type metadata
            }

            return op switch {
                "=" => new Assignment(new VariableName(id), exp),
                "+=" => new CompoundAssignment(new VariableName(id), YololBinaryOp.Add, exp),
                "-=" => new CompoundAssignment(new VariableName(id), YololBinaryOp.Subtract, exp),
                "*=" => new CompoundAssignment(new VariableName(id), YololBinaryOp.Multiply, exp),
                "/=" => new CompoundAssignment(new VariableName(id), YololBinaryOp.Divide, exp),
                "^=" => new CompoundAssignment(new VariableName(id), YololBinaryOp.Exponent, exp),
                "%=" => new CompoundAssignment(new VariableName(id), YololBinaryOp.Modulo, exp),
                _ => throw new InvalidOperationException($"Cannot parse: Unknown op type `{op}`")
            };
        }

        private BaseExpression ParseExpression(JToken jtok)
        {
            var type = jtok.Tok("type").Value<string>();

            switch (type)
            {
                case "expression::group":
                    return new Bracketed(ParseExpression(jtok.Tok("group")));

                case "expression::binary_op":
                    return ParseBinaryExpression(jtok);

                case "expression::unary_op":
                    return ParseUnaryExpression(jtok);

                case "expression::number":
                    return new ConstantNumber((Number)decimal.Parse(jtok.Value<string>("num"), CultureInfo.InvariantCulture));

                case "expression::string":
                    return new ConstantString(jtok.Value<string>("str"));

                case "expression::identifier":
                    var name = jtok.Value<string>("name");
                    return new Variable(new VariableName(name));

                default:
                    throw new InvalidOperationException($"Cannot parse: Unknown expression type `{type}`");
            }
        }

        private BaseExpression ParseUnaryExpression(JToken jtok)
        {
            var op = jtok.Tok("operator").Value<string>();
            var exp = ParseExpression(jtok.Tok("operand"));

            return op switch {
                "not" => (BaseExpression)new Not(exp),
                "-" => new Negate(exp),
                "a++" => new PostIncrement(((Variable)exp).Name),
                "++a" => new PreIncrement(((Variable)exp).Name),
                "a--" => new PostDecrement(((Variable)exp).Name),
                "--a" => new PreDecrement(((Variable)exp).Name),
                "abs" => new Abs(exp),
                "sqrt" => new Sqrt(exp),
                "sin" => new Sine(exp),
                "cos" => new Cosine(exp),
                "tan" => new Tangent(exp),
                "asin" => new ArcSine(exp),
                "acos" => new ArcCos(exp),
                "atan" => new ArcTan(exp),
                _ => throw new InvalidOperationException($"Cannot parse: Unknown unary op `{op}`")
            };
        }

        private BaseExpression ParseBinaryExpression(JToken jtok)
        {
            var op = jtok.Tok("operator").Value<string>();

            var left = ParseExpression(jtok.Tok("left"));
            var right = ParseExpression(jtok.Tok("right"));

            return op switch {
                "*" => new Multiply(left, right),
                "/" => new Divide(left, right),
                "+" => new Add(left, right),
                "-" => new Subtract(left, right),
                "%" => new Modulo(left, right),
                "^" => new Exponent(left, right),
                "and" => new And(left, right),
                "or" => new Or(left, right),
                "==" => new EqualTo(left, right),
                "!=" => new NotEqualTo(left, right),
                "<=" => new LessThanEqualTo(left, right),
                "<" => new LessThan(left, right),
                ">=" => new GreaterThanEqualTo(left, right),
                ">" => new GreaterThan(left, right),
                _ => throw new InvalidOperationException($"Cannot parse: Unknown binary op `{op}`")
            };
        }
    }
}
