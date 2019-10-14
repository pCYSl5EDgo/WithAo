using System;
using AoAndSugi.Game.Models.Unit;

namespace AoAndSugi.Game.Models
{
    public struct Power
    {
        public int TeamCount;
        public UnitCommonInfo[] CommonInfos;
        public UnitType[] UnitTypes;
        public UnitInitialCount[] InitialCounts;
        public UnitTotalHp[] TotalHps;
        public UnitStatus[] Statuses;
        public UnitPosition[] Positions;

        public void RemoveAtSwapBack(int index)
        {
            if(index < 0 || index >= TeamCount) throw new ArgumentOutOfRangeException(index + " should be less than " + TeamCount);
            TeamCount--;

            void SwapBack<T>(T[] array, int i, int end) where T : unmanaged
            {
                array[i] = array[end];
            }
            SwapBack(CommonInfos, index, TeamCount);
            SwapBack(UnitTypes, index, TeamCount);
            SwapBack(InitialCounts, index, TeamCount);
            SwapBack(TotalHps, index, TeamCount);
            SwapBack(Statuses, index, TeamCount);
            SwapBack(Positions, index, TeamCount);
        }
    }
}