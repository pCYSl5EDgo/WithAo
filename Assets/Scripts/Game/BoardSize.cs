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
}