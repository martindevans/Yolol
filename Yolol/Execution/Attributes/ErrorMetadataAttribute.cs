using System;

namespace Yolol.Execution.Attributes
{
    /// <summary>
    /// Tags an operator overload with methods that check if it might/will throw
    /// </summary>
    [AttributeUsage(AttributeTargets.Method)]
    public class ErrorMetadataAttribute
        : Attribute
    {
        public bool IsInfallible { get; }
        public string? WillThrow { get; }

        /// <summary>
        /// </summary>
        /// <param name="isInfallible">If true, indicates that this method can never throw a runtime error</param>
        public ErrorMetadataAttribute(bool isInfallible)
        {
            IsInfallible = isInfallible;
            WillThrow = null;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="willThrow">The name of a method which checks if the operation (given value(s)) will throw.</param>
        public ErrorMetadataAttribute(string willThrow)
        {
            IsInfallible = false;
            WillThrow = willThrow;
        }
    }
}
