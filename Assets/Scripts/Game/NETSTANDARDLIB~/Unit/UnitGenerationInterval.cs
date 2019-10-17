using System;

namespace AoAndSugi.Game.Models.Unit
{
    public struct UnitGenerationInterval : IEquatable<UnitGenerationInterval>
    {
        public uint Value;

        public bool Equals(UnitGenerationInterval other)
        {
            return Value == other.Value;
        }

        public override bool Equals(object obj)
        {
            return obj is UnitGenerationInterval other && Equals(other);
        }

        public override int GetHashCode()
        {
            return (int) Value;
        }
    }
}