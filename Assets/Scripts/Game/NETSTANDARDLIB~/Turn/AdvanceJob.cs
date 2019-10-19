using System;
using UniNativeLinq;
using AoAndSugi.Game.Models.Unit;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Jobs;
using Unity.Mathematics;

namespace AoAndSugi.Game.Models
{
    public unsafe struct AdvanceJob : IJob
    {
        [NativeDisableUnsafePtrRestriction] private readonly GameMasterData* master;
        [NativeDisableUnsafePtrRestriction] private readonly Turn* turn;

        public AdvanceJob(GameMasterData* master, Turn* turn)
        {
            this.master = master;
            this.turn = turn;
        }

        public void Execute()
        {
            foreach (ref var power in turn->Powers)
            {
                for (var i = 0; i < power.TeamCount; i++)
                {
                    ProcessTeams(ref power, i, turn->TurnId.Value);
                }
            }
        }

        private void ProcessTeams(ref Power power, int teamIndex, uint turnValue)
        {
            ref var unitStatus = ref power.Statuses[teamIndex];
            if (unitStatus != UnitStatus.AdvanceAndRole && unitStatus != UnitStatus.AdvanceAndStop) return;
            var speciesType = power.SpeciesTypes[teamIndex];
            var unitType = power.UnitTypes[teamIndex];

            ref var unitPosition = ref power.Positions[teamIndex];
            ref var cell = ref turn->Board[master->Width, unitPosition.Value];

            ref var unitMovePower = ref power.MovePowers[teamIndex];

            {
                var movePower = master->GetMovePower(speciesType, unitType, cell.CellTypeValue, cell.IsTerritoryOf((int)power.PowerId.Value));
                unitMovePower.Value += movePower.Value;
            }

            var unitDestination = power.Destinations[teamIndex];
            while (true)
            {
                var moveCost = master->GetCellMoveCost(cell.CellTypeValue).Value;
                if (unitMovePower.Value < moveCost) break;
                unitMovePower.Value -= moveCost;
                var diff = unitDestination.Value - unitPosition.Value;
                AdvanceUnitPositionToDestination(diff, ref unitPosition);
                cell = ref turn->Board[master->Width, unitPosition.Value];
            }
            if (math.any(unitPosition.Value != unitDestination.Value))
                return;

            WhenReachingTheDestination(ref power, teamIndex, unitType, unitDestination.Value);
        }

        private readonly struct SamePosition : IRefFunc<EnergySupplier, bool>
        {
            private readonly int2 position;

            public SamePosition(int2 position)
            {
                this.position = position;
            }

            public bool Calc(ref EnergySupplier arg0) => arg0.Position.Equals(position);
        }

        private void WhenReachingTheDestination(ref Power power, int teamIndex, UnitType unitType, int2 position)
        {
            power.GenerationTurns[teamIndex] = turn->TurnId;
            ref UnitStatus unitStatus = ref power.Statuses[teamIndex];
            if (turn->EnergySuppliers.TryGetFirstIndexOf(out var supplierIndex, new SamePosition(position)))
            {
                unitStatus = UnitStatus.Eating;
                power.MiscellaneousData[teamIndex] = (ulong)supplierIndex;
                return;
            }
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