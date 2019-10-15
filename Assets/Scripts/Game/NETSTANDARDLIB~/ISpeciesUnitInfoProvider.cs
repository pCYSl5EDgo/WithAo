namespace AoAndSugi.Game
{
    public interface ISpeciesUnitInfoProvider : IAttackInfoProvider, IColorPainterInfoProvider, IHPInfoProvider, ILivingCostInfoProvider
    {
        string SpeciesName { get; }
        uint SpeciesType { get; }

        UnitType UnitType { get; }
    }
}