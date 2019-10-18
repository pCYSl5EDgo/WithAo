using System;
using AoAndSugi.Game.Models.Unit;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Jobs;
using Unity.Mathematics;

namespace AoAndSugi.Game.Models
{
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

            ref var unitMovePower = ref power.MovePowers[i];
            
            {
                var movePower = master->GetMovePower(speciesType, unitType, cell.CellTypeValue, cell.IsTerritoryOf((int) power.PowerId.Value));
                unitMovePower.Value += movePower.Value;
            }

            var unitDestination = power.Destinations[i];
            while (true)
            {
                var moveCost = master->GetCellMoveCost(cell.CellTypeValue).Value;
                if(unitMovePower.Value < moveCost) break;
                unitMovePower.Value -= moveCost;
                var diff = unitDestination.Value - unitPosition.Value;
                AdvanceUnitPositionToDestination(diff, ref unitPosition);
                cell = ref turn->Board[master->Width, unitPosition.Value];
            }
            if (math.any(unitPosition.Value != unitDestination.Value))
                return;

            WhenReachingTheDestination(ref unitStatus, unitType);
            power.GenerationTurns[i] = turn->TurnId;
        }

        private static void WhenReachingTheDestination(ref UnitStatus unitStatus, UnitType unitType)
        {
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
            else if ((math.csum(diff) & 1) == 0)
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