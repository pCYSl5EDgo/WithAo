using System;

namespace AoAndSugi.Game.Models
{
    public struct TurnId : IEquatable<TurnId>, IComparable<TurnId>
    {
        public uint Value;

        public bool Equals(TurnId other)
        {
            return Value == other.Value;
        }

        public override bool Equals(object obj)
        {
            return obj is TurnId other && Equals(other);
        }

        public override int GetHashCode() => (int)Value;

        public int CompareTo(TurnId other)
        {
            return Value.CompareTo(other.Value);
        }
    }
}