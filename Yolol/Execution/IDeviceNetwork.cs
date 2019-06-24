using JetBrains.Annotations;

namespace Yolol.Execution
{
    public interface IDeviceNetwork
    {
        [NotNull] IVariable Get([NotNull] string name);
    }
}
