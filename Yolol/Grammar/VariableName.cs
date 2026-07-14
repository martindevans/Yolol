using System;

namespace Yolol.Grammar
{
    public class VariableName
        : IEquatable<VariableName>
    {
        public string Name { get; }

        internal string PureName { get; private set; }
        
        public bool IsExternal => Name.StartsWith(':');

        public VariableName(string name)
        {
            // Normalise name
            Name = name.ToLowerInvariant();

            // Slice out the "pure" name part
            if (IsExternal)
                PureName = Name[1..];
            else
                PureName = Name;
        }

        public override string ToString()
        {
            return Name;
        }

        public bool Equals(VariableName? other)
        {
            if (other is null)
                return false;
            if (ReferenceEquals(this, other))
                return true;
            return string.Equals(Name, other.Name, StringComparison.Ordinal);
        }

        public override bool Equals(object? obj)
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
            return HashCode.Combine(Name);
        }

        public static bool operator ==(VariableName? left, VariableName? right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(VariableName? left, VariableName? right)
        {
            return !Equals(left, right);
        }
    }
}