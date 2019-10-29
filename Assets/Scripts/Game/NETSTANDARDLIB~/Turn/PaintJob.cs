using Unity.Burst;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Jobs;

namespace AoAndSugi.Game.Models
{
    [BurstCompile]
    public unsafe struct PaintJob : IJob
    {
        [NativeDisableUnsafePtrRestriction] private readonly GameMasterData* master;
        [NativeDisableUnsafePtrRestriction] private readonly Turn* turn;

        public PaintJob(GameMasterData* master, Turn* turn)
        {
            this.master = master;
            this.turn = turn;
        }

        private void ProcessTeams(ref Power power, int teamIndex, uint turnValue)
        {
            ref var currentCell = ref turn->Board[master->Width, power.Positions[teamIndex].Value];
            var powerIdValue = (int)power.PowerId.Value;

            if (currentCell[powerIdValue] == 256) return;

            var speciesType = power.SpeciesTypes[teamIndex];
            var unitType = power.UnitTypes[teamIndex];

            var sincePaint = turnValue - power.GenerationTurns[teamIndex].Value;
            var paintInterval = master->GetPaintInterval(speciesType, unitType).Value + 1u;
            if (sincePaint / paintInterval * paintInterval != sincePaint) return;

            var paintCost = master->GetPaintCost(speciesType, unitType).Value;
            var painterCount = power.CalcUnitCountInTeam(teamIndex, master->GetInitialHp(speciesType, unitType));

            ref var totalHp = ref power.TotalHps[teamIndex].Value;
            totalHp -= paintCost * (int)painterCount;

            currentCell.AddPaint(powerIdValue, (int)painterCount * master->GetPaintPoint(speciesType, unitType).Value);

            if (totalHp > 0) return;

            power.RemoveAtSwapBack(teamIndex);
        }

        public void Execute()
        {
            foreach (ref var power in turn->Powers)
            {
                for (var i = power.TeamCount - 1; i >= 0; i--)
                {
                    ProcessTeams(ref power, i, turn->TurnId.Value);
                }
            }
        }
    }
}