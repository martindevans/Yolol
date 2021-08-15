using System;

namespace Yolol.Execution.Attributes
{
    /// <summary>
    /// Tag a parameter of a method. If the method is successful that means that the parameter must be the given type.
    /// </summary>
    [AttributeUsage(AttributeTargets.Parameter)]
    public class TypeImplicationAttribute
        : Attribute
    {
        public Type Type { get; }

        public TypeImplicationAttribute(Type type)
        {
            Type = type;
        }
    }
}
