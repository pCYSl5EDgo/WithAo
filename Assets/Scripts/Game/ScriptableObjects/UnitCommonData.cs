using AoAndSugi.Game.Models;
using UnityEngine;
using UnityEngine.Serialization;

namespace AoAndSugi.Game
{
    #if UNITY_EDITOR
    [CreateAssetMenu(menuName = "AoAndSugi/UnitCommonData")]
    #endif
    public sealed class UnitCommonData
        : ScriptableObject,
            ISpeciesUnitInfoProvider
    {
        [SerializeField] private int attackCost;
        [SerializeField] private int attackPoint;
        [SerializeField] private int attackCalculationFormulaIndex;
        [SerializeField] private int attackInterval;
        [SerializeField] private int paintCost;
        [SerializeField] private int paintPoint;
        [SerializeField] private int paintInterval;
        [SerializeField] private uint initialHp;
        [SerializeField] private uint maxHp;
        [SerializeField] private int livingCost;
        [SerializeField] private int livingInterval;
        [SerializeField] private string speciesName;
        [SerializeField] private uint speciesType;
        [SerializeField] private UnitType unitType;
        [SerializeField] private int generationCost;
        [SerializeField] private int generationInterval;

        public int AttackCost => attackCost;

        public int AttackPoint => attackPoint;

        public int AttackCalculationFormulaIndex => attackCalculationFormulaIndex;

        public int AttackInterval => attackInterval;

        public int PaintCost => paintCost;

        public int PaintPoint => paintPoint;

        public int PaintInterval => paintInterval;

        public uint InitialHP => initialHp;

        public uint MaxHP => maxHp;

        public int LivingCost => livingCost;
        
        public int LivingInterval => livingInterval;

        public string SpeciesName => speciesName;

        public uint SpeciesType => speciesType;

        public UnitType UnitType => unitType;

        public int GenerationCost => generationCost;

        public int GenerationInterval => generationInterval;
    }
}