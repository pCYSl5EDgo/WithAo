using System;

namespace AoAndSugi.Game.Models.Unit
{
    public struct UnitPaintPoint : IEquatable<UnitPaintPoint>
    {
        public int Value;

        public bool Equals(UnitPaintPoint other)
        {
            return Value == other.Value;
        }

        public override bool Equals(object obj)
        {
            return obj is UnitPaintPoint other && Equals(other);
        }

        public override int GetHashCode()
        {
            return Value;
        }
    }
}