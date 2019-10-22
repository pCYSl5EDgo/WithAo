﻿using System;
using System.Runtime.CompilerServices;
using UniNativeLinq;
using AoAndSugi.Game.Models.Unit;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Mathematics;
using UnityEngine;

namespace AoAndSugi.Game.Models
{
    public unsafe struct Power : IDisposable
    {
        public PowerId PowerId;
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
        public NativeEnumerable<ulong> MiscellaneousData;
        public NativeEnumerable<ulong> MiscellaneousData2;
        public NativeEnumerable<TurnId> GenerationTurns;
        private uint knownPowerFlags;

        public Power(PowerId powerId, int capacity)
        {
            if (capacity < 0) throw new ArgumentOutOfRangeException(capacity + " should not be less than 0!");
            if (capacity == 0)
            {
                this = default;
                PowerId = powerId;
                return;
            }
            PowerId = powerId;
            knownPowerFlags = 1U << (int)powerId.Value;
            NextUnitId = default;
            TeamCount = default;
            SpeciesTypes = NativeEnumerable<SpeciesType>.Create((SpeciesType*)Malloc(capacity), capacity);
            UnsafeUtility.MemClear(SpeciesTypes.Ptr, CalcCapacityByteLength(capacity));
            UnitIds = NativeEnumerable<UnitId>.Create((UnitId*)(SpeciesTypes.Ptr + capacity), capacity);
            UnitTypes = NativeEnumerable<UnitType>.Create((UnitType*)(UnitIds.Ptr + capacity), capacity);
            InitialCounts = NativeEnumerable<UnitInitialCount>.Create((UnitInitialCount*)(UnitTypes.Ptr + capacity), capacity);
            TotalHps = NativeEnumerable<UnitTotalHp>.Create((UnitTotalHp*)(InitialCounts.Ptr + capacity), capacity);
            Statuses = NativeEnumerable<UnitStatus>.Create((UnitStatus*)(TotalHps.Ptr + capacity), capacity);
            Positions = NativeEnumerable<UnitPosition>.Create((UnitPosition*)(Statuses.Ptr + capacity), capacity);
            MovePowers = NativeEnumerable<UnitMovePower>.Create((UnitMovePower*)(Positions.Ptr + capacity), capacity);
            Destinations = NativeEnumerable<UnitDestination>.Create((UnitDestination*)(MovePowers.Ptr + capacity), capacity);
            MiscellaneousData = NativeEnumerable<ulong>.Create((ulong*)(Destinations.Ptr + capacity), capacity);
            MiscellaneousData2 = NativeEnumerable<ulong>.Create((ulong*)(MiscellaneousData.Ptr + capacity), capacity);
            GenerationTurns = NativeEnumerable<TurnId>.Create((TurnId*)(MiscellaneousData2.Ptr + capacity), capacity);
        }

        private static void* Malloc(int capacity)
            => UnsafeUtility.Malloc(CalcCapacityByteLength(capacity), 4, Allocator.Persistent);

        private static long CalcCapacityByteLength(long capacity) => capacity * (sizeof(SpeciesType) + sizeof(UnitId) + sizeof(UnitType) + sizeof(UnitInitialCount) + sizeof(UnitTotalHp) + sizeof(UnitStatus) + sizeof(UnitPosition) + sizeof(UnitMovePower) + sizeof(UnitDestination) + sizeof(ulong) * 2 + sizeof(TurnId));

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
            SwapBack(MiscellaneousData, index, TeamCount);
            SwapBack(MiscellaneousData2, index, TeamCount);
            SwapBack(GenerationTurns, index, TeamCount);
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
            if (oldCapacity >= capacity)
            {
                return;
            }

            if (capacity < oldCapacity + (oldCapacity >> 1))
                capacity = (int)(oldCapacity + (oldCapacity >> 1));

            var SpeciesTypes = NativeEnumerable<SpeciesType>.Create((SpeciesType*)Malloc(capacity), capacity);
            UnsafeUtility.MemClear(SpeciesTypes.Ptr, CalcCapacityByteLength(capacity));
            var UnitIds = NativeEnumerable<UnitId>.Create((UnitId*)(SpeciesTypes.Ptr + capacity), capacity);
            var UnitTypes = NativeEnumerable<UnitType>.Create((UnitType*)(UnitIds.Ptr + capacity), capacity);
            var InitialCounts = NativeEnumerable<UnitInitialCount>.Create((UnitInitialCount*)(UnitTypes.Ptr + capacity), capacity);
            var TotalHps = NativeEnumerable<UnitTotalHp>.Create((UnitTotalHp*)(InitialCounts.Ptr + capacity), capacity);
            var Statuses = NativeEnumerable<UnitStatus>.Create((UnitStatus*)(TotalHps.Ptr + capacity), capacity);
            var Positions = NativeEnumerable<UnitPosition>.Create((UnitPosition*)(Statuses.Ptr + capacity), capacity);
            var MovePowers = NativeEnumerable<UnitMovePower>.Create((UnitMovePower*)(Positions.Ptr + capacity), capacity);
            var Destinations = NativeEnumerable<UnitDestination>.Create((UnitDestination*)(MovePowers.Ptr + capacity), capacity);
            var MiscellaneousData = NativeEnumerable<ulong>.Create((ulong*)(Destinations.Ptr + capacity), capacity);
            var MiscellaneousData2 = NativeEnumerable<ulong>.Create((ulong*)(MiscellaneousData.Ptr + capacity), capacity);
            var GenerationTurns = NativeEnumerable<TurnId>.Create((TurnId*)(MiscellaneousData2.Ptr + capacity), capacity);

