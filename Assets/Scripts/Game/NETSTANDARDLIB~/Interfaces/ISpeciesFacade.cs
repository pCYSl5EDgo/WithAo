using System;

namespace AoAndSugi.Game.Models
{
    public interface ISpeciesFacade : IComparable<ISpeciesFacade>
    {
        SpeciesType SpeciesType { get; }
        ISpeciesUnitInfoProvider[] UnitInfoProviders { get; }
    }
}