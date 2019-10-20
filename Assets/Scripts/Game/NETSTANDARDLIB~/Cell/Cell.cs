#if DEBUG
using System;
#endif
using System.Text;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Mathematics;

namespace AoAndSugi.Game.Models
{
    public unsafe struct Cell
    {
        public CellType CellTypeValue;

        public Cell(CellType cellType)
        {
            this = default;
            CellTypeValue = cellType;
        }

        /// <summary>
        /// bitフラッグ管理
        /// </summary>
        public uint PowerFlags;

        /// <summary>
        /// 各チームの色塗り率
        /// 0~255だが、bitフラッグがonなら1~256 offなら0と計算する
        /// </summary>
        public fixed byte PowerValues[20];

        public bool IsTerritoryOf(int powerId) => ((PowerFlags >> powerId) & 0x1U) != 0;
        public bool IsOtherTerritory(int powerId) => (PowerFlags & ~(1U << powerId)) != 0U;

        public int Sum()
        {
            var answer = math.countbits(PowerFlags);
            for (var i = 0; i < 20; i++)
            {
                answer += PowerValues[i];
            }
            return answer;
        }

        public int this[int powerId]
        {
            get => IsTerritoryOf(powerId) ? PowerValues[powerId] + 1 : 0;
            set
            {
#if DEBUG
                if (value < 0) throw new ArgumentOutOfRangeException(value + " should not be less than 0!");
                else if (value > 256) throw new ArgumentOutOfRangeException(value + " should not be greater than 256!");
#endif
                if (value == 0)
                {
                    PowerValues[powerId] = 0;
                    PowerFlags &= ~(1U << powerId);
                }
                else
                {
                    PowerValues[powerId] = (byte)(value - 1);
                    PowerFlags |= 1U << powerId;
                }
            }
        }

        public bool AddPaint(int powerId, int value)
        {
#if DEBUG
            if (value < 0) throw new ArgumentOutOfRangeException(value + " should not be less than 0!");
#endif
            if (value == 0) return false;
            var territoryCount = math.countbits(PowerFlags);

            if (value >= 256)
            {
                Clear();
                var old = this[powerId];
                if (old == 256) return false;
                this[powerId] = 256;
                return true;
            }

            // tzcnt ２の累乗なら何乗なのかわかる
            switch (territoryCount)
            {
                case 0:
                    this[powerId] = value;
                    return true;
                case 1:
                    var targetTeamId = math.tzcnt(PowerFlags);
                    if (targetTeamId == 64)
                        targetTeamId = 0;
                    if (targetTeamId == powerId)
                    {
                        var old = this[powerId];
                        var renew = old + value;
                        if (renew > 256)
                            renew = 256;
                        if (renew == old) return false;
                        this[powerId] = renew;
                        return true;
                    }
                    else
                    {
                        this[powerId] = value;
                        var sum = this[targetTeamId] + value;
                        if (sum <= 256)
                        {
                            return true;
                        }
                        this[targetTeamId] = 256 - value;
                        return true;
                    }
                default:
                    {
                        var old = this[powerId];
                        if (old + value >= 256)
                        {
                            Clear();
                            if (old == 256) return false;
                            this[powerId] = 256;
                            return true;
                        }
                        var sum = Sum();
                        if (sum + value <= 256)
                        {
                            this[powerId] = old + value;
                        }
                        else
                        {
                            Decrease(exceptId: powerId, exceptIdValue: old + value, overflow: sum + value - 256);
                        }
                        return true;
                    }
            }
        }

        private void Decrease(int exceptId, int exceptIdValue, int overflow)
        {
#if DEBUG
            if (exceptId < 0 || exceptId > 20) throw new ArgumentOutOfRangeException(exceptId.ToString());
            if (exceptIdValue < 0 || exceptIdValue > 256) throw new ArgumentOutOfRangeException(exceptIdValue.ToString());
            if (overflow <= 0) throw new ArgumentOutOfRangeException(overflow.ToString());
#endif
            this[exceptId] = 0;

            do
            {
                var count = math.countbits(PowerFlags);
                var dec = overflow / count;
                var rest = overflow - dec * count;
                for (var i = 0; i < 20; i++)
                {
                    var val = this[i];
                    if (val == 0) continue;
                    if (rest > 0)
                    {
                        rest--;
                        val--;
                    }
                    if (dec <= val)
                    {
                        this[i] = val - dec;
                        continue;
                    }
                    this[i] = 0;
                    rest += dec - val;
                }
                overflow = rest;
            } while (overflow != 0);

            this[exceptId] = exceptIdValue;
        }

        public void Clear()
        {
            fixed (void* ptr = PowerValues)
            {
                UnsafeUtility.MemClear(ptr, 20L);
            }
            PowerFlags = 0;
        }

        public override string ToString()
        {
            var builder = new StringBuilder();
            builder.Append("{\"ChipType\": \"").Append(CellTypeValue.ToString()).Append("\", \"Territories\": [");
            for (var i = 0; i < 20; i++)
            {
                if (IsTerritoryOf(i))
                {
                    builder.Append("\n\t{\"Power\": ").Append(i).Append(", \"Value\": ").Append(this[i]).Append("},");
                }
            }
            builder.Append("]}");
            return builder.ToString();
        }
    }
}