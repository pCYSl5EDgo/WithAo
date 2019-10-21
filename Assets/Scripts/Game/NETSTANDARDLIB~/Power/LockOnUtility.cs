using AoAndSugi.Game.Models.Unit;

namespace AoAndSugi.Game.Models
{
    public static class LockOnUtility
    {
        public static void Deconstruct(this ulong miscellaneous, out UnitId enemyUnitId, out int enemyTeamIndex)
        {
            enemyTeamIndex = (int)(uint)miscellaneous;
            enemyUnitId = new UnitId((uint)(miscellaneous >> 32));
        }

        public static ulong Construct(UnitId enemyUnitId, int enemyTeamIndex) => (uint)enemyTeamIndex | ((ulong)enemyUnitId.Value << 32);
    }
}