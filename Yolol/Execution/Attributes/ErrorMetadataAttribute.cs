using System;

namespace Yolol.Execution.Attributes
{
    /// <summary>
    /// Tags a method with methods that check if it might/will throw
    /// </summary>
    [AttributeUsage(AttributeTargets.Method)]
    public sealed class ErrorMetadataAttribute
        : Attribute
    {
        public string WillThrow { get; }
        public string? UnsafeAlternative { get; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="willThrow">The name of a method which checks if the operation (given value(s)) will throw.</param>
        /// <param name="unsafeAlternative">An alternative implementation of the tagged method which does not have any runtime checks</param>
        public ErrorMetadataAttribute(string willThrow, string? unsafeAlternative = null)
        {
            WillThrow = willThrow;
            UnsafeAlternative = unsafeAlternative;
        }
    }
}
