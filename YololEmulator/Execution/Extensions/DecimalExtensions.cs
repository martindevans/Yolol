namespace YololEmulator.Execution.Extensions
{
    public static class DecimalExtensions
    {
        public static string Coerce(this decimal d)
        {
            return d.ToString("#.####");
        }
    }
}
