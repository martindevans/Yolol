using System;

namespace Yolol.Execution.Attributes
{
    /// <summary>
    /// Indicates that the result of a method is "trim safe", i.e. does not need trimming to the max string length
    /// </summary>
    [AttributeUsage(AttributeTargets.Method)]
    public sealed class TrimSafeAttribute
        : Attribute
    {
    }
}
