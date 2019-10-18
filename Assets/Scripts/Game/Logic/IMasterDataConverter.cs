using AoAndSugi.Game.Models;
using Unity.Mathematics;

namespace AoAndSugi.Game.Models
{
    public interface IMasterDataConverter
    {
        GameMasterData Convert( 
            int2 size,
            int maxTeamCount,
            ISpeciesFacade[] speciesUnitInfoProviders,
            IUnitMovePowerDataProvider[] unitMovePowerDataProviders);
    }
}