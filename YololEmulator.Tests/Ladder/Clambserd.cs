using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Yolol.Execution;

namespace YololEmulator.Tests.Ladder
{
    [TestClass]
    public class AlmostRandomAccess
        : BaseGenerator
    {
        private readonly char[] _memory = new char[1024];

        private BaseAccessStrategy? _current;

        [TestMethod]
        public void Generate()
        {
            Array.Fill(_memory, ' ');
            Run(87457, 50000, false, Generator.ScoreMode.BasicScoring);
        }

        protected override bool GenerateCase(Random random, int index, Dictionary<string, Value> inputs, Dictionary<string, Value> outputs)
        {
            if (_current == null || _current.IsComplete)
                _current = PickStrategy(random);

            _current.Generate(_memory, inputs, outputs);
            return true;
        }

        private BaseAccessStrategy PickStrategy(Random random)
        {
            return random.Next(0, 3) switch
            {
                0 => new SingleRandomWrite(random),
                1 => new SingleRandomRead(random),
                2 => new RepeatedDuplicateAccess(random),
                _ => throw new NotImplementedException()
            };
        }

        private static char RandomChar(Random rng)
        {
            const string lower = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
            return lower[rng.Next(lower.Length)];
        }

        private abstract class BaseAccessStrategy
        {
            public abstract bool IsComplete { get; }

            public abstract void Generate(char[] memory, Dictionary<string, Value> inputs, Dictionary<string, Value> outputs);

            protected static void Read(int address, char[] memory, Dictionary<string, Value> inputs, Dictionary<string, Value> outputs)
            {
                inputs.Add("i", (Number)address);
                inputs.Add("m", "r");

                var value = memory[address];
                outputs.Add("v", value.ToString());
            }

            protected static void Write(int address, char[] memory, char value, Dictionary<string, Value> inputs, Dictionary<string, Value> outputs)
            {
                memory[address] = value;

                inputs.Add("i", (Number)address);
                inputs.Add("m", "w");
                inputs.Add("v", value.ToString());
            }
        }

        class SingleRandomRead
            : BaseAccessStrategy
        {
            private readonly Random _rand;

            public override bool IsComplete => _rand.NextDouble() < 0.75;

            public SingleRandomRead(Random rand)
            {
                _rand = rand;
            }

            public override void Generate(char[] memory, Dictionary<string, Value> inputs, Dictionary<string, Value> outputs)
            {
                Read(_rand.Next(1024), memory, inputs, outputs);
            }
        }

        class SingleRandomWrite
            : BaseAccessStrategy
        {
            private readonly Random _rand;

            public override bool IsComplete => _rand.NextDouble() < 0.75;

            public SingleRandomWrite(Random rand)
            {
                _rand = rand;
            }

            public override void Generate(char[] memory, Dictionary<string, Value> inputs, Dictionary<string, Value> outputs)
            {
                var addr = _rand.Next(1024);
                var value = RandomChar(_rand);
                Write(addr, memory, value, inputs, outputs);
            }
        }

        class RepeatedDuplicateAccess
            : BaseAccessStrategy
        {
            private readonly Random _rand;
            private readonly int _addr;

            public override bool IsComplete => _rand.NextDouble() < 0.15;

            public RepeatedDuplicateAccess(Random rand)
            {
                _rand = rand;
                _addr = rand.Next(1024);
            }

            public override void Generate(char[] memory, Dictionary<string, Value> inputs, Dictionary<string, Value> outputs)
            {
                if (_rand.NextDouble() < 0.6)
                    Read(_addr, memory, inputs, outputs);
                else
                    Write(_addr, memory, RandomChar(_rand), inputs, outputs);
            }
        }
    }
}
