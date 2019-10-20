using System;
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
                    case OrderKind.Generate:
                        Value |= 2;
                        Value &= ~1U;
                        break;
                    case OrderKind.LockOn:
                        Value |= 3;
                        break;
                    default:
                        throw new ArgumentOutOfRangeException(nameof(value), value, null);
                }
            }
        }

        // 2bit 2, 3
        public UnitType Type
        {
            get => (UnitType)((Value >> 4) & 0x3);
            set
            {
                switch (value)
                {
                    case UnitType.Soldier:
                        Value &= ~12U;
                        break;
                    case UnitType.Worker:
                        Value |= 4U;
                        Value &= ~8U;
                        break;
                    case UnitType.Porter:
                        Value |= 8U;
                        Value &= ~4U;
                        break;
                    case UnitType.Queen:
                        Value |= 12U;
                        break;
                    default:
                        throw new ArgumentOutOfRangeException(nameof(value), value, null);
                }
            }
        }
        // 5bit 4,  5,  6,  7,  8
        public PowerId Power
        {
            get => new PowerId((Value >> 4) & 0b1111_1U);
            set
            {
                Value &= ~0b1111_1_0000U;
                Value |= (value.Value & 0b1111_1U) << 4;
            }
        }

        // 5bit 9, 10, 11, 12, 13
        public PowerId LockOnPower
        {
            get => new PowerId((Value >> 9) & 0b1111_1U);
            set
            {
                Value &= ~(0b1111_1U << 9);
                Value |= (value.Value & 0b1111_1U) << 9;
            }
        }
    }

    public enum OrderKind
    {
        AdvanceAndStop,
        AdvanceAndExecuteJobOfEachType,
        Generate,
        LockOn,
    }
}