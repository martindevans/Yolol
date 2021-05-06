namespace YololAssembler.Grammar.Errors
{
    public class TooManySubstitutions
        : BaseCompileException
    {
        internal TooManySubstitutions(string original, string result, int matches)
            : base($"Too many substitutions made while processing input:\n\n\t`{original}`\n\nResult after {matches} substitutions:\n\n\t`{result}`")
        {
        }
    }
}
