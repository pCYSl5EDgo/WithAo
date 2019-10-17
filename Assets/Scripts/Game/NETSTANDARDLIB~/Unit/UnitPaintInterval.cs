using System;

namespace AoAndSugi.Game.Models.Unit
{
    public struct UnitPaintInterval : IEquatable<UnitPaintInterval>
    {
        public uint Value;

        public bool Equals(UnitPaintInterval other)
        {
            return Value == other.Value;
        }

        public override bool Equals(object obj)
        {
            return obj is UnitPaintInterval other && Equals(other);
        }

        public override int GetHashCode()
        {
            return (int) Value;
        }
    }
}