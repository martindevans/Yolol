using JetBrains.Annotations;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Yolol.Execution;
using Yolol.Grammar;

namespace YololEmulator.Tests
{
    public static class TestExecutor
    {
        [NotNull] public static MachineState Execute([NotNull] params string[] lines)
        {
            var state = new MachineState(new ConstantNetwork(), new DefaultIntrinsics());

            var pc = 0;
            while (pc < 20)
            {
                if (pc >= lines.Length)
                    break;

                var line = lines[pc];
                var tokens = Tokenizer.TryTokenize(line);
                Assert.IsTrue(tokens.HasValue, tokens.FormatErrorMessageFragment());

                var parsed = Parser.TryParse(tokens.Value);
                Assert.IsTrue(parsed.HasValue, parsed.FormatErrorMessageFragment());

                pc = parsed.Value.Evaluate(pc, state);
            }

            return state;
        }
    }
}
