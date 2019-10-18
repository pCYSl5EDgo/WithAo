using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AoAndSugi.Game.Models
{
    #if UNITY_EDITOR
    [CreateAssetMenu(menuName = "AoAndSugi/CellCommonData")]
    #endif
    public sealed class CellCommonData
        : ScriptableObject,
            IUnitMovePowerDataProvider
    {
        [SerializeField] private SpeciesType targetSpecies;
        [SerializeField] private UnitType targetUnitType;
        [SerializeField] private CellType targetCellType;
        [SerializeField] private int earnPowerInItsTerritory;
        [SerializeField] private int earnPowerOther;
        [SerializeField] private int cost;

        public SpeciesType TargetSpecies => targetSpecies;

        public UnitType TargetUnitType => targetUnitType;

        public CellType TargetCellType => targetCellType;

        public int EarnPowerInItsTerritory => earnPowerInItsTerritory;

        public int EarnPowerOther => earnPowerOther;

        public int Cost => cost;
    }
}