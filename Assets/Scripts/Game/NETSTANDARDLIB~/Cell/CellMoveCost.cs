using System;

namespace AoAndSugi.Game.Models
{
    public readonly struct CellMoveCost : IEquatable<CellMoveCost>
    {
        public readonly int Value;

        public CellMoveCost(int value)
        {
            Value = value;
        }

        public bool Equals(CellMoveCost other)
        {
            return Value == other.Value;
        }

        public override bool Equals(object obj)
        {
            return obj is CellMoveCost other && Equals(other);
        }

        public override int GetHashCode() => Value;
    }
}