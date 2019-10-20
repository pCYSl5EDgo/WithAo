using System;

namespace AoAndSugi.Game.Models.Unit
{
    public struct UnitInitialHp : IEquatable<UnitInitialHp>
    {
        public uint Value;

        public bool Equals(UnitInitialHp other)
        {
            return Value == other.Value;
        }

        public override bool Equals(object obj)
        {
            return obj is UnitInitialHp other && Equals(other);
        }

        public override int GetHashCode()
        {
            return (int) Value;
        }

        public override string ToString() => Value.ToString();

        public UnitInitialHp(uint value) => Value = value;
    }
}