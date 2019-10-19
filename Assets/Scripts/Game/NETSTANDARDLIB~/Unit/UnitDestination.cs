using System;
using Unity.Mathematics;

namespace AoAndSugi.Game.Models.Unit
{
    public struct UnitDestination : IEquatable<UnitDestination>
    {
        public int2 Value;

        public override string ToString() => Value.ToString();

        public UnitDestination(int2 value) => Value = value;

        public bool Equals(UnitDestination other)
        {
            return Value.Equals(other.Value);
        }

        public override bool Equals(object obj)
        {
            return obj is UnitDestination other && Equals(other);
        }

        public override int GetHashCode()
        {
            return Value.GetHashCode();
        }
    }
}