using AoAndSugi.Game.Models;
using AoAndSugi.Game.Models.Unit;
using UniNativeLinq;
using Unity.Collections;
using Unity.Mathematics;
using UnityEngine;

namespace AoAndSugi.Game.IO
{
    public sealed class DrawUnit : IDrawUnit
    {
        private const int MaxDrawCount = 0x400;
        private readonly Matrix4x4[] matrices = new Matrix4x4[MaxDrawCount];
        private readonly Material drawUnitMaterial;
        private readonly uint ownerPowerId;

        public DrawUnit(uint ownerPowerId, Material drawUnitMaterial)
        {
            this.ownerPowerId = ownerPowerId;
            this.drawUnitMaterial = drawUnitMaterial;
        }

        public void Draw(ref Turn turn)
        {
            ref var myPower = ref turn.Powers[ownerPowerId];
            var count = 0;
            foreach (ref var power in turn.Powers)
            {
                var positions = new SelectEnumerable<NativeEnumerable<int2>, NativeEnumerable<int2>.Enumerator, int2, float2, SelectorInt2ToFloat2>(power.Positions.Take(MaxDrawCount - count).Cast<UnitPosition, int2>(), default).ToNativeEnumerable(Allocator.Temp);
                if(positions.Length == 0) continue;
                positions.Dispose(Allocator.Temp);
            }
        }
        
        private readonly struct SelectorInt2ToFloat2 : IRefAction<int2, float2>
        {
            public void Execute(ref int2 point, ref float2 value) => value = point;
        }
    }
}