            this.SpeciesTypes.Take(TeamCount).CopyTo(SpeciesTypes.Ptr);
            this.UnitIds.Take(TeamCount).CopyTo(UnitIds.Ptr);
            this.UnitTypes.Take(TeamCount).CopyTo(UnitTypes.Ptr);
            this.InitialCounts.Take(TeamCount).CopyTo(InitialCounts.Ptr);
            this.TotalHps.Take(TeamCount).CopyTo(TotalHps.Ptr);
            this.Statuses.Take(TeamCount).CopyTo(Statuses.Ptr);
            this.MovePowers.Take(TeamCount).CopyTo(MovePowers.Ptr);
            this.Destinations.Take(TeamCount).CopyTo(Destinations.Ptr);
            this.MiscellaneousData.Take(TeamCount).CopyTo(MiscellaneousData.Ptr);
            this.MiscellaneousData2.Take(TeamCount).CopyTo(MiscellaneousData2.Ptr);
            this.GenerationTurns.Take(TeamCount).CopyTo(GenerationTurns.Ptr);

            var teamCount = TeamCount;
            var id = PowerId;

            Dispose();

            this.PowerId = id;
            this.TeamCount = teamCount;
            this.SpeciesTypes = SpeciesTypes;
            this.UnitIds = UnitIds;
            this.UnitTypes = UnitTypes;
            this.InitialCounts = InitialCounts;
            this.TotalHps = TotalHps;
            this.Statuses = Statuses;
            this.Positions = Positions;
            this.MovePowers = MovePowers;
            this.Destinations = Destinations;
            this.MiscellaneousData = MiscellaneousData;
            this.MiscellaneousData2 = MiscellaneousData2;
            this.GenerationTurns = GenerationTurns;
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

            power.PowerId = PowerId;
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
            MiscellaneousData.Take(TeamCount).CopyTo(power.MiscellaneousData.Ptr);
            MiscellaneousData2.Take(TeamCount).CopyTo(power.MiscellaneousData2.Ptr);
            GenerationTurns.Take(TeamCount).CopyTo(GenerationTurns.Ptr);
        }

        public void CreateNewUnit(SpeciesType speciesType, UnitType unitType, UnitInitialCount initialCount, UnitInitialHp initialHp, UnitPosition position, TurnId turn, ulong miscellaneousDatum = default, ulong miscellaneousDatum2 = default)
        {
            ReAlloc(TeamCount + 1);
            SpeciesTypes[TeamCount] = speciesType;
            UnitIds[TeamCount] = NextUnitId;
            NextUnitId.Value++;
            UnitTypes[TeamCount] = unitType;
            InitialCounts[TeamCount] = initialCount;
            TotalHps[TeamCount].Value = (int)(initialHp.Value * initialCount.Value);
            Statuses[TeamCount] = UnitStatus.Idle;
            Positions[TeamCount] = position;
            MovePowers[TeamCount] = default;
            Destinations[TeamCount] = default;
            MiscellaneousData[TeamCount] = miscellaneousDatum;
            MiscellaneousData2[TeamCount] = miscellaneousDatum2;
            GenerationTurns[TeamCount] = turn;
            TeamCount++;
        }

        public void AddInitialCount(int teamIndex, UnitInitialCount addCount, UnitInitialHp initialHp, TurnId turn)
        {
            InitialCounts[teamIndex].Value += addCount.Value;
            TotalHps[teamIndex].Value += (int)(initialHp.Value * addCount.Value);
            MovePowers[teamIndex] = default;
            GenerationTurns[teamIndex] = turn;
        }

        public void MergeUnits(int mergeLastIndex, int mergeBanishIndex, TurnId turn)
        {
            InitialCounts[mergeLastIndex].Value += InitialCounts[mergeBanishIndex].Value;
            TotalHps[mergeLastIndex].Value += TotalHps[mergeBanishIndex].Value;
            MovePowers[mergeLastIndex] = default;
            GenerationTurns[mergeLastIndex] = turn;
            RemoveAtSwapBack(mergeBanishIndex);
        }

