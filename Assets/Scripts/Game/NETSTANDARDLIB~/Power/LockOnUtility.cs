namespace AoAndSugi.Game.Models
{
    public static class LockOnUtility
    {
        public static void SetLockOnTarget(ref this Power power, long teamIndex, ref Power enemyPower, uint enemyUnitId, int enemyTeamIndex)
        {
            power.MiscellaneousData[teamIndex] = ((ulong)enemyUnitId << 32) | (uint)enemyTeamIndex;
            power.MiscellaneousData2[teamIndex] = enemyPower.PowerId.Value;
            power.Destinations[teamIndex].Value = enemyPower.Positions[enemyTeamIndex].Value;
        }
    }
}