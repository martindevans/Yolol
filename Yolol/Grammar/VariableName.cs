using System;
using JetBrains.Annotations;

namespace Yolol.Grammar
{
    public class VariableName
        : IEquatable<VariableName>
    {
        public string Name { get; }

        public bool IsExternal => Name.StartsWith(":");

        public VariableName([NotNull] string name)
        {
            Name = name.ToLowerInvariant();
        }

        public override string ToString()
        {
            return Name;
        }

        public bool Equals(VariableName other)
        {
            if (other is null)
                return false;
            if (ReferenceEquals(this, other))
                return true;
            return string.Equals(Name, other.Name);
        }

        public override bool Equals(object obj)
        {
            if (obj is null)
                return false;
            if (ReferenceEquals(this, obj))
                return true;
            if (obj.GetType() != GetType())
                return false;
            return Equals((VariableName)obj);
        }

        public override int GetHashCode()
        {
            return (Name != null ? Name.GetHashCode() : 0);
        }

        public static bool operator ==([CanBeNull] VariableName left, [CanBeNull] VariableName right)
        {
            return Equals(left, right);
        }

        public static bool operator !=([CanBeNull] VariableName left, [CanBeNull] VariableName right)
        {
            return !Equals(left, right);
        }
    }
}