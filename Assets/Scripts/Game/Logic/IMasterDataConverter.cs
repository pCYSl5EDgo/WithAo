using AoAndSugi.Game.Models;
using Unity.Mathematics;

namespace AoAndSugi.Game.Models
{
    public interface IMasterDataConverter
    {
        GameMasterData Convert(int2 size,
            int maxTeamCount,
            uint energySupplierCount,
            uint maxTurnCount,
            ISpeciesFacade[] speciesUnitInfoProviders,
            IUnitMovePowerDataProvider[] unitMovePowerDataProviders);
    }
}