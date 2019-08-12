using Microsoft.VisualStudio.TestTools.UnitTesting;
using Yolol.Cylon;

namespace YololEmulator.Tests.CylonAst
{
    [TestClass]
    public class Playground
    {
        [TestMethod]
        public void AST_1()
        {
            const string ast = "{\"version\":\"0.3.0\",\"metadata\":{\"exporter\":\"yoloxide 0.3.3\"},\"program\":{\"type\":\"program\",\"lines\":[{\"type\":\"line\",\"commen" +
                               "t\":\"\",\"code\":[{\"type\":\"statement::assignment\",\"identifier\":\"a\",\"operator\":\"=\",\"value\":{\"type\":\"expression::number\",\"num" +
                               "\":\"1\"}},{\"type\":\"statement::assignment\",\"identifier\":\"b\",\"operator\":\"=\",\"value\":{\"type\":\"expression::string\",\"str\":\"2\"" +
                               "}},{\"type\":\"statement::assignment\",\"identifier\":\"c\",\"operator\":\"=\",\"value\":{\"type\":\"expression::identifier\",\"name\":\":c\"}}" +
                               ",{\"type\":\"statement::assignment\",\"identifier\":\"d\",\"operator\":\"=\",\"value\":{\"type\":\"expression::binary_op\",\"operator\":\"+\",\"" +
                               "left\":{\"type\":\"expression::unary_op\",\"operator\":\"--a\",\"operand\":{\"type\":\"expression::identifier\",\"name\":\"a\"}},\"right\":{\"t" +
                               "ype\":\"expression::binary_op\",\"operator\":\"*\",\"left\":{\"type\":\"expression::unary_op\",\"operator\":\"sin\",\"operand\":{\"type\":\"exp" +
                               "ression::group\",\"group\":{\"type\":\"expression::identifier\",\"name\":\"b\"}}},\"right\":{\"type\":\"expression::unary_op\",\"operator\":\"-" +
                               "\",\"operand\":{\"type\":\"expression::identifier\",\"name\":\"c\"}}}}},{\"type\":\"statement::expression\",\"expression\":{\"type\":\"expressi" +
                               "on::unary_op\",\"operator\":\"a--\",\"operand\":{\"type\":\"expression::identifier\",\"name\":\"e\"}}}]},{\"type\":\"line\",\"comment\":\"\",\"c" +
                               "ode\":[{\"type\":\"statement::if\",\"condition\":{\"type\":\"expression::identifier\",\"name\":\"d\"},\"body\":[{\"type\":\"statement::goto\",\"" +
                               "expression\":{\"type\":\"expression::number\",\"num\":\"1\"}}],\"else_body\":[{\"type\":\"statement::goto\",\"expression\":{\"type\":\"expressio" +
                               "n::number\",\"num\":\"2\"}}]}]}]}}";

            var parser = new CylonParser();
            parser.Parse(ast);
        }
    }
}
