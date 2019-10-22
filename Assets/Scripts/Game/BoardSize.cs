using System.Collections;
using System.Collections.Generic;
using UniNativeLinq;
using Unity.Burst;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;
using Enumerable = System.Linq.Enumerable;

namespace AoAndSugi.Game.Models
{
    public struct BoardSize
    {
        public int2 Value;
    }

    [BurstCompile]
    public struct X : IJob
    {
        private NativeArray<float4x4> answer;
        [NativeDisableUnsafePtrRestriction] private readonly NativeEnumerable<float4x4> enumerable;
        public X(NativeEnumerable<float4x4> enumerable, NativeArray<float4x4> answer)
        {
            this.enumerable = enumerable;
            this.answer = answer;
        }

        public void Execute()
        {
            ref var item = ref answer.AsRefEnumerable()[0];
            enumerable.Aggregate(ref item, (ref float4x4 arg0, ref float4x4 x4) => { arg0 += x4; });
        }
    }
}