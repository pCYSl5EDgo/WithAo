using System;

namespace AoAndSugi.Game.Models.Unit
{
    public struct UnitPaintCost : IEquatable<UnitPaintCost>
    {
        public int Value;

        public bool Equals(UnitPaintCost other)
        {
            return Value == other.Value;
        }

        public override bool Equals(object obj)
        {
            return obj is UnitPaintCost other && Equals(other);
        }

        public override int GetHashCode()
        {
            return Value;
        }
    }
}