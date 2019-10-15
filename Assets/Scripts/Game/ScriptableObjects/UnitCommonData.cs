using UnityEngine;

namespace AoAndSugi.Game
{
#if UNITY_EDITOR
    [CreateAssetMenu(menuName = "AoAndSugi/UnitCommonData")]
#endif
    public sealed class UnitCommonData
        : ScriptableObject
    {
        public string SpeciesName;
        public uint SpeciesType;

        public UnitType UnitType;

        public uint InitialHP;
        public uint MaxHP;

        public int AttackCost;
        public int AttackPoint;
        public int AttackCalculationFormulaIndex;
        public int AttackInterval;

        public MoveType MoveType;

        public int PaintCost;
        public int PaintPoint;
        public int PaintInterval;

        public int LivingCost;
        public int LivingInterval;

        public static implicit operator UnitCommonDataProvider(UnitCommonData @this) 
            => new UnitCommonDataProvider(
                @this.SpeciesName, @this.SpeciesType, @this.UnitType,
                @this.InitialHP, @this.MaxHP, 
                @this.AttackCost, @this.AttackPoint, @this.AttackCalculationFormulaIndex, @this.AttackInterval, 
                @this.MoveType, 
                @this.PaintCost, @this.PaintPoint, @this.PaintInterval,
                @this.LivingCost, @this.LivingInterval
            );
    }

    public sealed class UnitCommonDataProvider
        : ScriptableObject, ISpeciesUnitInfoProvider
    {
        public string SpeciesName { get; }
        public uint SpeciesType { get; }
        
        public UnitType UnitType { get; }

        public uint InitialHP { get; }
        public uint MaxHP { get; }

        public int AttackCost { get; }
        public int AttackPoint { get; }
        public int AttackCalculationFormulaIndex { get; }
        public int AttackInterval { get; } 
        
        public MoveType MoveTypeValue;

        public int PaintCost { get; }
        public int PaintPoint { get; }
        public int PaintInterval { get; }

        public int LivingCost { get; }
        public int LivingCostInterval { get; }

        public UnitCommonDataProvider(
            string speciesName, uint speciesType,  UnitType unitType,
            uint initialHp, uint maxHp, 
            int attackCost, int attackPoint, int attackCalculationFormulaIndex, int attackInterval, 
            MoveType moveTypeValue, 
            int paintCost, int paintPoint, int paintInterval,
            int livingCost,
            int livingCostInterval)
        {
            SpeciesName = speciesName;
            SpeciesType = speciesType;
            UnitType = unitType;
            InitialHP = initialHp;
            MaxHP = maxHp;
            AttackCost = attackCost;
            AttackPoint = attackPoint;
            AttackCalculationFormulaIndex = attackCalculationFormulaIndex;
            AttackInterval = attackInterval;
            MoveTypeValue = moveTypeValue;
            PaintCost = paintCost;
            PaintPoint = paintPoint;
            PaintInterval = paintInterval;
            LivingCost = livingCost;
            LivingCostInterval = livingCostInterval;
        }

    }
}
