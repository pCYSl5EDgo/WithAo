﻿using Unity.Mathematics;

namespace AoAndSugi.Game.Models.Unit
{
    public struct UnitDestination
    {
        public int2 Value;

        public override string ToString() => "[" + Value.x + ", " + Value.y + "]";
    }
}