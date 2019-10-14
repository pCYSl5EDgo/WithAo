using System.Runtime.InteropServices;
using System.Security.Cryptography.X509Certificates;
using UniNativeLinq;
using Unity.Mathematics;

namespace AoAndSugi.Game.Models
{
    public unsafe struct Cell
    {
        public CellType CellTypeValue;

        /// <summary>
        /// bitフラッグ管理
        /// </summary>
        public uint TeamFlags;

        /// <summary>
        /// 各チームの色塗り率
        /// 0~255だが、bitフラッグがonなら1~256 offなら0と計算する
        /// </summary>
        public fixed byte TeamValues[20];

        public bool IsTerritoryOf(int teamId)
        {
            return ((TeamFlags >> teamId) & 0x1U) == 0x1U;
        }

        private void SetTerritoryOf(int teamId)
        {
            TeamFlags |= 1U << teamId;
        }

        public int Paint(int teamId, int value)
        {
            if (value <= 0) return 0;
            var territoryCount = math.lzcnt(TeamFlags);
            var sum = territoryCount;
            for (var i = 0; i < 20; i++)
            {
                sum += TeamValues[i];
            }
            var total = sum + value;
            if (total > 256)
            {
                if (IsTerritoryOf(teamId))
                {

                }
                else
                {

                }
            }
            else
            {
                if (IsTerritoryOf(teamId))
                {
                    TeamValues[teamId] = (byte)(TeamValues[teamId] + value);
                }
                else
                {
                    SetTerritoryOf(teamId);
                    TeamValues[teamId] = (byte)(value - 1);
                }
            }
        }
    }

    public enum CellType
    {
        None,
    }
}