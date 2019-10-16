using System;
using UniNativeLinq;

namespace AoAndSugi.Game.Models
{
    using Unit;
    public struct TurnProcessor
    {
        public void ProcessOrderCollection<TEnumerable, TEnumerator>(ref GameMasterData master, ref Turn turn, ref TEnumerable enumerable)
            where TEnumerator : struct, IRefEnumerator<Order>
            where TEnumerable : struct, IRefEnumerable<TEnumerator, Order>
        {
            foreach (ref var order in new WhereEnumerable<TEnumerable, TEnumerator, Order, TurnIdEquality>(enumerable, new TurnIdEquality(turn.Id)))
            {
                ProcessEachOrder(ref master, ref turn, ref order);
            }
        }

        private readonly struct TurnIdEquality : IRefFunc<Order, bool>
        {
            private readonly uint turnId;

            public TurnIdEquality(TurnId turnId)
            {
                this.turnId = turnId.Value;
            }

            public bool Calc(ref Order arg0) => arg0.TurnId.Value == turnId;
        }

        private static void ProcessEachOrder(ref GameMasterData master, ref Turn turn, ref Order order)
        {
            switch (order.Kind)
            {
                case OrderKind.AdvanceAndStop:
                    ProcessAdvanceAndStopOrder(ref master, ref turn, ref order);
                    return;
                case OrderKind.AdvanceAndExecuteJobOfEachType:
                    break;
                case OrderKind.AdvanceSearchAndDestroy:
                    break;
                case OrderKind.Generate:
                    break;
                default:
                    throw new ArgumentOutOfRangeException(order.ToString());
            }
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

        private static void ProcessAdvanceAndStopOrder(ref GameMasterData master, ref Turn turn, ref Order order)
        {
            ref var power = ref turn[order.Power];

            power.UnitIds.TryGetFirstIndexOf(out var index, new OrderEquality(order.UnitId));

            if (index == -1) throw new InvalidOperationException(order.ToString());

            
        }

        private static void ProcessAdvanceAndExecuteJobOrder(ref GameMasterData master, ref Turn turn, ref Order order)
        {
            ref var power = ref turn[order.Power];

            var index = -1;
            for (var i = 0; i < power.TeamCount; i++)
            {
                if (!power.UnitIds[i].Equals(order.UnitId)) continue;
                index = i;
                break;
            }
            if (index == -1) throw new InvalidOperationException(order.ToString());


        }
    }
}