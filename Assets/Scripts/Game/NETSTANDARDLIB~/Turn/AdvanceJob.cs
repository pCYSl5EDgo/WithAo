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
            var hasNoticedPower = stackalloc bool[master->MaxPowerCount];
            foreach (ref var power in turn->Powers)
            {
                UnsafeUtility.MemClear(hasNoticedPower, master->MaxPowerCount);
                for (var teamIndex = power.TeamCount - 1; teamIndex >= 0; teamIndex--)
                {
                    ProcessTeams(ref power, teamIndex, hasNoticedPower);
                }
                for (var i = 0; i < master->MaxPowerCount; i++)
                {
                    if (!hasNoticedPower[i]) continue;
                    turn->Powers[i].SetKnowEnemy(power.PowerId, true);
                }
            }
        }

        private void ProcessTeams(ref Power power, int teamIndex, bool* hasNoticedPower)
        {
            ref var unitStatus = ref power.Statuses[teamIndex];
            if (IsNotAdvanceState(unitStatus)) return;
            var speciesType = power.SpeciesTypes[teamIndex];
            var unitType = power.UnitTypes[teamIndex];
            var powerId = (int)power.PowerId.Value;

            ref var unitPosition = ref power.Positions[teamIndex];
            ref var board = ref turn->Board;
            ref var cell = ref board[master->Width, unitPosition.Value];

            ref var unitMovePower = ref power.MovePowers[teamIndex];

            {
                var movePower = master->GetMovePower(speciesType, unitType, cell.CellTypeValue, cell.IsMainTerritoryOwner((int)power.PowerId.Value));
                unitMovePower.Value += movePower.Value;
            }

            var unitDestination = power.Destinations[teamIndex];
            var hasNoticed = stackalloc bool[master->MaxPowerCount];
            UnsafeUtility.MemClear(hasNoticed, master->MaxPowerCount);
            while (true)
            {
                var moveCost = master->GetCellMoveCost(cell.CellTypeValue).Value;
                if (unitMovePower.Value < moveCost) break;

                unitMovePower.Value -= moveCost;
                var diff = unitDestination.Value - unitPosition.Value;
                var isReached = AdvanceUnitPositionToDestination(diff, ref unitPosition);
                Notice(ref power, teamIndex, ref (cell = ref board[master->Width, unitPosition.Value]), powerId, unitType, hasNoticed, hasNoticedPower);
                if (isReached) break;
            }

            if (!unitPosition.Value.Equals(unitDestination.Value))
                return;

            WhenReachingTheDestination(ref power, teamIndex, unitType, unitDestination.Value);
        }

        private static bool IsNotAdvanceState(UnitStatus unitStatus)
        {
            return unitStatus != UnitStatus.AdvanceAndRole
                && unitStatus != UnitStatus.AdvanceAndStop
                && unitStatus != UnitStatus.LockOn
                && unitStatus != UnitStatus.Battle;
        }

        private void Notice(ref Power power, int teamIndex, ref Cell cell, int powerId, UnitType unitType, bool* hasNoticed, bool* hasNoticedOfPower)
        {
            if (!cell.IsOtherTerritory(powerId)) return;
            var turnId = new TurnId(turn->TurnId.Value + 1);
            for (var powerIndex = 0; powerIndex < master->MaxPowerCount; powerIndex++)
            {
                if (powerIndex == powerId) continue;
                if (!cell.IsTerritoryOf(powerIndex)) continue;
                if (hasNoticed[powerIndex]) continue;
                hasNoticed[powerIndex] = true;
                hasNoticedOfPower[powerIndex] = true;
                if (power.PowerId.Value == powerIndex) throw new InvalidOperationException();
                turn->AddOrder(new Order
                {
                    Destination = new UnitDestination(new int2(teamIndex, 0)),
                    Kind = OrderKind.LockOn,
                    Power = new PowerId((uint)powerIndex),
                    TurnId = turnId,
                    LockOnPower = power.PowerId,
                    UnitId = power.UnitIds[teamIndex],
                    Type = unitType,
                });
            }
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
            ref var unitStatus = ref power.Statuses[teamIndex];
            if (unitStatus == UnitStatus.LockOn) return;

            if (WhenReachEnergySupplier(ref power, teamIndex, position)) return;
            if (IfStatusIsAdvanceAndStop(ref power, teamIndex, unitStatus)) return;
            if (unitStatus == UnitStatus.Battle) return;
            switch (unitType)
            {
                case UnitType.Queen:
                    unitStatus = UnitStatus.Generate;
                    break;
                case UnitType.Soldier:
                    unitStatus = UnitStatus.Scouting;
                    break;
                case UnitType.Porter:
                case UnitType.Worker:
                    if (power.UnitTypes.TryGetFirstIndexOf(out var queenIndex, new IsQueen()))
                    {
                        unitStatus = UnitStatus.AdvanceAndStop;
                        power.Destinations[teamIndex].Value = power.Positions[queenIndex].Value;
                    }
                    else
                    {
                        power.SetStatusIdle(teamIndex, turn->TurnId);
                    }
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private bool IfStatusIsAdvanceAndStop(ref Power power, int teamIndex, UnitStatus unitStatus)
        {
            if (unitStatus != UnitStatus.AdvanceAndStop) return false;
            power.SetStatusIdle(teamIndex, turn->TurnId);
            for (var i = 0; i < power.TeamCount; i++)
            {
                if (i == teamIndex || power.UnitTypes[i] != UnitType.Queen) continue;
                TransferHp(ref power, teamIndex, i);
                break;
            }
            return true;
        }

        private void TransferHp(ref Power power, int source, int destination)
        {
            var initialSourceHp = (int)(master->GetInitialHp(power.SpeciesTypes[source], power.UnitTypes[source]).Value * power.InitialCounts[source].Value);
            ref var hp = ref power.TotalHps[source].Value;
            if (initialSourceHp >= hp) return;
            power.TotalHps[destination].Value += hp - initialSourceHp;
            hp = initialSourceHp;
        }

        private bool WhenReachEnergySupplier(ref Power power, int teamIndex, int2 position)
        {
            if (!turn->EnergySuppliers.TryGetFirstIndexOf(out var supplierIndex, new SamePosition(position))) return false;
            power.Statuses[teamIndex] = UnitStatus.Eating;
            power.MiscellaneousData[teamIndex] = (ulong)supplierIndex;
            return true;
        }

        private readonly struct IsQueen : IRefFunc<UnitType, bool>
        {
            public bool Calc(ref UnitType arg0) => arg0 == UnitType.Queen;
        }

        private static bool AdvanceUnitPositionToDestination(int2 diff, ref UnitPosition unitPosition)
        {
            if (diff.x == 0)
            {
                if (diff.y == 0) return true;
                if (diff.y < 0)
                {
                    // Go Down
                    unitPosition.Value.y--;
                    return diff.y == -1;
                }
                else
                {
                    // Go Up
                    unitPosition.Value.y++;
                    return diff.y == 1;
                }
            }
            else if (diff.y == 0)
            {
                if (diff.x < 0)
                {
                    // Go Left
                    unitPosition.Value.x--;
                    return diff.x == -1;
                }
                else
                {
                    unitPosition.Value.x++;
                    return diff.x == 1;
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
            return false;
        }
    }
}