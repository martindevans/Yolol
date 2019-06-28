using System;
using System.Linq;
using JetBrains.Annotations;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Yolol.Execution;
using Yolol.Grammar;
using Yolol.Grammar.AST.Statements;

namespace YololEmulator.Tests
{
    public static class TestExecutor
    {
        [NotNull]
        public static Program Parse([NotNull] params string[] lines)
        {
            var tokens = Tokenizer.TryTokenize(string.Join("\n", lines));
            Assert.IsTrue(tokens.HasValue, tokens.FormatErrorMessageFragment());

            var parsed = Parser.TryParseProgram(tokens.Value);
            Assert.IsTrue(parsed.HasValue, parsed.FormatErrorMessageFragment());

            return parsed.Value;
        }

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

                var parsed = Parser.TryParseLine(tokens.Value);
                Assert.IsTrue(parsed.HasValue, parsed.FormatErrorMessageFragment());

                pc = parsed.Value.Evaluate(pc, state);
            }

            return state;
        }

        public static (MachineState, MachineState) Equivalence([NotNull] Func<Program, Program> transform, [NotNull] params string[] lines)
        {
            var prog1 = Parse(lines);
            var m1 = Execute(prog1.Lines.Select(l => l.ToString()).ToArray());

            var prog2 = transform(prog1);
            var m2 = Execute(prog2.Lines.Select(l => l.ToString()).ToArray());

            return (m1, m2);
        }
    }
}
