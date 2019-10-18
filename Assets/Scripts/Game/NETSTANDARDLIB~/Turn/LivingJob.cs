using Unity.Burst;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Jobs;

namespace AoAndSugi.Game.Models
{
    [BurstCompile(FloatPrecision.Standard, FloatMode.Deterministic, CompileSynchronously = false, Debug =
#if DEBUG
            true
#else
            false
#endif
    )]
    public unsafe struct LivingJob : IJobParallelFor
    {
        [NativeDisableUnsafePtrRestriction] private readonly GameMasterData* master;
        [NativeDisableUnsafePtrRestriction] private readonly Turn* turn;

        public LivingJob(GameMasterData* master, Turn* turn)
        {
            this.master = master;
            this.turn = turn;
        }

        public void Execute(int index)
        {
            ref var power = ref turn->Powers[index];
            for (var i = power.TeamCount; i-- != 0;)
            {
                ProcessTeams(ref power, i, turn->TurnId.Value);
            }
        }

        private void ProcessTeams(ref Power power, int teamIndex, uint turnValue)
        {
            var speciesType = power.SpeciesTypes[teamIndex];
            var unitType = power.UnitTypes[teamIndex];
            var sinceGeneration = turnValue - power.GenerationTurns[teamIndex].Value;
            var interval = master->GetLivingInterval(speciesType, unitType).Value;

            if (sinceGeneration / (interval + 1) * (interval + 1) != sinceGeneration) return;

            var cost = master->GetLivingCost(speciesType, unitType).Value;
            var unitCount = power.CalcUnitCountInTeam(teamIndex, master->GetInitialHp(speciesType, unitType));
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
    }
}