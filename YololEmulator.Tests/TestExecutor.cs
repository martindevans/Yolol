using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Yolol.Execution;
using Yolol.Grammar;
using Yolol.Grammar.AST;

namespace YololEmulator.Tests
{
    public static class TestExecutor
    {
        public static Program Parse(params string[] lines)
        {
            var tokens = Tokenizer.TryTokenize(string.Join("\n", lines));
            Assert.IsTrue(tokens.HasValue, tokens.FormatErrorMessageFragment());

            var parsed = Parser.TryParseProgram(tokens.Value);
            Assert.IsTrue(parsed.HasValue, parsed.FormatErrorMessageFragment());

            return parsed.Value;
        }


        public static MachineState Execute(params string[] lines)
        {
            return Execute(new ConstantNetwork(), lines);
        }

        public static MachineState Execute(Program p)
        {
            return Execute(new ConstantNetwork(), p);
        }

        public static MachineState Execute(IDeviceNetwork network, params string[] lines)
        {
            var state = new MachineState(network);

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

        public static MachineState Execute(IDeviceNetwork network, Program p)
        {
            var state = new MachineState(network);

            var pc = 0;
            while (pc < 20)
            {
                if (pc >= p.Lines.Count)
                    break;

                var line = p.Lines[pc];
                pc = line.Evaluate(pc, state);
            }

            return state;
        }


        public static MachineState Execute2(int count, params string[] lines)
        {
            return Execute2(count, new ConstantNetwork(), lines);
        }

        public static MachineState Execute2(int count, IDeviceNetwork network, params string[] lines)
        {
            var state = new MachineState(network);

            var pc = 0;
            while (pc < 20)
            {
                if (pc >= lines.Length || count-- <= 0)
                    break;

                var line = lines[pc];
                var tokens = Tokenizer.TryTokenize(line);
                Assert.IsTrue(tokens.HasValue, tokens.FormatErrorMessageFragment());

                var parsed = Parser.TryParseLine(tokens.Value);
                Assert.IsTrue(parsed.HasValue, parsed.FormatErrorMessageFragment());

                try
                {
                    pc = parsed.Value.Evaluate(pc, state);
                }
                catch (ExecutionException)
                {
                    pc++;
                }
                
            }

            return state;
        }

        public static MachineState Execute2(int count, Program p)
        {
            return Execute2(count, new ConstantNetwork(), p);
        }

        public static MachineState Execute2(int count, IDeviceNetwork network, Program p)
        {
            var state = new MachineState(network);

            var pc = 0;
            while (pc < 20)
            {
                if (pc >= p.Lines.Count || count-- <= 0)
                    break;

                var line = p.Lines[pc];

                try
                {
                    pc = line.Evaluate(pc, state);
                }
                catch (ExecutionException)
                {
                    pc++;
                }
            }

            return state;
        }

        public static (MachineState, MachineState) Equivalence(Func<Program, Program> transform, params string[] lines)
        {
            var prog1 = Parse(lines);
            var m1 = Execute(prog1.Lines.Select(l => l.ToString()).ToArray());

            var prog2 = transform(prog1);
            var m2 = Execute(prog2.Lines.Select(l => l.ToString()).ToArray());

            return (m1, m2);
        }
    }
}
