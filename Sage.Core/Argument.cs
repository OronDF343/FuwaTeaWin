using System;

namespace Sage.Core
{
    public class Argument : IEquatable<Argument>
    {
        public Argument(string fullName, string shortName = null, bool hasValue = false)
        {
            FullName = fullName;
            ShortName = shortName;
            HasValue = hasValue;
        }

        public string FullName { get; }
        public string ShortName { get; }
        public bool HasValue { get; }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;
            return Equals((Argument)obj);
        }

        public bool Equals(Argument other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return string.Equals(FullName, other.FullName);
        }

        public override int GetHashCode()
        {
            return FullName != null ? FullName.GetHashCode() : 0;
        }
    }
}
