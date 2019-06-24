namespace Yolol.Execution
{
    public class NullDeviceNetwork
        : IDeviceNetwork
    {
        public IVariable Get(string name)
        {
            return new Variable();
        }
    }
}
