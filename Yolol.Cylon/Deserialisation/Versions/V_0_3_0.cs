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
    internal class V_0_3_0
    {
        private readonly bool _typeExtension;

        public V_0_3_0(bool typeExtension = false)
        {
            _typeExtension = typeExtension;
        }

        [NotNull] public Program Parse([NotNull] string json)
        {
            var jobj = JObject.Parse(json);

            var version = Semver.SemVersion.Parse(jobj["version"].Value<string>());

            if (version < "0.3.0")
                throw new InvalidOperationException("AST version is too low (must be >= 0.3.0)");
            if (version > "0.3.0")
                throw new InvalidOperationException("AST version is too high (must be <= 0.3.0)");

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
            var type = jtok["type"].Value<string>();

            switch (type)
            {
                case "statement::goto":
                    return new Goto(ParseExpression(jtok["expression"]));

                case "statement::if":
                    var @if = ParseExpression(jtok["condition"]);
                    var @true = new StatementList(((JArray)jtok["body"]).Select(ParseStatement));
                    var @false = new StatementList(((JArray)jtok["else_body"]).Select(ParseStatement));
                    return new If(@if, @true, @false);

                case "statement::assignment":
                    return ParseAssignment(jtok);

                case "statement::expression":
                    return new ExpressionWrapper(ParseExpression(jtok["expression"]));

                default:
                    throw new InvalidOperationException($"Cannot parse: Unknown statement type `{type}`");
            }
        }

        [NotNull] private BaseStatement ParseAssignment([NotNull] JToken jtok)
        {
            var id = jtok["identifier"].Value<string>();
            var op = jtok["operator"].Value<string>();
            var exp = ParseExpression(jtok["value"]);

            Execution.Type? type = null;
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

                    var t = Execution.Type.Unassigned;
                    t |= num ? Execution.Type.Number : Execution.Type.Unassigned;
                    t |= str ? Execution.Type.String : Execution.Type.Unassigned;
                    t |= err ? Execution.Type.Error  : Type.Unassigned;
                    type = t;
                }
            }

            if (type != null)
            {
                // todo: do something with type metadata
            }

            switch (op)
            {
                case "=": return new Assignment(new VariableName(id), exp);

                case "+=": return new CompoundAssignment(new VariableName(id), YololBinaryOp.Add, exp);
                case "-=": return new CompoundAssignment(new VariableName(id), YololBinaryOp.Subtract, exp);
                case "*=": return new CompoundAssignment(new VariableName(id), YololBinaryOp.Multiply, exp);
                case "/=": return new CompoundAssignment(new VariableName(id), YololBinaryOp.Divide, exp);
                case "^=": return new CompoundAssignment(new VariableName(id), YololBinaryOp.Exponent, exp);
                case "%=": return new CompoundAssignment(new VariableName(id), YololBinaryOp.Modulo, exp);

                default:
                    throw new InvalidOperationException($"Cannot parse: Unknown op type `{op}`");
            }
        }

        [NotNull] private BaseExpression ParseExpression([NotNull] JToken jtok)
        {
            var type = jtok["type"].Value<string>();

            switch (type)
            {
                case "expression::group":
                    return new Bracketed(ParseExpression(jtok["group"]));

                case "expression::binary_op":
                    return ParseBinaryExpression(jtok);

                case "expression::unary_op":
                    return ParseUnaryExpression(jtok);

                case "expression::number":
                    return new ConstantNumber(decimal.Parse(jtok.Value<string>("num")));

                case "expression::string":
                    return new ConstantString(jtok.Value<string>("str"));

                case "expression::identifier":
                    var name = jtok.Value<string>("name");
                    return new Variable(new VariableName(name));

                default:
                    throw new InvalidOperationException($"Cannot parse: Unknown expression type `{type}`");
            }
        }

        [NotNull] private BaseExpression ParseUnaryExpression([NotNull] JToken jtok)
        {
            var op = jtok["operator"].Value<string>();
            var exp = ParseExpression(jtok["operand"]);

            switch (op)
            {
                case "not": return new Not(exp);

                case "-": return new Negate(exp);

                case "a++": return new PostIncrement(((Variable)exp).Name);
                case "++a": return new PreIncrement(((Variable)exp).Name);
                case "a--": return new PostDecrement(((Variable)exp).Name);
                case "--a": return new PreDecrement(((Variable)exp).Name);

                default:
                    return new Application(new FunctionName(op), exp);
            }
        }

        [NotNull] private BaseExpression ParseBinaryExpression([NotNull] JToken jtok)
        {
            var op = jtok["operator"].Value<string>();

            var left = ParseExpression(jtok["left"]);
            var right = ParseExpression(jtok["right"]);

            switch (op)
            {
                case "*": return new Multiply(left, right);
                case "/": return new Divide(left, right);
                case "+": return new Add(left, right);
                case "-": return new Subtract(left, right);
                case "%": return new Modulo(left, right);
                case "^": return new Exponent(left, right);

                case "and": return new And(left, right);
                case "or": return new Or(left, right);

                case "==": return new EqualTo(left, right);
                case "!=": return new NotEqualTo(left, right);

                case "<=": return new LessThanEqualTo(left, right);
                case "<": return new LessThan(left, right);
                case ">=": return new GreaterThanEqualTo(left, right);
                case ">": return new GreaterThan(left, right);

                default:
                    throw new InvalidOperationException($"Cannot parse: Unknown binary op `{op}`");
            }
        }
    }
}
