using System;

namespace AoAndSugi.Game.Models.Unit
{
    public struct UnitGenerationRequiredHp : IEquatable<UnitGenerationRequiredHp>
    {
        public int Value;

        public bool Equals(UnitGenerationRequiredHp other)
        {
            return Value == other.Value;
        }

        public override bool Equals(object obj)
        {
            return obj is UnitGenerationRequiredHp other && Equals(other);
        }

        public override int GetHashCode()
        {
            return Value;
        }
    }
}