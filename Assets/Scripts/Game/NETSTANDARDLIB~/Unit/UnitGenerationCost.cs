using System;

namespace AoAndSugi.Game.Models.Unit
{
    public struct UnitGenerationCost : IEquatable<UnitGenerationCost>
    {
        public int Value;

        public bool Equals(UnitGenerationCost other)
        {
            return Value == other.Value;
        }

        public override bool Equals(object obj)
        {
            return obj is UnitGenerationCost other && Equals(other);
        }

        public override int GetHashCode()
        {
            return Value;
        }
    }
}