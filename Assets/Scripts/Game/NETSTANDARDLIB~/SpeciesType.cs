using System;

namespace AoAndSugi.Game.Models
{
    public struct SpeciesType : IEquatable<SpeciesType>, IComparable<SpeciesType>
    {
        public uint Value;

        public SpeciesType(uint value) => Value = value;

        public bool Equals(SpeciesType other)
        {
            return Value == other.Value;
        }

        public override bool Equals(object obj)
        {
            return obj is SpeciesType other && Equals(other);
        }

        public override int GetHashCode() => (int)Value;

        public int CompareTo(SpeciesType other)
        {
            return Value.CompareTo(other.Value);
        }

        public override string ToString() => Value.ToString();
    }
}