using System;
using AoAndSugi.Game.Models.Unit;
using Unity.Burst;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Jobs;
using Unity.Mathematics;

namespace AoAndSugi.Game.Models
{
    [BurstCompile]
    public unsafe struct GenerateJob : IJob
    {
        [NativeDisableUnsafePtrRestriction] private readonly GameMasterData* master;
        [NativeDisableUnsafePtrRestriction] private readonly Turn* turn;

        public GenerateJob(GameMasterData* master, Turn* turn)
        {
            this.master = master;
            this.turn = turn;
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

        private void ProcessTeams(ref Power power, int teamIndex, uint turnValue)
        {
            if (GuardBlock(ref power, teamIndex)) return;

            ref var miscellaneousDatum = ref power.MiscellaneousData[teamIndex];
            ref var totalHp = ref power.TotalHps[teamIndex].Value;

            if (GuardBlockCanGenerate(ref power, teamIndex, miscellaneousDatum, totalHp, out var generateUnitType, out var speciesType, out var generationCount)) return;

            var generationCost = master->GetGenerationCost(speciesType, generateUnitType).Value;
            
            {
                if (GuardBlockCanPayCost(totalHp, generationCost, out var countCostAffordable)) return;

                if (generationCount > countCostAffordable)
                    generationCount = countCostAffordable;
            }

            if (GuardBlockInterval(turnValue, miscellaneousDatum, speciesType, generateUnitType)) return;

            RenewLastGenerationTurn(turnValue, ref miscellaneousDatum);

            totalHp -= generationCount * generationCost;

            var generateInitialHp = master->GetInitialHp(speciesType, generateUnitType);
            var unitInitialCount = new UnitInitialCount((uint)generationCount);
            var turnId = new TurnId(turnValue);
            if (generateUnitType == UnitType.Queen)
            {
                power.AddInitialCount(teamIndex, unitInitialCount, generateInitialHp, turnId);
                return;
            }
            var spawnPosition = CalcGeneratePosition(width: master->Width, height: master->Height, power.Positions[teamIndex], generateUnitType);

            for (var i = 0; i < power.TeamCount; i++)
            {
                if (!power.Positions[i].Value.Equals(spawnPosition.Value)) continue;
                if (power.Statuses[i] != UnitStatus.Idle) continue;
                power.AddInitialCount(i, unitInitialCount, generateInitialHp, turnId);
                return;
            }

            power.CreateNewUnit(speciesType, generateUnitType, unitInitialCount, generateInitialHp, spawnPosition, turnId);
        }

        private static void RenewLastGenerationTurn(uint turnValue, ref ulong miscellaneousDatum)
        {
            miscellaneousDatum &= 0xFFFF_FFFFUL;
            miscellaneousDatum |= ((ulong) turnValue) << 32;
        }

        private bool GuardBlockInterval(uint turnValue, ulong miscellaneousDatum, SpeciesType speciesType, UnitType generateUnitType)
        {
            var sinceGeneration = turnValue - (uint) (miscellaneousDatum >> 32);

            var interval = master->GetGenerationInterval(speciesType, generateUnitType).Value;

            if (sinceGeneration <= interval) return true;
            return false;
        }

        private static bool GuardBlockCanPayCost(int totalHp, int generationCost, out int countCostAffordable) => (countCostAffordable = (totalHp - 1) / generationCost) == 0;

        private bool GuardBlockCanGenerate(ref Power power, int teamIndex, ulong miscellaneousDatum, int totalHp, out UnitType generateUnitType, out SpeciesType speciesType, out int generationCount)
        {
            generateUnitType = (UnitType)(uint)miscellaneousDatum;

            speciesType = power.SpeciesTypes[teamIndex];


            generationCount = totalHp / master->GetGenerationRequiredHp(speciesType, generateUnitType).Value;

            if (generationCount == 0) return true;
            return false;
        }

        private static bool GuardBlock(ref Power power, int teamIndex)
        {
            if (power.Statuses[teamIndex] != UnitStatus.Generate) return true;

            var unitType = power.UnitTypes[teamIndex];
            if (unitType != UnitType.Queen) return true;
            return false;
        }

        private static UnitPosition CalcGeneratePosition(int width, int height, UnitPosition position, UnitType generateUnitType)
        {
            if (position.Value.x == 0)
            {
                if (position.Value.y == 0)
                {
                    switch (generateUnitType)
                    {
                        case UnitType.Soldier:
                            return new UnitPosition(new int2(0, 1));
                        case UnitType.Worker:
                            return new UnitPosition(new int2(1, 1));
                        case UnitType.Porter:
                            return new UnitPosition(new int2(1, 0));
                    }
                }
                else if (position.Value.y == height - 1)
                {
                    switch (generateUnitType)
                    {
                        case UnitType.Soldier:
                            return new UnitPosition(new int2(0, height - 2));
                        case UnitType.Worker:
                            return new UnitPosition(new int2(1, height - 2));
                        case UnitType.Porter:
                            return new UnitPosition(new int2(1, height - 1));
                    }
                }
                else
                {
                    // 左端
                    switch (generateUnitType)
                    {
                        case UnitType.Soldier:
                            return new UnitPosition(new int2(0, position.Value.y + 1));
                        case UnitType.Worker:
                            return new UnitPosition(new int2(1, position.Value.y));
                        case UnitType.Porter:
                            return new UnitPosition(new int2(0, position.Value.y - 1));
                    }
                }
            }
            else if (position.Value.x == width - 1)
            {
                if (position.Value.y == 0)
                {
                    switch (generateUnitType)
                    {
                        case UnitType.Soldier:
                            return new UnitPosition(new int2(width - 1, 1));
                        case UnitType.Worker:
                            return new UnitPosition(new int2(width - 2, 1));
                        case UnitType.Porter:
                            return new UnitPosition(new int2(width - 2, 0));
                    }
                }
                else if (position.Value.y == height - 1)
                {
                    switch (generateUnitType)
                    {
                        case UnitType.Soldier:
                            return new UnitPosition(new int2(width - 1, height - 2));
                        case UnitType.Worker:
                            return new UnitPosition(new int2(width - 2, height - 2));
                        case UnitType.Porter:
                            return new UnitPosition(new int2(width - 2, height - 1));
                    }
                }
                else
                {
                    // 右端
                    switch (generateUnitType)
                    {
                        case UnitType.Soldier:
                            return new UnitPosition(new int2(width - 1, position.Value.y + 1));
                        case UnitType.Worker:
                            return new UnitPosition(new int2(width - 2, position.Value.y));
                        case UnitType.Porter:
                            return new UnitPosition(new int2(width - 1, position.Value.y - 1));
                    }
                }
            }
            else if (position.Value.y == 0)
            {
                // 下端
                switch (generateUnitType)
                {
                    case UnitType.Soldier:
                        return new UnitPosition(new int2(position.Value.x - 1, 0));
                    case UnitType.Worker:
                        return new UnitPosition(new int2(position.Value.x, 1));
                    case UnitType.Porter:
                        return new UnitPosition(new int2(position.Value.x + 1, 0));
                }
            }
            else if (position.Value.y == height - 1)
            {
                // 上端
                switch (generateUnitType)
                {
                    case UnitType.Soldier:
                        return new UnitPosition(new int2(position.Value.x - 1, height - 1));
                    case UnitType.Worker:
                        return new UnitPosition(new int2(position.Value.x, height - 2));
                    case UnitType.Porter:
                        return new UnitPosition(new int2(position.Value.x + 1, height - 1));
                }
            }
            else
            {
                // まんなか
                switch (generateUnitType)
                {
                    case UnitType.Soldier:
                        return new UnitPosition(new int2(position.Value.x - 1, position.Value.y));
                    case UnitType.Worker:
                        return new UnitPosition(new int2(position.Value.x, position.Value.y + 1));
                    case UnitType.Porter:
                        return new UnitPosition(new int2(position.Value.x + 1, position.Value.y));
                }
            }
            throw new ArgumentOutOfRangeException();
        }
    }
}