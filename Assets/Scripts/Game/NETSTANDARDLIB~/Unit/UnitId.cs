using System;

namespace AoAndSugi.Game.Models.Unit
{
    public struct UnitId : IEquatable<UnitId>
    {
        public uint Value;

        public UnitId(uint value) => Value = value;

        public bool Equals(UnitId other)
        {
            return Value == other.Value;
        }

        public static bool operator==(UnitId left, UnitId right)
        {
            return left.Value == right.Value;
        }

        public static bool operator!=(UnitId left, UnitId right)
        {
            return left.Value != right.Value;
        }

        public override bool Equals(object obj)
        {
            return obj is UnitId other && Equals(other);
        }

        public override int GetHashCode()
        {
            return (int) Value;
        }
    }
}