        public int DivideNewUnitFromOriginal(int sourceIndex, UnitInitialCount count, UnitInitialHp initialHp, UnitStatus status, UnitDestination destination, TurnId turn)
        {
            ref var sourceCount = ref InitialCounts[sourceIndex];

            if (count.Value >= sourceCount.Value)
            {
                return RewriteStatus(sourceIndex, status, destination);
            }

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
            MiscellaneousData[index] = MiscellaneousData[sourceIndex];
            MiscellaneousData2[index] = MiscellaneousData2[sourceIndex];
            GenerationTurns[index] = turn;

            NextUnitId.Value++;

            return index;
        }

        public int RewriteStatus(int sourceIndex, UnitStatus status, UnitDestination destination, TurnId turnId)
        {
            Statuses[sourceIndex] = status;
            Destinations[sourceIndex] = destination;
            GenerationTurns[sourceIndex] = turnId;
            return sourceIndex;
        }

        public int RewriteStatus(int sourceIndex, UnitStatus status, UnitDestination destination)
        {
            Statuses[sourceIndex] = status;
            Destinations[sourceIndex] = destination;
            return sourceIndex;
        }

        public uint CalcUnitCountInTeam(int teamIndex, UnitInitialHp initialHp)
        {
            var total = TotalHps[teamIndex].Value;
            var answer = total / initialHp.Value;
            if (answer * initialHp.Value != total)
                answer++;
            var initial = InitialCounts[teamIndex].Value;
            if (answer > initial)
                return initial;
            return (uint)answer;
        }

        public void SetStatusIdle(int teamIndex, TurnId turnId)
        {
            var pos = Positions[teamIndex].Value;
            for (var i = 0; i < TeamCount; i++)
            {
                if (i == teamIndex || Statuses[i] != UnitStatus.Idle || !Positions[i].Value.Equals(pos)) continue;
                MergeUnits(i, teamIndex, turnId);
                return;
            }
            Statuses[teamIndex] = UnitStatus.Idle;
            GenerationTurns[teamIndex] = turnId;
        }

        public void SetStatusRole(int teamIndex, [CallerFilePath] string file = "", [CallerMemberName] string name = "", [CallerLineNumber] int number = 0)
        {
            if (Positions[teamIndex].Value.Equals(new int2(510, 514)))
                Debug.Log("SET ROLE : " + PowerId.Value + " -> " + teamIndex + " : " + Statuses[teamIndex] + "\n" + file + " : " + name + " @ " + number);
            Statuses[teamIndex] = UnitStatus.AdvanceAndRole;
            Destinations[teamIndex].Value = Positions[teamIndex].Value;
        }

        public bool DoesKnowEnemy(PowerId enemyPowerId) => (knownPowerFlags & (1U << (int)enemyPowerId.Value)) != 0U;

        public void SetKnowEnemy(PowerId enemyPowerId, bool value)
        {
            if (value)
            {
                knownPowerFlags |= 1U << (int)enemyPowerId.Value;
            }
            else
            {
                knownPowerFlags &= ~(1U << (int)enemyPowerId.Value);
            }
        }

        /// <summary>
        /// Taken damage
        /// </summary>
        /// <param name="teamIndex">team index damaged</param>
        /// <param name="enemyPower"></param>
        /// <param name="enemyUnitId">enemy unit id</param>
        /// <param name="enemyTeamIndex">enemy unit index</param>
        /// <param name="damageValue">damage</param>
        /// <param name="enemyPowerId">enemy power</param>
        /// <returns>is dead</returns>
        public bool SetDamage(int teamIndex, ref Power enemyPower, UnitId enemyUnitId, int enemyTeamIndex, int damageValue)
        {
            SetKnowEnemy(enemyPower.PowerId, true);
            ref var hp = ref TotalHps[teamIndex].Value;
            hp -= damageValue;
            var isDead = hp <= 0;
            if (isDead)
            {
                RemoveAtSwapBack(teamIndex);
                return true;
            }
            ref var unitStatus = ref Statuses[teamIndex];
            switch (unitStatus)
            {
                case UnitStatus.AskingForDestination:
                case UnitStatus.Ordered:
                case UnitStatus.Dying:
                case UnitStatus.Dead:
                case UnitStatus.AdvanceAndStop:
                case UnitStatus.Generate:
                case UnitStatus.Battle:
                    break;
                case UnitStatus.Idle:
                case UnitStatus.AdvanceAndRole:
                case UnitStatus.Scouting:
                case UnitStatus.Eating:
                case UnitStatus.LockOn:
                    unitStatus = UnitStatus.LockOn;
                    SetLockOnTarget(teamIndex, ref enemyPower, enemyUnitId, enemyTeamIndex);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            return false;
        }

        public void SetLockOnTarget(long teamIndex, ref Power enemyPower, UnitId enemyUnitId, int enemyTeamIndex)
        {
            if(enemyPower.PowerId.Equals(PowerId)) throw new ArgumentException();
            MiscellaneousData[teamIndex] = LockOnUtility.Construct(enemyUnitId, enemyTeamIndex);
            MiscellaneousData2[teamIndex] = enemyPower.PowerId.Value;
            Destinations[teamIndex].Value = enemyPower.Positions[enemyTeamIndex].Value;
        }

    }
}