using System.Collections.Concurrent;
using System.Text;
using JetBrains.Annotations;
using Yolol.Grammar;

namespace Yolol.Analysis.Reduction
{
    public class VariableSimplificationVisitor
        : BaseTreeVisitor
    {
        private readonly INameGenerator _names;
        private readonly ConcurrentDictionary<string, string> _remap = new ConcurrentDictionary<string, string>();
        
        public VariableSimplificationVisitor([CanBeNull] INameGenerator names = null)
        {
            _names = names ?? new SequentialNameGenerator();
        }

        protected override VariableName Visit(VariableName var)
        {
            if (var.IsExternal)
                return var;
            else
                return new VariableName(_remap.GetOrAdd(var.Name, _names.Name(var.Name)));
        }
    }

    public class SequentialNameGenerator
        : INameGenerator
    {
        private const string FirstChars = "abcdefghijklmnopqrstuvwxyz";
        private const string RemainingChars = FirstChars + "1234567890_";
        private int _nextId;

        public string Name(string name)
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
        string Name(string name);
    }
}
