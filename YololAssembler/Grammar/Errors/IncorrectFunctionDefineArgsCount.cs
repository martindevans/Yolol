namespace YololAssembler.Grammar.Errors
{
    public class IncorrectFunctionDefineArgsCount
        : BaseCompileException
    {
        public IncorrectFunctionDefineArgsCount(string name, int expected, int actual)
            : base($"Incorrect number of arguments passed to function `{name}` (expected {expected}, got {actual})")
        {
        }
    }
}
