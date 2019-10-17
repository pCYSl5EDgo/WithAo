using UniNativeLinq;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using UnsafeUtilityEx = UniNativeLinq.UnsafeUtilityEx;

namespace AoAndSugi.Game
{
    internal static class ReAllocHelper
    {
        public static unsafe void ReAlloc<T>(this ref NativeEnumerable<T> collection, long capacity) where T : unmanaged
        {
            if (capacity < collection.Length) return;
            var tmp = NativeEnumerable<T>.Create(UnsafeUtilityEx.Malloc<T>(capacity, Allocator.Persistent), capacity);
            collection.CopyTo(tmp.Ptr);
            UnsafeUtility.MemClear(tmp.Ptr + collection.Length, sizeof(T) * (capacity - collection.Length));
            collection.Dispose(Allocator.Persistent);
            collection = tmp;
        }
    }
}