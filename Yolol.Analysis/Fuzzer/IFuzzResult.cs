using System;
using System.Collections.Generic;
using Yolol.Execution;

namespace Yolol.Analysis.Fuzzer
{
    public interface IFuzzResult
        : IEquatable<IFuzzResult>
    {
        int Count { get; }

        IFuzzResultItem this[int index] { get; }
    }

    public interface IFuzzResultItem
        : IEquatable<IFuzzResultItem>
    {
        int Index { get; }

        int IterCount { get; }

        IReadOnlyList<(string, int, Value)> Sets { get; }

        IReadOnlyList<(string, int, Value)> Gets { get; }
    }
}
