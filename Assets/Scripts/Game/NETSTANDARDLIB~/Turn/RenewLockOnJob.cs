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
                for (var teamIndex = 0; teamIndex < power.TeamCount; teamIndex++)
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
                Debug.Log("ZZZ");
                power.SetStatusRole(teamIndex);
                return;
            }
            var targetId = (uint)(datum >> 32);
            var targetIndex = (uint)datum;
            if (targetIndex >= targetPower.TeamCount)
                targetIndex = (uint)targetPower.TeamCount - 1;
            while (true)
            {
                if (targetPower.UnitIds[targetIndex].Value == targetId)
                    break;
                if (targetIndex == 0)
                {
                    power.SetStatusRole(teamIndex);
                    Debug.Log("YYY");
                    return;
                }
                targetIndex--;
            }
            var targetPosition = targetPower.Positions[targetIndex].Value;

            var speciesType = power.SpeciesTypes[teamIndex];
            var unitType = power.UnitTypes[teamIndex];

            var isInAttackRange = math.csum(math.abs(targetPosition - power.Positions[teamIndex].Value)) <= master->GetAttackRange(speciesType, unitType).Value;
            if (isInAttackRange)
            {
                unitStatus = UnitStatus.Battle;
            }
            Debug.Log("XXX");
            power.SetLockOnTarget(teamIndex, ref targetPower, targetId, (int)targetIndex);
        }
    }
}