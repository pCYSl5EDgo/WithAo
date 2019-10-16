using System;

namespace AoAndSugi.Game.Models
{
    public struct SpeciesType : IEquatable<SpeciesType>
    {
        public int Value;

        public bool Equals(SpeciesType other)
        {
            return Value == other.Value;
        }

        public override bool Equals(object obj)
        {
            return obj is SpeciesType other && Equals(other);
        }

        public override int GetHashCode()
        {
            return Value;
        }
    }
}