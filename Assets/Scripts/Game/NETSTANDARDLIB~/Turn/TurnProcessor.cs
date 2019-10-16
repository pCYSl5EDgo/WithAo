using System;
using UniNativeLinq;

namespace AoAndSugi.Game.Models
{
    using Unit;
    public struct TurnProcessor
    {
        public void ProcessOrderCollection<TEnumerable, TEnumerator>(ref Turn turn, ref TEnumerable enumerable)
            where TEnumerator : struct, IRefEnumerator<Order>
            where TEnumerable : struct, IRefEnumerable<TEnumerator, Order>
        {
            foreach (ref var order in enumerable)
            {
                if (!turn.Id.Equals(order.TurnId)) continue;
                ProcessEachOrder(ref turn, ref order);
            }
        }

        private static void ProcessEachOrder(ref Turn turn, ref Order order)
        {
            switch (order.Kind)
            {
                case OrderKind.AdvanceAndStop:
                    ProcessAdvanceAndStopOrder(ref turn, ref order);
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

        private static void ProcessAdvanceAndStopOrder(ref Turn turn, ref Order order)
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

        private static void ProcessAdvanceAndExecuteJobOrder(ref Turn turn, ref Order order)
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