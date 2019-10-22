﻿using System;
using AoAndSugi.Game.Models;
using UnityEngine;

namespace AoAndSugi.Game
{
    #if UNITY_EDITOR
    [CreateAssetMenu(menuName = "AoAndSugi/SpeciesCommonData")]
    #endif
    public sealed class SpeciesCommonData
        : ScriptableObject,
            ISpeciesFacade
    {
        public UnitCommonData[] UnitCommonData;
        private ISpeciesUnitInfoProvider[] unitInfoProviders;

        public SpeciesType SpeciesType => UnitCommonData[0].SpeciesType;

        public ISpeciesUnitInfoProvider[] UnitInfoProviders
        {
            get
            {
                if (!(unitInfoProviders is null)) return unitInfoProviders;
                if (UnitCommonData.Length == 0)
                {
                    unitInfoProviders = Array.Empty<ISpeciesUnitInfoProvider>();
                }
                else
                {
                    unitInfoProviders = new ISpeciesUnitInfoProvider[UnitCommonData.Length];
                    for (var i = 0; i < UnitCommonData.Length; i++)
                    {
                        unitInfoProviders[i] = UnitCommonData[i];
                    }
                }
                return unitInfoProviders;
            }
        }

        public int CompareTo(ISpeciesFacade other) => other is null ? 1 : SpeciesType.CompareTo(other.SpeciesType);
    }
}