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
        uint SpeciesType { get; }

        UnitType UnitType { get; }
    }
}