namespace Yolol.Execution
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