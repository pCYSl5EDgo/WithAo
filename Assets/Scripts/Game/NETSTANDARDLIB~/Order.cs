using System;
using System.Text;
using AoAndSugi.Game.Models.Unit;

namespace AoAndSugi.Game.Models
{
    public struct Order : IOrder
    {
        public uint Value;
        public TurnId TurnId;
        public UnitId UnitId;
        public UnitInitialCount InitialCount;
        public UnitDestination Destination;

        public override string ToString()
        {
            var buffer = new StringBuilder();
            buffer
                .Append("{\"TurnId\": ")
                .Append(TurnId)
                .Append(", \"InitialCount\":")
                .Append(InitialCount.ToString())
                .Append(", \"Destination\": ")
                .Append(Destination.ToString())
                .Append(", \"Kind\": ")
                .Append(Kind.ToString())
                .Append(", \"Power\": ")
                .Append(Power.ToString())
                .Append(", \"Type\": ")
                .Append(Type.ToString())
                .Append("}");

            return buffer.ToString();
        }

        public UnitInitialCount Count => InitialCount;
        UnitDestination IOrder.Destination => Destination;
        UnitId IOrder.UnitId => UnitId;
        TurnId IOrder.TurnId => TurnId;

        // 2bit 0, 1
        public OrderKind Kind
        {
            get => (OrderKind)(Value & 0x3);
            set
            {
                switch (value)
                {
                    case OrderKind.AdvanceAndStop:
                        Value &= ~3U;
                        break;
                    case OrderKind.AdvanceAndExecuteJobOfEachType:
                        Value |= 1;
                        Value &= ~2U;
                        break;
                    case OrderKind.None:
                        Value |= 2;
                        Value &= ~1U;
                        break;
                    case OrderKind.Prepare:
                        Value |= 3;
                        break;
                    default:
                        throw new ArgumentOutOfRangeException(nameof(value), value, null);
                }
            }
        }

        // 2bit 2, 3
        public PowerId Power
        {
            get => new PowerId((Value >> 2) & 0x3);
            set
            {
                Value &= ~12U;
                Value |= value.Value << 2;
            }
        }

        // 2bit 4, 5
        public UnitType Type
        {
            get { return (UnitType)((Value >> 4) & 0x3); }
            set
            {
                switch (value)
                {
                    case UnitType.Soldier:
                        Value &= ~48U;
                        break;
                    case UnitType.Worker:
                        Value |= 16U;
                        Value &= ~32U;
                        break;
                    case UnitType.Porter:
                        Value |= 32U;
                        Value &= ~16U;
                        break;
                    case UnitType.Queen:
                        Value |= 48U;
                        break;
                    default:
                        throw new ArgumentOutOfRangeException(nameof(value), value, null);
                }
            }
        }
    }

    public enum OrderKind
    {
        AdvanceAndStop,
        AdvanceAndExecuteJobOfEachType,
        None,
        Prepare,
    }
}