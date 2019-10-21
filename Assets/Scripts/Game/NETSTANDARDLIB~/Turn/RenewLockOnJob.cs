using AoAndSugi.Game.Models.Unit;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;

namespace AoAndSugi.Game.Models
{
    public unsafe struct RenewLockOnJob : IJob
    {
        [NativeDisableUnsafePtrRestriction] private readonly GameMasterData* master;
        [NativeDisableUnsafePtrRestriction] private readonly Turn* turn;

        public RenewLockOnJob(GameMasterData* master, Turn* turn)
        {
            this.master = master;
            this.turn = turn;
        }

        public void Execute()
        {
            foreach (ref var power in turn->Powers)
            {
                for (var teamIndex = power.TeamCount - 1; teamIndex >= 0; teamIndex--)
                {
                    ProcessTeams(ref power, teamIndex);
                }
            }
        }

        private void ProcessTeams(ref Power power, int teamIndex)
        {
            ref var unitStatus = ref power.Statuses[teamIndex];
            if (unitStatus != UnitStatus.LockOn && unitStatus != UnitStatus.Battle) return;
            ref var datum = ref power.MiscellaneousData[teamIndex];
            ref var datum2 = ref power.MiscellaneousData2[teamIndex];
            ref var targetPower = ref turn->Powers[(uint)datum2];
            if (targetPower.TeamCount == 0)
            {
                power.SetStatusRole(teamIndex);
                return;
            }
            var (targetId, targetIndex) = datum;
            if (targetIndex >= targetPower.TeamCount)
                targetIndex = targetPower.TeamCount - 1;
            while (true)
            {
                if (targetPower.UnitIds[targetIndex].Value == targetId.Value)
                    break;
                if (targetIndex == 0)
                {
                    power.SetStatusRole(teamIndex);
                    return;
                }
                targetIndex--;
            }
            var targetPosition = targetPower.Positions[targetIndex].Value;

            var speciesType = power.SpeciesTypes[teamIndex];
            var unitType = power.UnitTypes[teamIndex];

            var manhattanDistance = math.csum(math.abs(targetPosition - power.Positions[teamIndex].Value));
            var attackRange = master->GetAttackRange(speciesType, unitType).Value;
            var isInAttackRange = manhattanDistance <= attackRange;
            if (isInAttackRange)
            {
                unitStatus = UnitStatus.Battle;
            }
            power.SetLockOnTarget(teamIndex, ref targetPower, targetId, targetIndex);
        }
    }
}