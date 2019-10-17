using System;

namespace AoAndSugi.Game.Models.Unit
{
    public struct UnitAttackInterval : IEquatable<UnitAttackInterval>
    {
        public uint Value;

        public bool Equals(UnitAttackInterval other)
        {
            return Value == other.Value;
        }

        public override bool Equals(object obj)
        {
            return obj is UnitAttackInterval other && Equals(other);
        }

        public override int GetHashCode()
        {
            return (int) Value;
        }
    }
}