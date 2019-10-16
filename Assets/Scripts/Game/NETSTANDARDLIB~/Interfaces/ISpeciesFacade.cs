namespace AoAndSugi.Game.Models
{
    public interface ISpeciesFacade
    {
        ISpeciesUnitInfoProvider[] UnitInfoProviders { get; }
    }
}