using System;

namespace AoAndSugi.Game.Models
{
    public struct PowerId : IEquatable<PowerId>, IComparable<PowerId>
    {
        public uint Value;

        public PowerId(uint value)
        {
#if DEBUG
            if (value >= 20) throw new ArgumentOutOfRangeException(value + " should not be greater than 20!");
#endif
            Value = value;
        }

        public bool Equals(PowerId other)
        {
            return Value == other.Value;
        }

        public override bool Equals(object obj)
        {
            return obj is PowerId other && Equals(other);
        }

        public override int GetHashCode()
        {
            return (int)Value;
        }

        public int CompareTo(PowerId other)
        {
            return Value.CompareTo(other.Value);
        }

        public override string ToString() => Value.ToString();
    }
}