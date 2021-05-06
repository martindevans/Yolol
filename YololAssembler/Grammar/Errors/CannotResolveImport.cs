namespace YololAssembler.Grammar.Errors
{
    public class CannotResolveImport
        : BaseCompileException
    {
        public CannotResolveImport(string path)
            : base($"Cannot interpret imported path `{path}` as a file path or a Uri")
        {
        }
    }
}
