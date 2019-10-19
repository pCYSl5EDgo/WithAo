using System;
using AoAndSugi.Game.Models.Unit;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Jobs;

namespace AoAndSugi.Game.Models
{
    public unsafe struct EatingJob : IJob
    {
        [NativeDisableUnsafePtrRestriction] private readonly GameMasterData* master;
        [NativeDisableUnsafePtrRestriction] private readonly Turn* turn;

        public EatingJob(GameMasterData* master, Turn* turn)
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
            CleanUpEnergySuppliers();
        }

        private void CleanUpEnergySuppliers()
        {
            ref var suppliers = ref turn->EnergySuppliers;
            var length = suppliers.Length;
            for (var i = length - 1; i >= 0; i--)
            {
                if (suppliers[i].Value > 0) continue;
                length--;
                suppliers[i] = suppliers[length];
            }
            suppliers = suppliers.Take(length);
        }

        private void ProcessTeams(ref Power power, int teamIndex)
        {
            ref var status = ref power.Statuses[teamIndex];
            if (status != UnitStatus.Eating) return;

            ref var datum = ref power.MiscellaneousData[teamIndex];
            var supplierIndex = (long)datum;
            var unitPosition = power.Positions[teamIndex].Value;
            ref var energySuppliers = ref turn->EnergySuppliers;
            ref var energySupplier = ref energySuppliers[supplierIndex];
            for (; supplierIndex >= 0; energySupplier = ref energySuppliers[--supplierIndex])
            {
                if (energySupplier.Position.Equals(unitPosition))
                    break;
            }
            if (supplierIndex == -1 || energySupplier.Value <= 0)
            {
                status = UnitStatus.Idle;
                datum = default;
                return;
            }

            var speciesType = power.SpeciesTypes[teamIndex];
            var unitType = power.UnitTypes[teamIndex];

            var maxHpEach = master->GetMaxHp(speciesType, unitType).Value;
            var count = power.InitialCounts[teamIndex].Value;
            var maxHp = maxHpEach * count;
            ref var hp = ref power.TotalHps[teamIndex].Value;
            var diff = maxHp - hp;
            if (diff < 0) throw new InvalidOperationException("count : " + count + ", maxHpEach" + maxHpEach + ", hp : " + hp);
            if (diff == 0)
            {
                status = UnitStatus.Idle;
                return;
            }
            var eatingNeeds = (int)((ulong)(diff + 1) >> 1);

            var actuallyEaten = energySupplier.Provide(eatingNeeds);
            hp += actuallyEaten;

            if (hp == maxHp)
            {
                status = UnitStatus.Idle;
            }
        }
    }
}