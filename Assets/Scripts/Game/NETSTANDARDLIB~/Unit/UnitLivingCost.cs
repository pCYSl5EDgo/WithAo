using System;

namespace AoAndSugi.Game.Models.Unit
{
    public struct UnitLivingCost : IEquatable<UnitLivingCost>
    {
        public int Value;

        public bool Equals(UnitLivingCost other)
        {
            return Value == other.Value;
        }

        public override bool Equals(object obj)
        {
            return obj is UnitLivingCost other && Equals(other);
        }

        public override int GetHashCode()
        {
            return Value;
        }
    }
}