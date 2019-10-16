namespace AoAndSugi.Game.Models
{
    public interface IUnitMovePowerDataProvider
    {
        SpeciesType TargetSpecies { get; }
        UnitType TargetUnitType { get; }
        CellType TargetCellType { get; }

        int EarnPowerInItsTerritory { get; }
        int EarnPowerOther { get; }
    }
}