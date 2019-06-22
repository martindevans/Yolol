namespace Yolol.Grammar
{
    public class VariableName
    {
        public string Name { get; }

        public VariableName(string name)
        {
            Name = name;
        }

        public override string ToString()
        {
            return Name;
        }
    }
}