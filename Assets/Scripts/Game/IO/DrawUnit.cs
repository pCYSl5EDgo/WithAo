using AoAndSugi.Game.Models;
using AoAndSugi.Game.Models.Unit;
using UniNativeLinq;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Rendering;

namespace AoAndSugi.Game.IO
{
    using SelectEnumerable = SelectEnumerable<NativeEnumerable<UnitPosition>, NativeEnumerable<UnitPosition>.Enumerator, UnitPosition, float2, DrawUnit.SelectorInt2ToFloat2>;

    public sealed class DrawUnit : IDrawUnit
    {
        private const int MaxDrawCount = 0x400;
        private readonly Matrix4x4[] matrices;
        private readonly Mesh quadMesh;
        private readonly Camera camera;
        private readonly Material drawUnitMaterial;
        private readonly uint ownerPowerId;
        private readonly MaterialPropertyBlock materialPropertyBlock;

        public DrawUnit(uint ownerPowerId, Material drawUnitMaterial, Camera camera, Mesh quadMesh)
        {
            this.ownerPowerId = ownerPowerId;
            this.drawUnitMaterial = drawUnitMaterial;
            this.matrices =  new Matrix4x4[MaxDrawCount];
            this.matrices.AsRefEnumerable().ForEach((ref Matrix4x4 item) => item = Matrix4x4.identity);
            this.camera = camera;
            this.quadMesh = quadMesh;
            this.materialPropertyBlock = new MaterialPropertyBlock();
        }

        public unsafe void Draw(ref Turn turn)
        {
            var count = 0;
            foreach (ref var power in turn.Powers)
            {
                for (var i = 0; i < power.TeamCount;)
                {
                    var positions = new SelectEnumerable(power.Positions.Skip(i).Take(MaxDrawCount - count), default)
                        .ToNativeEnumerable(Allocator.Temp);
                    if (positions.Length == default) continue;
                    fixed (float* dst = &matrices[count].m23)
                    {
                        var powerId = (1f + power.PowerId.Value) / 16f;
                        UnsafeUtility.MemCpyStride(dst,  sizeof(Matrix4x4), &powerId, 0, sizeof(float), positions.Count());
                    }
                    fixed (float* dst = &matrices[count].m03)
                    {
                        UnsafeUtility.MemCpyStride(dst, sizeof(Matrix4x4), positions.Ptr, sizeof(float2), sizeof(float2), positions.Count());
                    }
                    count += positions.Count();
                    positions.Dispose(Allocator.Temp);
                    if (count != MaxDrawCount) continue;
                    Draw(count);
                    count = default;
                }
            }
            if (count != 0)
            {
                Draw(count);
            }
        }

        private void Draw(int count)
        {
            Graphics.DrawMeshInstanced(quadMesh, 0, drawUnitMaterial, matrices, count, materialPropertyBlock, ShadowCastingMode.Off, false, 0, camera, LightProbeUsage.Off, null);
        }

        internal readonly struct SelectorInt2ToFloat2 : IRefAction<UnitPosition, float2>
        {
            public void Execute(ref UnitPosition point, ref float2 value) => value = point.Value;
        }
    }
}