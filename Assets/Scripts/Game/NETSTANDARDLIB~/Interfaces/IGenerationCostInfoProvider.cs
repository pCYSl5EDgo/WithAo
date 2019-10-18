namespace AoAndSugi.Game.Models
{
    public interface IGenerationCostInfoProvider
    {
        int GenerationCost { get; }
        uint GenerationInterval { get; }
        int GenerationRequiredHp { get; }
    }
}