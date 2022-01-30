using System;
using Yolol.Execution.Extensions;

namespace Yolol.Execution
{
    /// <summary>
    /// Opsque struct which contains information to make constructing a `YString` faster from a known constant string
    /// </summary>
    public readonly struct StringOptimisationData
    {
        internal readonly SaturatingCounters Counts;

        public StringOptimisationData(string constant)
        {
            Counts = constant.AsSpan().CountDigits();
        }
    }
}
