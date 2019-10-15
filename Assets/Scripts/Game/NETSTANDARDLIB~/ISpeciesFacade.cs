namespace AoAndSugi.Game
{
    public interface ISpeciesFacade
    {
        ISpeciesUnitInfoProvider[] UnitInfoProviders { get; }
    }
}