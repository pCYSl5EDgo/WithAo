using System;

namespace AoAndSugi.Game.Models.Unit
{
    public struct UnitTotalHp : IComparable<UnitTotalHp>, IEquatable<UnitTotalHp>
    {
        public int Value;

        public int CompareTo(UnitTotalHp other)
        {
            return Value.CompareTo(other.Value);
        }

        public bool Equals(UnitTotalHp other)
        {
            return Value == other.Value;
        }

        public override bool Equals(object obj)
        {
            return obj is UnitTotalHp other && Equals(other);
        }

        public override int GetHashCode()
        {
            return Value;
        }
    }
}
