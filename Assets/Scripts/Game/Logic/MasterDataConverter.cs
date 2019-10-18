using System;
using System.Linq;
using AoAndSugi.Game.Models.Unit;
using UniNativeLinq;
using Unity.Collections;
using Unity.Mathematics;

namespace AoAndSugi.Game.Models
{
    public readonly struct MasterDataConverter : IMasterDataConverter
    {
        public GameMasterData Convert(int2 size, int maxTeamCount, ISpeciesFacade[] speciesUnitInfoProviders, IUnitMovePowerDataProvider[] unitMovePowerDataProviders)
        {
            Array.Sort(speciesUnitInfoProviders);
            var speciesTypeCount = speciesUnitInfoProviders.Length;
            var unitTypeCount = Enum.GetValues(typeof(UnitType)).Length;
            var cellTypeCount = Enum.GetValues(typeof(CellType)).Length;

            long MovePowerKeySelector(IUnitMovePowerDataProvider provider)
            {
                return ((provider.TargetSpecies.Value * unitTypeCount) + (uint) provider.TargetUnitType) * cellTypeCount + (uint) provider.TargetCellType;
            }

            return new GameMasterData(
                speciesTypeCount,
                unitTypeCount,
                cellTypeCount,
                size.x,
                size.y,
                maxTeamCount,
                new NativeArray<UnitInitialHp>(
                    speciesUnitInfoProviders.SelectMany(
                        x => x.UnitInfoProviders.Select(
                            y => new UnitInitialHp()
                            {
                                Value = y.InitialHP
                            })).ToArray(), Allocator.Persistent).AsRefEnumerable(),
                new NativeArray<UnitMaxHp>(
                    speciesUnitInfoProviders.SelectMany(
                        x => x.UnitInfoProviders.Select(
                            y => new UnitMaxHp()
                            {
                                Value = y.MaxHP
                            })).ToArray(), Allocator.Persistent).AsRefEnumerable(),
                new NativeArray<UnitMovePower>(unitMovePowerDataProviders.OrderBy(MovePowerKeySelector).SelectMany(x => new UnitMovePower[]
                {
                    new UnitMovePower
                    {
                        Value = x.EarnPowerInItsTerritory,
                    },
                    new UnitMovePower
                    {
                        Value = x.EarnPowerOther,
                    },
                }).ToArray(), Allocator.Persistent).AsRefEnumerable(),
                new NativeArray<UnitPaintCost>(
                    speciesUnitInfoProviders.SelectMany(
                        x => x.UnitInfoProviders.Select(
                            y => new UnitPaintCost()
                            {
                                Value = y.PaintCost
                            })).ToArray(), Allocator.Persistent).AsRefEnumerable(),
                new NativeArray<UnitPaintPoint>(
                    speciesUnitInfoProviders.SelectMany(
                        x => x.UnitInfoProviders.Select(
                            y => new UnitPaintPoint()
                            {
                                Value = y.PaintPoint
                            })).ToArray(), Allocator.Persistent).AsRefEnumerable(),
                new NativeArray<UnitPaintInterval>(
                    speciesUnitInfoProviders.SelectMany(
                        x => x.UnitInfoProviders.Select(
                            y => new UnitPaintInterval()
                            {
                                Value = y.PaintInterval
                            })).ToArray(), Allocator.Persistent).AsRefEnumerable(),
                new NativeArray<UnitGenerationCost>(
                    speciesUnitInfoProviders.SelectMany(
                        x => x.UnitInfoProviders.Select(
                            y => new UnitGenerationCost()
                            {
                                Value = y.GenerationCost
                            })).ToArray(), Allocator.Persistent).AsRefEnumerable(),
                new NativeArray<UnitGenerationInterval>(
                    speciesUnitInfoProviders.SelectMany(
                        x => x.UnitInfoProviders.Select(
                            y => new UnitGenerationInterval()
                            {
                                Value = y.GenerationInterval
                            })).ToArray(), Allocator.Persistent).AsRefEnumerable(),
                new NativeArray<UnitGenerationRequiredHp>(
                    speciesUnitInfoProviders.SelectMany(
                        x => x.UnitInfoProviders.Select(
                            y => new UnitGenerationRequiredHp()
                            {
                                Value = y.GenerationRequiredHp
                            })).ToArray(), Allocator.Persistent).AsRefEnumerable(),
                new NativeArray<UnitAttackPoint>(
                    speciesUnitInfoProviders.SelectMany(
                        x => x.UnitInfoProviders.Select(
                            y => new UnitAttackPoint()
                            {
                                Value = y.AttackPoint
                            })).ToArray(), Allocator.Persistent).AsRefEnumerable(),
                new NativeArray<UnitAttackCost>(
                    speciesUnitInfoProviders.SelectMany(
                        x => x.UnitInfoProviders.Select(
                            y => new UnitAttackCost()
                            {
                                Value = y.AttackCost
                            })).ToArray(), Allocator.Persistent).AsRefEnumerable(),
                new NativeArray<UnitAttackInterval>(
                    speciesUnitInfoProviders.SelectMany(
                        x => x.UnitInfoProviders.Select(
                            y => new UnitAttackInterval()
                            {
                                Value = y.AttackInterval
                            })).ToArray(), Allocator.Persistent).AsRefEnumerable(),
                new NativeArray<UnitLivingCost>(
                    speciesUnitInfoProviders.SelectMany(
                        x => x.UnitInfoProviders.Select(
                            y => new UnitLivingCost()
                            {
                                Value = y.LivingCost
                            })).ToArray(), Allocator.Persistent).AsRefEnumerable(),
                new NativeArray<UnitLivingInterval>(
                    speciesUnitInfoProviders.SelectMany(
                        x => x.UnitInfoProviders.Select(
                            y => new UnitLivingInterval()
                            {
                                Value = y.LivingInterval
                            })).ToArray(), Allocator.Persistent).AsRefEnumerable(),
                new NativeArray<CellMoveCost>(
                    unitMovePowerDataProviders.OrderBy(MovePowerKeySelector).Select(
                        x => new CellMoveCost(x.Cost))
                        .ToArray(), Allocator.Persistent)
                    .AsRefEnumerable()
            );
        }
    }
}