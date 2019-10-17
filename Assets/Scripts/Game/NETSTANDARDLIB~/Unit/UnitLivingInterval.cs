using System;

namespace AoAndSugi.Game.Models.Unit
{
    public struct UnitLivingInterval : IEquatable<UnitLivingInterval>
    {
        public uint Value;

        public bool Equals(UnitLivingInterval other)
        {
            return Value == other.Value;
        }

        public override bool Equals(object obj)
        {
            return obj is UnitLivingInterval other && Equals(other);
        }

        public override int GetHashCode()
        {
            return (int) Value;
        }
    }
}