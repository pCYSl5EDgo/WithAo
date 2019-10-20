using System;
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
        [SerializeField] private uint attackInterval;
        [SerializeField] private uint attackRange;
        [SerializeField] private int paintCost;
        [SerializeField] private int paintPoint;
        [SerializeField] private uint paintInterval;
        [SerializeField] private uint initialHp;
        [SerializeField] private uint maxHp;
        [SerializeField] private int livingCost;
        [SerializeField] private uint livingInterval;
        [SerializeField] private string speciesName;
        [SerializeField] private uint speciesType;
        [SerializeField] private UnitType unitType;
        [SerializeField] private int generationCost;
        [SerializeField] private uint generationInterval;
        [SerializeField] private int generationRequiredHp;
        [SerializeField] private uint viewRange;

        public int AttackCost => attackCost;

        public int AttackPoint => attackPoint;

        public int AttackCalculationFormulaIndex => attackCalculationFormulaIndex;

        public uint AttackInterval => attackInterval;

        public uint AttackRange => attackRange;

        public int PaintCost => paintCost;

        public int PaintPoint => paintPoint;

        public uint PaintInterval => paintInterval;

        public uint InitialHP => initialHp;

        public uint MaxHP => maxHp;

        public int LivingCost => livingCost;

        public uint LivingInterval => livingInterval;

        public string SpeciesName => speciesName;

        public SpeciesType SpeciesType => new SpeciesType(speciesType);

        public UnitType UnitType => unitType;

        public uint ViewRange => viewRange;

        public int GenerationCost => generationCost;

        public uint GenerationInterval => generationInterval;

        public int GenerationRequiredHp => generationRequiredHp;
    }
}