using System;

namespace AoAndSugi.Game.Models
{
    public interface ISpeciesUnitInfoProvider
        : IAttackInfoProvider,
            IColorPainterInfoProvider,
            IHPInfoProvider,
            ILivingCostInfoProvider,
            IGenerationCostInfoProvider
    {
        string SpeciesName { get; }
        SpeciesType SpeciesType { get; }

        UnitType UnitType { get; }
        uint ViewRange { get; }
    }
}