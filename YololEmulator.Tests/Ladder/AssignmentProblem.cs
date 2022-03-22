using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Yolol.Execution;
using System.Linq;
using System.Text;

namespace YololEmulator.Tests.Ladder
{
    [TestClass]
    public class AssignmentProblem
    {
        [TestMethod]
        public void GenerateAssignmentProblem()
        {
            var input = new List<Dictionary<string, Value>>();
            var output = new List<Dictionary<string, Value>>();

            const int CASE_MIN = 2021;
            const int CASE_MAX = 10000;

            void SingleCase(Random random)
            {
                var agents = new[]
                {
                    GenerateAgent(0, random), GenerateAgent(1, random), GenerateAgent(2, random), GenerateAgent(3, random)
                };

                var best = MoreLinq.MoreEnumerable.Permutations(agents)
                    .Select(a => (a, Cost(a)))
                    .GroupBy(a => a.Item2)
                    .MinBy(a => a.Key)
                    ?.ToList();

                if (best == null || best.Count > 1)
                    return;

                var (agentsInOrder, _) = best[0];

                var inputs = new Dictionary<string, Value>();
                input!.Add(inputs);
                for (var i = 0; i < agents.Length; i++)
                {
                    var agent = agents[i];
                    for (var j = 0; j < Agent.JobCount; j++)
                        inputs.Add($"a{i}j{j}", agent[j]);
                }

                var outputs = new Dictionary<string, Value>();
                output!.Add(outputs);
                var o = new StringBuilder();
                for (var j = 0; j < agentsInOrder.Count; j++)
                    o.Append(agentsInOrder[j].ID);
                outputs.Add("o", o.ToString());
            }

            var rng = new Random(23437);
            for (var i = 0; i < 1000; i++)
                SingleCase(rng);

            Generator.YololLadderGenerator(input, output, shuffle: false);
        }

        private static Number Cost(IList<Agent> agents)
        {
            var cost = Number.Zero;
            for (var i = 0; i < Agent.JobCount; i++)
                cost += agents[i][i];
            return cost;
        }

        private static Agent GenerateAgent(int id, Random random)
        {
            return new Agent(
                id,
                (Number)random.NextDouble(),
                (Number)random.NextDouble(),
                (Number)random.NextDouble()
            );
        }

        private readonly struct Agent
        {
            public const int JobCount = 3;

            public readonly int ID;
            public readonly Number A;
            public readonly Number B;
            public readonly Number C;

            public Number this[int index] => index switch
            {
                0 => A,
                1 => B,
                2 => C,
                _ => throw new IndexOutOfRangeException()
            };

            public Agent(int id, Number a, Number b, Number c)
            {
                ID = id;
                A = a;
                B = b;
                C = c;
            }
        }
    }
}