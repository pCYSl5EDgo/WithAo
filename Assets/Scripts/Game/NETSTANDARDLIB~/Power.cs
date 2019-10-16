using System;
using UniNativeLinq;
using AoAndSugi.Game.Models.Unit;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;

namespace AoAndSugi.Game.Models
{
    public unsafe struct Power : IDisposable
    {
        public PowerId Id;
        public int TeamCount;
        public UnitId NextUnitId;
        public NativeEnumerable<SpeciesType> SpeciesTypes;
        public NativeEnumerable<UnitId> UnitIds;
        public NativeEnumerable<UnitType> UnitTypes;
        public NativeEnumerable<UnitInitialCount> InitialCounts;
        public NativeEnumerable<UnitTotalHp> TotalHps;
        public NativeEnumerable<UnitStatus> Statuses;
        public NativeEnumerable<UnitPosition> Positions;
        public NativeEnumerable<UnitMovePower> MovePowers;
        public NativeEnumerable<UnitDestination> Destinations;

        public Power(PowerId id, int capacity)
        {
            if (capacity < 0) throw new ArgumentOutOfRangeException(capacity + " should not be less than 0!");
            if (capacity == 0)
            {
                this = default;
                Id = id;
                return;
            }
            Id = id;
            NextUnitId = default;
            TeamCount = default;
            SpeciesTypes = NativeEnumerable<SpeciesType>.Create((SpeciesType*)Malloc(capacity), capacity);
            UnitIds = NativeEnumerable<UnitId>.Create((UnitId*)(SpeciesTypes.Ptr + capacity), capacity);
            UnitTypes = NativeEnumerable<UnitType>.Create((UnitType*)(UnitIds.Ptr + capacity), capacity);
            InitialCounts = NativeEnumerable<UnitInitialCount>.Create((UnitInitialCount*)(UnitTypes.Ptr + capacity), capacity);
            TotalHps = NativeEnumerable<UnitTotalHp>.Create((UnitTotalHp*)(InitialCounts.Ptr + capacity), capacity);
            Statuses = NativeEnumerable<UnitStatus>.Create((UnitStatus*)(TotalHps.Ptr + capacity), capacity);
            Positions = NativeEnumerable<UnitPosition>.Create((UnitPosition*)(Statuses.Ptr + capacity), capacity);
            MovePowers = NativeEnumerable<UnitMovePower>.Create((UnitMovePower*)(Positions.Ptr + capacity), capacity);
            Destinations = NativeEnumerable<UnitDestination>.Create((UnitDestination*)(MovePowers.Ptr + capacity), capacity);
        }

        private static void* Malloc(int capacity)
            => UnsafeUtility.Malloc(capacity * (sizeof(SpeciesType) + sizeof(UnitId) + sizeof(UnitType) + sizeof(UnitInitialCount) + sizeof(UnitTotalHp) + sizeof(UnitStatus) + sizeof(UnitPosition) + sizeof(UnitMovePower) + sizeof(UnitDestination)), 4, Allocator.Persistent);

