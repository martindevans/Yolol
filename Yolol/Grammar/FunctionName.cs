namespace Yolol.Grammar
{
    public class FunctionName
    {
        public string Name { get; }

        public FunctionName(string name)
        {
            Name = name;
        }

        public override string ToString()
        {
            return Name;
        }
    }
}