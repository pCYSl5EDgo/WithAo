using System;
using System.Linq;
using AoAndSugi.Game.Models.Unit;
using UniNativeLinq;
using Unity.Collections;
using Unity.Mathematics;
using UnityEngine;

namespace AoAndSugi.Game.Models
{
    public readonly struct MasterDataConverter : IMasterDataConverter
    {
        public GameMasterData Convert(int2 size, int maxTeamCount, uint energySupplierCount, uint maxTurnCount, ISpeciesFacade[] speciesUnitInfoProviders, IUnitMovePowerDataProvider[] unitMovePowerDataProviders)
        {
            Array.Sort(speciesUnitInfoProviders);
            var speciesTypeCount = speciesUnitInfoProviders.Length;
            var unitTypeCount = Enum.GetValues(typeof(UnitType)).Length;
            var cellTypeCount = Enum.GetValues(typeof(CellType)).Length;

            long MovePowerKeySelector(IUnitMovePowerDataProvider provider)
            {
                return ((provider.TargetSpecies.Value * unitTypeCount) + (uint) provider.TargetUnitType) * cellTypeCount + (uint) provider.TargetCellType;
            }

            var providers = unitMovePowerDataProviders.OrderBy(MovePowerKeySelector).ToArray();
            var initialHpTable = speciesUnitInfoProviders.SelectMany(
                x => x.UnitInfoProviders.Select(
                    y => new UnitInitialHp()
                    {
                        Value = y.InitialHP
                    })).ToArray().AsRefEnumerable().ToNativeEnumerable(Allocator.Persistent);
            return new GameMasterData(
                speciesTypeCount,
                unitTypeCount,
                cellTypeCount,
                size.x,
                size.y,
                maxTeamCount,
                energySupplierCount,
                maxTurnCount,
                initialHpTable,
                speciesUnitInfoProviders.SelectMany(
                    x => x.UnitInfoProviders.Select(
                        y => new UnitMaxHp()
                        {
                            Value = y.MaxHP
                        })).ToArray().AsRefEnumerable().ToNativeEnumerable(Allocator.Persistent),
                providers.SelectMany(x => new UnitMovePower[]
                {
                    new UnitMovePower
                    {
                        Value = x.EarnPowerInItsTerritory,
                    },
                    new UnitMovePower
                    {
                        Value = x.EarnPowerOther,
                    },
                }).ToArray().AsRefEnumerable().ToNativeEnumerable(Allocator.Persistent),
                speciesUnitInfoProviders.SelectMany(
                    x => x.UnitInfoProviders.Select(
                        y => new UnitPaintCost()
                        {
                            Value = y.PaintCost
                        })).ToArray().AsRefEnumerable().ToNativeEnumerable(Allocator.Persistent),
                speciesUnitInfoProviders.SelectMany(
                    x => x.UnitInfoProviders.Select(
                        y => new UnitPaintPoint()
                        {
                            Value = y.PaintPoint
                        })).ToArray().AsRefEnumerable().ToNativeEnumerable(Allocator.Persistent),
                speciesUnitInfoProviders.SelectMany(
                    x => x.UnitInfoProviders.Select(
                        y => new UnitPaintInterval()
                        {
                            Value = y.PaintInterval
                        })).ToArray().AsRefEnumerable().ToNativeEnumerable(Allocator.Persistent),
                speciesUnitInfoProviders.SelectMany(
                    x => x.UnitInfoProviders.Select(
                        y => new UnitGenerationCost()
                        {
                            Value = y.GenerationCost
                        })).ToArray().AsRefEnumerable().ToNativeEnumerable(Allocator.Persistent),
                speciesUnitInfoProviders.SelectMany(
                    x => x.UnitInfoProviders.Select(
                        y => new UnitGenerationInterval()
                        {
                            Value = y.GenerationInterval
                        })).ToArray().AsRefEnumerable().ToNativeEnumerable(Allocator.Persistent),
                speciesUnitInfoProviders.SelectMany(
                        x => x.UnitInfoProviders.Select(
                            y => new UnitGenerationRequiredHp()
                            {
                                Value = y.GenerationRequiredHp
                            })).ToArray()
                    .AsRefEnumerable().ToNativeEnumerable(Allocator.Persistent),
                speciesUnitInfoProviders.SelectMany(
                        x => x.UnitInfoProviders.Select(
                            y => new UnitAttackPoint()
                            {
                                Value = y.AttackPoint
                            })).ToArray()
                    .AsRefEnumerable().ToNativeEnumerable(Allocator.Persistent),
                speciesUnitInfoProviders.SelectMany(
                        x => x.UnitInfoProviders.Select(
                            y => new UnitAttackCost()
                            {
                                Value = y.AttackCost
                            })).ToArray()
                    .AsRefEnumerable().ToNativeEnumerable(Allocator.Persistent),
                speciesUnitInfoProviders.SelectMany(
                        x => x.UnitInfoProviders.Select(
                            y => new UnitAttackInterval()
                            {
                                Value = y.AttackInterval
                            })).ToArray()
                    .AsRefEnumerable().ToNativeEnumerable(Allocator.Persistent),
                speciesUnitInfoProviders.SelectMany(
                        x => x.UnitInfoProviders.Select(
                            y => new UnitAttackRange()
                            {
                                Value = y.AttackRange
                            })).ToArray()
                    .AsRefEnumerable().ToNativeEnumerable(Allocator.Persistent),
                speciesUnitInfoProviders.SelectMany(
                        x => x.UnitInfoProviders.Select(
                            y => new UnitLivingCost()
                            {
                                Value = y.LivingCost
                            })).ToArray()
                    .AsRefEnumerable().ToNativeEnumerable(Allocator.Persistent),
                speciesUnitInfoProviders.SelectMany(
                        x => x.UnitInfoProviders.Select(
                            y => new UnitLivingInterval()
                            {
                                Value = y.LivingInterval
                            })).ToArray()
                    .AsRefEnumerable().ToNativeEnumerable(Allocator.Persistent),
                providers.Select(x => new CellMoveCost(x.Cost))
                    .ToArray()
                    .AsRefEnumerable().ToNativeEnumerable(Allocator.Persistent),
                speciesUnitInfoProviders.SelectMany(
                        x => x.UnitInfoProviders.Select(
                            y => new UnitViewRange()
                            {
                                Value = y.ViewRange
                            })).ToArray()
                    .AsRefEnumerable().ToNativeEnumerable(Allocator.Persistent)
            );
        }
    }
}