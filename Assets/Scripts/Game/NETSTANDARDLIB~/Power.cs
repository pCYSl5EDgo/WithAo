using System;
using UniNativeLinq;
using AoAndSugi.Game.Models.Unit;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;

namespace AoAndSugi.Game.Models
{
    public unsafe struct Power : IDisposable
    {
        public int TeamCount;
        private void* handle;
        public NativeEnumerable<UnitCommonInfo> CommonInfos;
        public NativeEnumerable<UnitType> UnitTypes;
        public NativeEnumerable<UnitInitialCount> InitialCounts;
        public NativeEnumerable<UnitTotalHp> TotalHps;
        public NativeEnumerable<UnitStatus> Statuses;
        public NativeEnumerable<UnitPosition> Positions;
        public NativeEnumerable<UnitMovePower> MovePowers;
        public NativeEnumerable<UnitDestination> Destinations;

        public Power(int capacity)
        {
            if (capacity < 0) throw new ArgumentOutOfRangeException(capacity + " should not be less than 0!");
            if (capacity == 0)
            {
                this = default;
                return;
            }
            TeamCount = default;
            handle = Malloc(capacity);
            CommonInfos = NativeEnumerable<UnitCommonInfo>.Create((UnitCommonInfo*)handle, capacity);
            UnitTypes = NativeEnumerable<UnitType>.Create((UnitType*)(CommonInfos.Ptr + capacity), capacity);
            InitialCounts = NativeEnumerable<UnitInitialCount>.Create((UnitInitialCount*)(UnitTypes.Ptr + capacity), capacity);
            TotalHps = NativeEnumerable<UnitTotalHp>.Create((UnitTotalHp*)(InitialCounts.Ptr + capacity), capacity);
            Statuses = NativeEnumerable<UnitStatus>.Create((UnitStatus*)(TotalHps.Ptr + capacity), capacity);
            Positions = NativeEnumerable<UnitPosition>.Create((UnitPosition*)(Statuses.Ptr + capacity), capacity);
            MovePowers = NativeEnumerable<UnitMovePower>.Create((UnitMovePower*)(Positions.Ptr + capacity), capacity);
            Destinations = NativeEnumerable<UnitDestination>.Create((UnitDestination*)(MovePowers.Ptr + capacity), capacity);
        }

        private static void* Malloc(int capacity)
        {
            return UnsafeUtility.Malloc(capacity * (sizeof(UnitCommonInfo) + sizeof(UnitType) + sizeof(UnitInitialCount) + sizeof(UnitTotalHp) + sizeof(UnitStatus) + sizeof(UnitPosition) + sizeof(UnitMovePower) + sizeof(UnitDestination)), 4, Allocator.Persistent);
        }

        public void RemoveAtSwapBack(int index)
        {
            if (index < 0 || index >= TeamCount) throw new ArgumentOutOfRangeException(index + " should be less than " + TeamCount);
            TeamCount--;

            void SwapBack<T>(NativeEnumerable<T> array, int i, int end) where T : unmanaged
            {
                array[i] = array[end];
            }
            SwapBack(CommonInfos, index, TeamCount);
            SwapBack(UnitTypes, index, TeamCount);
            SwapBack(InitialCounts, index, TeamCount);
            SwapBack(TotalHps, index, TeamCount);
            SwapBack(Statuses, index, TeamCount);
            SwapBack(Positions, index, TeamCount);
            SwapBack(MovePowers, index, TeamCount);
            SwapBack(Destinations, index, TeamCount);
        }

        public void ReAlloc(int capacity)
        {
            if (capacity < 0) throw new ArgumentOutOfRangeException(capacity + " should not be less than 0!");
            if (capacity == 0)
            {
                Dispose();
                this = default;
                return;
            }
            var oldCapacity = this.CommonInfos.Length;
            if (oldCapacity > capacity)
            {
                return;
            }

            var handle = Malloc(capacity);
            var CommonInfos = NativeEnumerable<UnitCommonInfo>.Create((UnitCommonInfo*)handle, capacity);
            var UnitTypes = NativeEnumerable<UnitType>.Create((UnitType*)(CommonInfos.Ptr + capacity), capacity);
            var InitialCounts = NativeEnumerable<UnitInitialCount>.Create((UnitInitialCount*)(UnitTypes.Ptr + capacity), capacity);
            var TotalHps = NativeEnumerable<UnitTotalHp>.Create((UnitTotalHp*)(InitialCounts.Ptr + capacity), capacity);
            var Statuses = NativeEnumerable<UnitStatus>.Create((UnitStatus*)(TotalHps.Ptr + capacity), capacity);
            var Positions = NativeEnumerable<UnitPosition>.Create((UnitPosition*)(Statuses.Ptr + capacity), capacity);
            var MovePowers = NativeEnumerable<UnitMovePower>.Create((UnitMovePower*)(Positions.Ptr + capacity), capacity);
            var Destinations = NativeEnumerable<UnitDestination>.Create((UnitDestination*)(MovePowers.Ptr + capacity), capacity);

            this.CommonInfos.CopyTo(CommonInfos.Ptr);
            this.UnitTypes.CopyTo(UnitTypes.Ptr);
            this.InitialCounts.CopyTo(InitialCounts.Ptr);
            this.TotalHps.CopyTo(TotalHps.Ptr);
            this.Statuses.CopyTo(Statuses.Ptr);
            this.MovePowers.CopyTo(MovePowers.Ptr);
            this.Destinations.CopyTo(Destinations.Ptr);

            var teamCount = TeamCount;

            Dispose();

            this.TeamCount = teamCount;
            this.handle = handle;
            this.CommonInfos = CommonInfos;
            this.UnitTypes = UnitTypes;
            this.InitialCounts = InitialCounts;
            this.TotalHps = TotalHps;
            this.Statuses = Statuses;
            this.Positions = Positions;
            this.MovePowers = MovePowers;
            this.Destinations = Destinations;
        }

        public void Dispose()
        {
            if (handle == null) return;
            UnsafeUtility.Free(handle, Allocator.Persistent);
            this = default;
        }

        public void CopyTo(ref Power power)
        {
            power.ReAlloc(TeamCount);

            CommonInfos.Take(TeamCount).CopyTo(power.CommonInfos.Ptr);
            UnitTypes.Take(TeamCount).CopyTo(power.UnitTypes.Ptr);
            InitialCounts.Take(TeamCount).CopyTo(power.InitialCounts.Ptr);
            TotalHps.Take(TeamCount).CopyTo(power.TotalHps.Ptr);
            Statuses.Take(TeamCount).CopyTo(power.Statuses.Ptr);
            Positions.Take(TeamCount).CopyTo(power.Positions.Ptr);
            MovePowers.Take(TeamCount).CopyTo(power.MovePowers.Ptr);
            Destinations.Take(TeamCount).CopyTo(power.Destinations.Ptr);
        }
    }
}