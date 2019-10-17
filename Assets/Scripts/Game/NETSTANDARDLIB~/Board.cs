using System;
using System.Collections;
using System.Collections.Generic;
using UniNativeLinq;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Mathematics;
using UnsafeUtilityEx = UniNativeLinq.UnsafeUtilityEx;

namespace AoAndSugi.Game.Models
{
    public unsafe struct Board : IDisposable, IRefEnumerable<NativeEnumerable<Cell>.Enumerator, Cell>
    {
        public NativeEnumerable<Cell> Cells;

        public ref Cell this[int width, int2 position] => ref this[position.x + position.y * width]; 

        public Board(int2 size)
        {
            var count = size.x * size.y;
            #if DEBUG
            if (math.any(size < 0)) throw new ArgumentOutOfRangeException(size.x + ", " + size.y + " should not be less than 0!");
            #endif
            if (math.any(size == 0))
            {
                Cells = default;
                return;
            }
            Cells = NativeEnumerable<Cell>.Create(UnsafeUtilityEx.Malloc<Cell>(count, Allocator.Persistent), count);
        }

        public void Clear() => UnsafeUtility.MemClear(Cells.Ptr, sizeof(Cell) * Cells.Length);

        public void Dispose()
        {
            Cells.Dispose(Allocator.Persistent);
        }

        IEnumerator<Cell> IEnumerable<Cell>.GetEnumerator() => Cells.GetEnumerator();

        public bool CanFastCount() => true;

        public bool Any() => Cells.Any();

        public int Count() => Cells.Count();

        public long LongCount() => Cells.LongCount();

        public long CopyTo(Cell* destination) => Cells.CopyTo(destination);
        public bool CopyTo(ref Board board)
        {
            var cellsPtr = board.Cells.Ptr;
            if (cellsPtr == null)
                return false;
            Cells.CopyTo(cellsPtr);
            return true;
        }

        public NativeEnumerable<Cell> ToNativeEnumerable(Allocator allocator) => Cells.ToNativeEnumerable(allocator);

        public NativeArray<Cell> ToNativeArray(Allocator allocator) => Cells.ToNativeArray(allocator);

        public Cell[] ToArray() => Cells.ToArray();

        public bool CanIndexAccess() => Cells.CanIndexAccess();

        public ref Cell this[long index] => ref Cells[index];

        IEnumerator IEnumerable.GetEnumerator() => ((IEnumerable) Cells).GetEnumerator();

        public NativeEnumerable<Cell>.Enumerator GetEnumerator() => Cells.GetEnumerator();
    }
}