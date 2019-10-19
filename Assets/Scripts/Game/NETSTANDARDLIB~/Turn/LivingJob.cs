using Unity.Collections.LowLevel.Unsafe;
using Unity.Jobs;

namespace AoAndSugi.Game.Models
{
    public unsafe struct LivingJob : IJob
    {
        [NativeDisableUnsafePtrRestriction] private readonly GameMasterData* master;
        [NativeDisableUnsafePtrRestriction] private readonly Turn* turn;

        public LivingJob(GameMasterData* master, Turn* turn)
        {
            this.master = master;
            this.turn = turn;
        }

        private void ProcessTeams(ref Power power, int teamIndex, uint turnValue)
        {
            var speciesType = power.SpeciesTypes[teamIndex];
            var unitType = power.UnitTypes[teamIndex];
            var sinceGeneration = turnValue - power.GenerationTurns[teamIndex].Value;
            var interval = master->GetLivingInterval(speciesType, unitType).Value;
            if (sinceGeneration / (interval + 1) * (interval + 1) != sinceGeneration) return;

            var cost = master->GetLivingCost(speciesType, unitType).Value;
            var unitInitialHp = master->GetInitialHp(speciesType, unitType);
            var unitCount = power.CalcUnitCountInTeam(teamIndex, unitInitialHp);
            ref var totalHp = ref power.TotalHps[teamIndex].Value;
#if DEBUG
            checked
            {
#endif
            power.TotalHps[teamIndex].Value -= (int)(cost * unitCount);
#if DEBUG
            }
#endif
            if (totalHp > 0) return;

            power.RemoveAtSwapBack(teamIndex);
        }

        public void Execute()
        {
            foreach (ref var power in turn->Powers)
            {
                for (var i = 0; i < power.TeamCount; i++)
                {
                    ProcessTeams(ref power, i, turn->TurnId.Value);
                }
            }
        }
    }
}