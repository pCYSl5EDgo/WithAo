using System;
using System.Runtime.CompilerServices;
using AoAndSugi.Game.Models.Unit;
using UniNativeLinq;
using Unity.Collections;

namespace AoAndSugi.Game.Models
{
    public struct GameMasterData : IDisposable
    {
        private NativeEnumerable<UnitInitialHp> initialHpTable;
        private NativeEnumerable<UnitMovePower> movePowerTable;
        private NativeEnumerable<UnitMaxHp> maxHpTable;
        private NativeEnumerable<UnitPaintPoint> paintPointTable;
        private NativeEnumerable<UnitPaintCost> paintCostTable;
        private NativeEnumerable<UnitPaintInterval> paintIntervalTable;
        private NativeEnumerable<UnitGenerationCost> generationCostTable;
        private NativeEnumerable<UnitGenerationInterval> generationIntervalTable;
        private NativeEnumerable<UnitAttackPoint> attackPointTable;
        private NativeEnumerable<UnitAttackCost> attackCostTable;
        private NativeEnumerable<UnitAttackInterval> attackIntervalTable;
        private NativeEnumerable<UnitLivingCost> livingCostTable;
        private NativeEnumerable<UnitLivingInterval> livingIntervalTable;

        private readonly int speciesTypeCount;
        private readonly int unitTypeCount;
        private readonly int cellTypeCount;

        public GameMasterData(int speciesTypeCount, int unitTypeCount, int cellTypeCount, NativeEnumerable<UnitInitialHp> initialHpTable, NativeEnumerable<UnitMovePower> movePowerTable, NativeEnumerable<UnitMaxHp> maxHpTable, NativeEnumerable<UnitPaintCost> paintCostTable, NativeEnumerable<UnitPaintPoint> paintPointTable, NativeEnumerable<UnitPaintInterval> paintIntervalTable, NativeEnumerable<UnitGenerationCost> generationCostTable, NativeEnumerable<UnitGenerationInterval> generationIntervalTable, NativeEnumerable<UnitAttackPoint> attackPointTable, NativeEnumerable<UnitAttackCost> attackCostTable, NativeEnumerable<UnitAttackInterval> attackIntervalTable, NativeEnumerable<UnitLivingCost> livingCostTable, NativeEnumerable<UnitLivingInterval> livingIntervalTable)
        {
            this.speciesTypeCount = speciesTypeCount;
            this.unitTypeCount = unitTypeCount;
            this.cellTypeCount = cellTypeCount;
            this.initialHpTable = initialHpTable;
            this.movePowerTable = movePowerTable;
            this.maxHpTable = maxHpTable;
            this.paintCostTable = paintCostTable;
            this.paintPointTable = paintPointTable;
            this.paintIntervalTable = paintIntervalTable;
            this.generationCostTable = generationCostTable;
            this.generationIntervalTable = generationIntervalTable;
            this.attackPointTable = attackPointTable;
            this.attackCostTable = attackCostTable;
            this.attackIntervalTable = attackIntervalTable;
            this.livingCostTable = livingCostTable;
            this.livingIntervalTable = livingIntervalTable;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private readonly long IndexFrom2(SpeciesType speciesType, UnitType unitType) => speciesType.Value * unitTypeCount + (long)unitType;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private readonly long IndexFrom3(SpeciesType speciesType, UnitType unitType, CellType cellType) => IndexFrom2(speciesType, unitType) * cellTypeCount + (long)cellType;

        public readonly ref readonly UnitInitialHp GetInitialHp(SpeciesType speciesType, UnitType unitType)
            => ref initialHpTable[IndexFrom2(speciesType, unitType)];

        public readonly ref readonly UnitMovePower GetMovePower(SpeciesType speciesType, UnitType unitType, CellType cellType, bool isInItsRegion)
            => ref movePowerTable[(IndexFrom3(speciesType, unitType, cellType) << 1) + (isInItsRegion ? 1 : 0)];

        public readonly ref readonly UnitMaxHp GetMaxHp(SpeciesType speciesType, UnitType unitType)
            => ref maxHpTable[IndexFrom2(speciesType, unitType)];

        public readonly ref readonly UnitPaintCost GetPaintCost(SpeciesType speciesType, UnitType unitType)
            => ref paintCostTable[IndexFrom2(speciesType, unitType)];

        public readonly ref readonly UnitPaintPoint GetPaintPoint(SpeciesType speciesType, UnitType unitType)
            => ref paintPointTable[IndexFrom2(speciesType, unitType)];

        public readonly ref readonly UnitPaintInterval GetPaintInterval(SpeciesType speciesType, UnitType unitType)
            => ref paintIntervalTable[IndexFrom2(speciesType, unitType)];

        public readonly ref readonly UnitGenerationCost GetGenerationCost(SpeciesType speciesType, UnitType unitType)
            => ref generationCostTable[IndexFrom2(speciesType, unitType)];

        public readonly ref readonly UnitGenerationInterval GetGenerationInterval(SpeciesType speciesType, UnitType unitType)
            => ref generationIntervalTable[IndexFrom2(speciesType, unitType)];

        public readonly ref readonly UnitAttackCost GetAttackCost(SpeciesType speciesType, UnitType unitType)
            => ref attackCostTable[IndexFrom2(speciesType, unitType)];

        public readonly ref readonly UnitAttackPoint GetAttackPoint(SpeciesType speciesType, UnitType unitType)
            => ref attackPointTable[IndexFrom2(speciesType, unitType)];

        public readonly ref readonly UnitAttackInterval GetAttackInterval(SpeciesType speciesType, UnitType unitType)
            => ref attackIntervalTable[IndexFrom2(speciesType, unitType)];

        public readonly ref readonly UnitLivingCost GetLivingCost(SpeciesType speciesType, UnitType unitType)
            => ref livingCostTable[IndexFrom2(speciesType, unitType)];

        public readonly ref readonly UnitLivingInterval GetLivingInterval(SpeciesType speciesType, UnitType unitType)
            => ref livingIntervalTable[IndexFrom2(speciesType, unitType)];

        public void Dispose()
        {
            initialHpTable.Dispose(Allocator.Persistent);
            movePowerTable.Dispose(Allocator.Persistent);
            maxHpTable.Dispose(Allocator.Persistent);
            paintCostTable.Dispose(Allocator.Persistent);
            paintPointTable.Dispose(Allocator.Persistent);
            paintIntervalTable.Dispose(Allocator.Persistent);
            generationCostTable.Dispose(Allocator.Persistent);
            generationIntervalTable.Dispose(Allocator.Persistent);
            attackCostTable.Dispose(Allocator.Persistent);
            attackPointTable.Dispose(Allocator.Persistent);
            attackIntervalTable.Dispose(Allocator.Persistent);
            livingCostTable.Dispose(Allocator.Persistent);
            livingIntervalTable.Dispose(Allocator.Persistent);
        }
    }
}