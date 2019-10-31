using System;
using AoAndSugi.Game.Models.Unit;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Jobs;

namespace AoAndSugi.Game.Models
{
    public unsafe struct BattleJob : IJob
    {
        [NativeDisableUnsafePtrRestriction] private readonly GameMasterData* master;
        [NativeDisableUnsafePtrRestriction] private readonly Turn* turn;

        public BattleJob(GameMasterData* master, Turn* turn)
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
            if (unitStatus != UnitStatus.Battle) return;
            var powerIndex = (uint)power.MiscellaneousData2[teamIndex];
            if (powerIndex == power.PowerId.Value)
                throw new ArgumentException("Don't attack ally!");
            ref var enemyPower = ref turn->Powers[powerIndex];
            ref var datum = ref power.MiscellaneousData[teamIndex];
            if (GuardBlockIsEnemyAlive(ref power, teamIndex, ref datum, ref enemyPower, out var enemyTeamIndex) ||
                GuardBlockIsAttackTiming(ref power, teamIndex, out var attackerSpeciesType, out var attackerUnitType))
                return;

            var teamCount = power.CalcUnitCountInTeam(teamIndex, master->GetInitialHp(attackerSpeciesType, attackerUnitType));
            var attackPoint = master->GetAttackPoint(attackerSpeciesType, attackerUnitType);
            var totalDamage = teamCount * attackPoint.Value;

            var isEnemyDead = enemyPower.SetDamage(enemyTeamIndex, ref power, power.UnitIds[teamIndex], teamIndex, (int)totalDamage);
            if (isEnemyDead)
            {
                power.SetStatusRole(teamIndex);
            }

            var totalCost = teamCount * master->GetAttackCost(attackerSpeciesType, attackerUnitType).Value;
            ref var hp = ref power.TotalHps[teamIndex].Value;
            hp -= (int)totalCost;
            if (hp <= 0)
            {
                power.RemoveAtSwapBack(teamIndex);
            }
        }

        private static bool GuardBlockIsEnemyAlive(ref Power power, int teamIndex, ref ulong datum, ref Power enemyPower, out int enemyTeamIndex)
        {
            if (enemyPower.TeamCount == 0)
            {
                power.SetStatusRole(teamIndex);
                enemyTeamIndex = default;
                return true;
            }
            UnitId enemyUnitId;
            (enemyUnitId, enemyTeamIndex) = datum;
            if (enemyTeamIndex >= enemyPower.TeamCount)
                enemyTeamIndex = enemyPower.TeamCount - 1;
            for (; enemyTeamIndex >= 0; enemyTeamIndex--)
            {
                if (enemyUnitId.Value != enemyPower.UnitIds[enemyTeamIndex].Value) continue;
                datum = LockOnUtility.Construct(enemyUnitId, enemyTeamIndex);
                return false;
            }
            power.SetStatusRole(teamIndex);
            return true;
        }

        private bool GuardBlockIsAttackTiming(ref Power power, int teamIndex, out SpeciesType attackerSpeciesType, out UnitType attackerUnitType)
        {
            attackerSpeciesType = power.SpeciesTypes[teamIndex];
            attackerUnitType = power.UnitTypes[teamIndex];
            var interval = master->GetAttackInterval(attackerSpeciesType, attackerUnitType).Value + 1;
            var sinceAttack = turn->TurnId.Value - power.GenerationTurns[teamIndex].Value;
            return sinceAttack / interval * interval != sinceAttack;
        }
    }
}