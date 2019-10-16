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
        public uint TeamFlags;

        /// <summary>
        /// 各チームの色塗り率
        /// 0~255だが、bitフラッグがonなら1~256 offなら0と計算する
        /// </summary>
        public fixed byte TeamValues[20];

        public bool IsTerritoryOf(int teamId) => ((TeamFlags >> teamId) & 0x1U) != 0;

        public int Sum()
        {
            var answer = math.countbits(TeamFlags);
            for (var i = 0; i < 20; i++)
            {
                answer += TeamValues[i];
            }
            return answer;
        }

        public int this[int teamId]
        {
            get => IsTerritoryOf(teamId) ? TeamValues[teamId] + 1 : 0;
            set
            {
                #if DEBUG
                if (value < 0) throw new ArgumentOutOfRangeException(value + " should not be less than 0!");
                else if (value > 256) throw new ArgumentOutOfRangeException(value + " should not be greater than 256!");
                #endif
                if (value == 0)
                {
                    TeamValues[teamId] = 0;
                    TeamFlags &= ~(1U << teamId);
                }
                else
                {
                    TeamValues[teamId] = (byte) (value - 1);
                    TeamFlags |= 1U << teamId;
                }
            }
        }

        public bool AddPaint(int teamId, int value)
        {
            #if DEBUG
            if (value < 0) throw new ArgumentOutOfRangeException(value + " should not be less than 0!");
            #endif
            if (value == 0) return false;
            var territoryCount = math.countbits(TeamFlags);

            if (value >= 256)
            {
                Clear();
                var old = this[teamId];
                if (old == 256) return false;
                this[teamId] = 256;
                return true;
            }

            // tzcnt ２の累乗なら何乗なのかわかる
            switch (territoryCount)
            {
            case 0:
                this[teamId] = value;
                return true;
            case 1:
                var targetTeamId = math.tzcnt(TeamFlags);
                if (targetTeamId == 64)
                    targetTeamId = 0;
                if (targetTeamId == teamId)
                {
                    var old = this[teamId];
                    var renew = old + value;
                    if (renew > 256)
                        renew = 256;
                    if (renew == old) return false;
                    this[teamId] = renew;
                    return true;
                }
                else
                {
                    this[teamId] = value;
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
                    var old = this[teamId];
                    if (old + value >= 256)
                    {
                        Clear();
                        if (old == 256) return false;
                        this[teamId] = 256;
                        return true;
                    }
                    var sum = Sum();
                    if (sum + value <= 256)
                    {
                        this[teamId] = old + value;
                    }
                    else
                    {
                        Decrease(exceptId: teamId, exceptIdValue: old + value, overflow: sum + value - 256);
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
                var count = math.countbits(TeamFlags);
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
            fixed (void* ptr = TeamValues)
            {
                UnsafeUtility.MemClear(ptr, 20L);
            }
            TeamFlags = 0;
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

    public enum CellType
    {
        None,
    }
}