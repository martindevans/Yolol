using YololEmulator.Execution;
using YololEmulator.Grammar;

namespace YololEmulator.Tests.Scripts
{
    public static class TestExecutor
    {
        public static MachineState Execute(params string[] lines)
        {
            var state = new MachineState();

            var pc = 0;
            foreach (var line in lines)
            {
                var tokens = Parser.TryTokenize(line);
                var parsed = Parser.TryParse(tokens.Value);
                pc = parsed.Value.Evaluate(pc, state);
            }
            
            return state;
        }
    }
}
