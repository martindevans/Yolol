using System;

namespace Yolol.Execution.Attributes
{
    /// <summary>
    /// Marks a parameter as not important (value does not affect the outcome) for a `WillThrow` method.
    /// </summary>
    [AttributeUsage(AttributeTargets.Parameter)]
    public class IgnoreParamAttribute
        : Attribute
    {
    }
}
