using System;
using Unity.Mathematics;

namespace AoAndSugi.Game.Models
{
    public struct EnergySupplier
    {
        public int Value;
        public int2 Position;

        public EnergySupplier(int value, int2 position)
        {
            Value = value;
            Position = position;
        }

        public int Provide(int needs)
        {
            #if DEBUG
            if (needs < 0) throw new ArgumentOutOfRangeException(needs + " should not be less than 0!");
            #endif
            if (needs > Value)
            {
                var answer = Value;
                Value = 0;
                return answer;
            }
            Value -= needs;
            return needs;
        }
    }
}