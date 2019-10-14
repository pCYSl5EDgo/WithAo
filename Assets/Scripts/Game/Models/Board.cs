using System;
using UniNativeLinq;
using Unity.Collections;
using Unity.Mathematics;

namespace AoAndSugi.Game.Models
{
    public unsafe struct Board : IDisposable
    {
        public NativeEnumerable<Cell> Cells;

        public Board(int2 size)
        {
            var count = size.x * size.y;
            if (math.any(size < 0)) throw new ArgumentOutOfRangeException(size.x + ", " + size.y + " should not be less than 0!");
            if (math.any(size == 0))
            {
                Cells = default;
                return;
            }
            Cells = NativeEnumerable<Cell>.Create(UnsafeUtilityEx.Malloc<Cell>(count, Allocator.Persistent), count);
        }

        public void Dispose()
        {
            Cells.Dispose(Allocator.Persistent);
        }
    }
}