using System.Runtime.InteropServices;

namespace Yolol.DLL
{
    public static class Class1
    {
        [UnmanagedCallersOnly(EntryPoint = "add")]
        public static int Add(int a, int b)
        {
            return a + b;
        }
    }
}