        public void RemoveAtSwapBack(int index)
        {
            if (index < 0 || index >= TeamCount) throw new ArgumentOutOfRangeException(index + " should be less than " + TeamCount);
            TeamCount--;

            static void SwapBack<T>(NativeEnumerable<T> array, int i, int end) where T : unmanaged
            {
                array[i] = array[end];
            }
            SwapBack(SpeciesTypes, index, TeamCount);
            SwapBack(UnitIds, index, TeamCount);
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
            var oldCapacity = this.SpeciesTypes.Length;
            if (oldCapacity > capacity)
            {
                return;
            }

            var CommonInfos = NativeEnumerable<SpeciesType>.Create((SpeciesType*)Malloc(capacity), capacity);
            var UnitIds = NativeEnumerable<UnitId>.Create((UnitId*)(CommonInfos.Ptr + capacity), capacity);
            var UnitTypes = NativeEnumerable<UnitType>.Create((UnitType*)(UnitIds.Ptr + capacity), capacity);
            var InitialCounts = NativeEnumerable<UnitInitialCount>.Create((UnitInitialCount*)(UnitTypes.Ptr + capacity), capacity);
            var TotalHps = NativeEnumerable<UnitTotalHp>.Create((UnitTotalHp*)(InitialCounts.Ptr + capacity), capacity);
            var Statuses = NativeEnumerable<UnitStatus>.Create((UnitStatus*)(TotalHps.Ptr + capacity), capacity);
            var Positions = NativeEnumerable<UnitPosition>.Create((UnitPosition*)(Statuses.Ptr + capacity), capacity);
            var MovePowers = NativeEnumerable<UnitMovePower>.Create((UnitMovePower*)(Positions.Ptr + capacity), capacity);
            var Destinations = NativeEnumerable<UnitDestination>.Create((UnitDestination*)(MovePowers.Ptr + capacity), capacity);

            this.SpeciesTypes.CopyTo(CommonInfos.Ptr);
            this.UnitIds.CopyTo(UnitIds.Ptr);
            this.UnitTypes.CopyTo(UnitTypes.Ptr);
            this.InitialCounts.CopyTo(InitialCounts.Ptr);
            this.TotalHps.CopyTo(TotalHps.Ptr);
            this.Statuses.CopyTo(Statuses.Ptr);
            this.MovePowers.CopyTo(MovePowers.Ptr);
            this.Destinations.CopyTo(Destinations.Ptr);

            var teamCount = TeamCount;
            var id = Id;

            Dispose();

            this.Id = id;
            this.TeamCount = teamCount;
            this.SpeciesTypes = CommonInfos;
            this.UnitIds = UnitIds;
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
            if (SpeciesTypes.Ptr == null) return;
            UnsafeUtility.Free(SpeciesTypes.Ptr, Allocator.Persistent);
            this = default;
        }

        public void CopyTo(ref Power power)
        {
            power.ReAlloc(TeamCount);

            power.Id = Id;
            power.NextUnitId = NextUnitId;
            power.TeamCount = TeamCount;

            SpeciesTypes.Take(TeamCount).CopyTo(power.SpeciesTypes.Ptr);
            UnitIds.Take(TeamCount).CopyTo(power.UnitIds.Ptr);
            UnitTypes.Take(TeamCount).CopyTo(power.UnitTypes.Ptr);
            InitialCounts.Take(TeamCount).CopyTo(power.InitialCounts.Ptr);
            TotalHps.Take(TeamCount).CopyTo(power.TotalHps.Ptr);
            Statuses.Take(TeamCount).CopyTo(power.Statuses.Ptr);
            Positions.Take(TeamCount).CopyTo(power.Positions.Ptr);
            MovePowers.Take(TeamCount).CopyTo(power.MovePowers.Ptr);
            Destinations.Take(TeamCount).CopyTo(power.Destinations.Ptr);
        }

        public int DivideNewUnitFromOriginal(int sourceIndex, UnitInitialCount count, UnitInitialHp initialHp, UnitStatus status, UnitDestination destination)
        {
            ref var sourceCount = ref InitialCounts[sourceIndex];
            if (count.Value > sourceCount.Value) return -1;

            sourceCount.Value -= count.Value;

            var index = TeamCount++;

            ReAlloc(TeamCount);
            
            SpeciesTypes[index] = SpeciesTypes[sourceIndex];
            UnitIds[index] = NextUnitId;
            UnitTypes[index] = UnitTypes[sourceIndex];
            InitialCounts[index] = count;
#if DEBUG
            checked
            {
#endif
                TotalHps[index].Value = (int)(initialHp.Value * count.Value);
#if DEBUG
            }
#endif
            Statuses[index] = status;
            Positions[index] = Positions[sourceIndex];
            MovePowers[index] = default;
            Destinations[index] = destination;

            NextUnitId.Value++;

            return index;
        }
    }
}