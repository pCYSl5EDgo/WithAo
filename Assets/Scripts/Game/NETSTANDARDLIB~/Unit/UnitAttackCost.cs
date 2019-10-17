using System;

namespace AoAndSugi.Game.Models.Unit
{
    public struct UnitAttackCost : IEquatable<UnitAttackCost>
    {
        public int Value;

        public bool Equals(UnitAttackCost other)
        {
            return Value == other.Value;
        }

        public override bool Equals(object obj)
        {
            return obj is UnitAttackCost other && Equals(other);
        }

        public override int GetHashCode()
        {
            return Value;
        }
    }
}