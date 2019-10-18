using System;
using AoAndSugi.Game.Models.Unit;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Jobs;
using Unity.Mathematics;

namespace AoAndSugi.Game.Models
{
    public unsafe struct GenerateJob : IJobParallelFor
    {
        [NativeDisableUnsafePtrRestriction] private readonly GameMasterData* master;
        [NativeDisableUnsafePtrRestriction] private readonly Turn* turn;

        public GenerateJob(GameMasterData* master, Turn* turn)
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
            if (power.Statuses[teamIndex] != UnitStatus.Generate) return;

            var unitType = power.UnitTypes[teamIndex];
            if (unitType != UnitType.Queen) return;

            ref var miscellaneousDatum = ref power.MiscellaneousData[teamIndex];
            var generateUnitType = (UnitType)(uint)miscellaneousDatum;

            var speciesType = power.SpeciesTypes[teamIndex];


            ref var totalHp = ref power.TotalHps[teamIndex].Value;

            var generationCount = totalHp / master->GetGenerationRequiredHp(speciesType, generateUnitType).Value;

            if (generationCount == 0) return;

            var generationCost = master->GetGenerationCost(speciesType, generateUnitType).Value;
            var countCostAffordable = (totalHp - 1) / generationCost;

            if (countCostAffordable == 0) return;
            if (generationCount > countCostAffordable)
                generationCount = countCostAffordable;

            var sinceGeneration = turnValue - (uint)(miscellaneousDatum >> 32);

            var interval = master->GetGenerationInterval(speciesType, unitType).Value;

            if (sinceGeneration <= interval) return;

            miscellaneousDatum &= 0xFFFF_FFFFUL;
            miscellaneousDatum |= ((ulong)turnValue) << 32;

            totalHp -= generationCount * generationCost;

            var generateInitialHp = master->GetInitialHp(speciesType, generateUnitType);
            if (generateUnitType == UnitType.Queen)
            {
                power.AddInitialCount(teamIndex, new UnitInitialCount((uint)generationCount), generateInitialHp, new TurnId(turnValue));
                return;
            }
            var spawnPosition = CalcGeneratePosition(width: master->Width, height: master->Height, power.Positions[teamIndex], generateUnitType);

            power.CreateNewUnit(speciesType, generateUnitType, new UnitInitialCount((uint)generationCount), generateInitialHp, spawnPosition, new TurnId(turnValue));
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