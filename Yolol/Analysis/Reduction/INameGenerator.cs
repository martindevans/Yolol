using System.Text;

namespace Yolol.Analysis.Reduction
{
    public class SequentialNameGenerator
        : INameGenerator
    {
        private const string FirstChars = "abcdefghijklmnopqrstuvwxyz";
        private const string RemainingChars = FirstChars + "1234567890_";
        private int _nextId;

        public string Name()
        {
            return GetVar(_nextId++);
        }

        private static string GetVar(int id)
        {
            var first = FirstChars[id % FirstChars.Length];
            id /= FirstChars.Length;

            if (id <= 0)
                return first.ToString();

            var result = new StringBuilder(10);
            while (id > 0)
            {
                result.Append(RemainingChars[id % RemainingChars.Length]);
                id /= RemainingChars.Length;
            }

            return first + result.ToString();
        }
    }

    public interface INameGenerator
    {
        string Name();
    }
}
