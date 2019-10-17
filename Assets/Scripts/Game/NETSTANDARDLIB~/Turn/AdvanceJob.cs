using System;
using System.Runtime.CompilerServices;
using AoAndSugi.Game.Models.Unit;
using UniNativeLinq;
using Unity.Burst;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Jobs;
using Unity.Mathematics;

namespace AoAndSugi.Game.Models
{
    [BurstCompile(FloatPrecision.Standard, FloatMode.Deterministic, CompileSynchronously = false, Debug =
#if DEBUG
            true
#else
            false
#endif
    )]
    public unsafe struct AdvanceJob : IJobParallelFor
    {
        [NativeDisableUnsafePtrRestriction] private readonly GameMasterData* master;
        [NativeDisableUnsafePtrRestriction] private readonly Turn* turn;

        public AdvanceJob(GameMasterData* master, Turn* turn)
        {
            this.master = master;
            this.turn = turn;
        }

        public void Execute(int index)
        {
            ref var power = ref turn->Powers[index];
            for (var i = 0; i < power.TeamCount; i++)
            {
                ProcessTeams(ref power, i);
            }
        }

        private void ProcessTeams(ref Power power, int i)
        {
            ref var unitStatus = ref power.Statuses[i];
            if (unitStatus != UnitStatus.AdvanceAndRole && unitStatus != UnitStatus.AdvanceAndStop) return;
            var speciesType = power.SpeciesTypes[i];
            var unitType = power.UnitTypes[i];

            ref var unitPosition = ref power.Positions[i];
            ref var cell = ref turn->Board[master->Width, unitPosition.Value];

            var movePower = master->GetMovePower(speciesType, unitType, cell.CellTypeValue, cell.IsTerritoryOf((int)power.Id.Value));

            ref var unitMovePower = ref power.MovePowers[i];
            unitMovePower.Value += movePower.Value;

            var moveCost = master->GetCellMoveCost(cell.CellTypeValue);
            var unitDestination = power.Destinations[i];
            while (unitMovePower.Value >= moveCost.Value)
            {
                unitMovePower.Value -= moveCost.Value;
                var diff = unitDestination.Value - unitPosition.Value;
                AdvanceUnitPositionToDestination(diff, ref unitPosition);
            }
            if (math.any(unitPosition.Value != unitDestination.Value))
                return;
            if (unitStatus != UnitStatus.AdvanceAndRole)
            {
                unitStatus = UnitStatus.Idle;
                return;
            }
            unitStatus = unitType switch
            {
                UnitType.Porter => UnitStatus.Return,
                UnitType.Queen => UnitStatus.Generate,
                UnitType.Soldier => UnitStatus.Scouting,
                UnitType.Worker => UnitStatus.Return,
                _ => throw new ArgumentOutOfRangeException()
            };
        }

        private static void AdvanceUnitPositionToDestination(int2 diff, ref UnitPosition unitPosition)
        {
            if (diff.x == 0)
            {
                if (diff.y == 0) return;
                if (diff.y < 0)
                {
                    // Go Down
                    unitPosition.Value.y--;
                }
                else
                {
                    // Go Up
                    unitPosition.Value.y++;
                }
            }
            else if (diff.y == 0)
            {
                if (diff.x < 0)
                {
                    // Go Left
                    unitPosition.Value.x--;
                }
                else
                {
                    unitPosition.Value.x++;
                }
            }
            else if (((diff.x + diff.y) & 1) == 0)
            {
                if (diff.x < 0)
                {
                    // Go Left
                    unitPosition.Value.x--;
                }
                else
                {
                    // Go Right
                    unitPosition.Value.x++;
                }
            }
            else
            {
                if (diff.y < 0)
                {
                    // Go Down
                    unitPosition.Value.y--;
                }
                else
                {
                    // Go Up
                    unitPosition.Value.y++;
                }
            }
        }
    }
}