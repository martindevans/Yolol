namespace Yolol.Execution
{
    public class Variable
        : IVariable
    {
        public Value Value { get; set; }

        public Variable()
        {
            Value = new Value();
        }

        public override string ToString()
        {
            return Value.ToString();
        }
    }
}
