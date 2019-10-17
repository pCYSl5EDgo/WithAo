using System;

namespace AoAndSugi.Game.Models.Unit
{
    public struct UnitAttackPoint : IEquatable<UnitAttackPoint>
    {
        public int Value;

        public bool Equals(UnitAttackPoint other)
        {
            return Value == other.Value;
        }

        public override bool Equals(object obj)
        {
            return obj is UnitAttackPoint other && Equals(other);
        }

        public override int GetHashCode()
        {
            return Value;
        }
    }
}