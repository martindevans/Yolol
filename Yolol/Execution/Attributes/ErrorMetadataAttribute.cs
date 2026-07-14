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
        /// <summary>
        /// The name of a method which checks if the operation (given value(s)) will throw.
        /// </summary>
        public string WillThrow { get; }

        /// <summary>
        /// An alternative implementation of the tagged method which does not have any runtime checks. You should only
        /// use this alternative if you have first checked that it will not throw!
        /// </summary>
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
