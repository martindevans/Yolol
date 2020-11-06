namespace YololAssembler.Grammar.Errors
{
    public class CannotResolveImport
        : BaseCompileException
    {
        public CannotResolveImport(string message)
            : base($"Cannot interpret imported path `{message}` as a file path or a Uri")
        {
        }
    }
}
