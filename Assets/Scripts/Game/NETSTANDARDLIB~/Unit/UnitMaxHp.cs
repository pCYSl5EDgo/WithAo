using System;

namespace AoAndSugi.Game.Models.Unit
{
    public struct UnitMaxHp : IEquatable<UnitMaxHp>
    {
        public uint Value;

        public bool Equals(UnitMaxHp other)
        {
            return Value == other.Value;
        }

        public override bool Equals(object obj)
        {
            return obj is UnitMaxHp other && Equals(other);
        }

        public override int GetHashCode()
        {
            return (int) Value;
        }
    }
}