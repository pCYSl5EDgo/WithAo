using System;
using System.IO.Pipes;
using System.Runtime.CompilerServices;
using AoAndSugi.Game.Models.Unit;
using UniNativeLinq;
using Unity.Burst;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Jobs;

namespace AoAndSugi.Game.Models
{
    [BurstCompile(FloatPrecision.Standard, FloatMode.Deterministic, CompileSynchronously = false, Debug =
#if DEBUG
            true
#else
            false
#endif
    )]
    public unsafe struct OrderJob : IJobParallelFor
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

        private readonly struct PowerEquality : IRefFunc<Order, bool>
        {
            private readonly uint value;

            public PowerEquality(int value) => this.value = (uint)value;

            public bool Calc(ref Order arg0) => arg0.Power.Value == value;
        }

        public void Execute(int powerIndex)
        {
            foreach (ref var order in orders.Where(new PowerEquality(powerIndex)))
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
                    case OrderKind.Prepare:
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
        }

        private void Generate(ref Order order)
        {
            ref var power = ref (*turn)[order.Power];

            if (!power.UnitIds.TryGetFirstIndexOf(out var index, new OrderEquality(order.UnitId)) || index == -1)
            {
                throw new InvalidOperationException();
            }

            power.Statuses[index] = UnitStatus.Generate;
            power.MiscellaneousData[index] = (ulong)order.ForQueenGenerateType;
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