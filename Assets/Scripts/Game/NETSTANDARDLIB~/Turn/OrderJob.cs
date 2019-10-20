using System;
using System.Runtime.CompilerServices;
using AoAndSugi.Game.Models.Unit;
using UniNativeLinq;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Jobs;

namespace AoAndSugi.Game.Models
{
    public unsafe struct OrderJob : IJob
    {
        [NativeDisableUnsafePtrRestriction] private readonly GameMasterData* master;
        [NativeDisableUnsafePtrRestriction] private readonly Turn* turn;
        [NativeDisableUnsafePtrRestriction] private readonly WhereEnumerable<NativeEnumerable<Order>, NativeEnumerable<Order>.Enumerator, Order, TurnIdEquality> orders;

        private readonly struct TurnIdEquality : IRefFunc<Order, bool>
        {
            private readonly uint turnId;

            public TurnIdEquality(TurnId turnId)
            {
                this.turnId = turnId.Value;
            }

            public bool Calc(ref Order arg0) => arg0.TurnId.Value == turnId;
        }

        public OrderJob(GameMasterData* master, Turn* turn, NativeEnumerable<Order> orders)
        {
            this.master = master;
            this.turn = turn;
            this.orders = orders.Where(new TurnIdEquality(turn->TurnId));
        }

        public void Execute()
        {
            var doesNotHaveScout = stackalloc bool[master->MaxTeamCount];
            UnsafeUtility.MemClear(doesNotHaveScout, master->MaxTeamCount);
            foreach (ref var order in orders)
            {
                switch (order.Kind)
                {
                    case OrderKind.AdvanceAndStop:
                        ProcessAdvance(ref order, UnitStatus.AdvanceAndStop);
                        return;
                    case OrderKind.AdvanceAndExecuteJobOfEachType:
                        ProcessAdvance(ref order, UnitStatus.AdvanceAndRole);
                        break;
                    case OrderKind.Generate:
                        Generate(ref order);
                        break;
                    case OrderKind.LockOn:
                        LockOn(ref order, doesNotHaveScout);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
        }

        private void LockOn(ref Order order, bool* doesNotHaveScout)
        {
            var powerValue = order.Power.Value;
            if(doesNotHaveScout[powerValue]) return;
            ref var enemyPower = ref turn->Powers[order.LockOnPower.Value];
            if (enemyPower.TeamCount == 0)
            {
                return;
            }
            var enemyTeamIndex = order.Destination.Value.x;
            if (enemyTeamIndex >= enemyPower.TeamCount)
                enemyTeamIndex = enemyPower.TeamCount - 1;
            var targetId = order.UnitId.Value;
            for (; enemyTeamIndex >= 0; enemyTeamIndex--)
            {
                if (enemyPower.UnitIds[enemyTeamIndex].Value == targetId)
                    break;
            }
            if (enemyTeamIndex == -1)
                return;
            ref var power = ref turn->Powers[powerValue];
            if (!power.Statuses.TryGetFirstIndexOf(out var scoutingIndex, new IsScouting()))
            {
                doesNotHaveScout[powerValue] = true;
                return;
            }
            power.Statuses[scoutingIndex] = UnitStatus.LockOn;
            power.GenerationTurns[scoutingIndex] = turn->TurnId;
            power.SetLockOnTarget(scoutingIndex, ref enemyPower, new UnitId(targetId), enemyTeamIndex);
        }

        private readonly struct IsScouting : IRefFunc<UnitStatus, bool>
        {
            public bool Calc(ref UnitStatus arg0) => arg0 == UnitStatus.Scouting;
        }

        private void Generate(ref Order order)
        {
            ref var power = ref (*turn)[order.Power];

            if (!power.UnitIds.TryGetFirstIndexOf(out var index, new OrderEquality(order.UnitId)) || index == -1)
            {
                throw new InvalidOperationException();
            }

            power.Statuses[index] = UnitStatus.Generate;
            power.MiscellaneousData[index] = (ulong)order.Type | ((ulong)order.TurnId.Value << 32);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void ProcessAdvance(ref Order order, UnitStatus advance)
        {
            ref var power = ref (*turn)[order.Power];

            if (!power.UnitIds.TryGetFirstIndexOf(out var index, new OrderEquality(order.UnitId)) || index == -1)
            {
                throw new InvalidOperationException();
            }

            var speciesType = power.SpeciesTypes[index];
            var unitType = power.UnitTypes[index];
            power.DivideNewUnitFromOriginal((int)index, order.InitialCount, master->GetInitialHp(speciesType, unitType), advance, order.Destination, turn->TurnId);
        }

        private readonly struct OrderEquality : IRefFunc<UnitId, bool>
        {
            private readonly uint unitId;

            public OrderEquality(UnitId unitId)
            {
                this.unitId = unitId.Value;
            }

            public bool Calc(ref UnitId arg0) => arg0.Value == unitId;
        }
    }
}