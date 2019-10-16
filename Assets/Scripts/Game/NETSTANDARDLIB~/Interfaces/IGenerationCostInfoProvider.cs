namespace AoAndSugi.Game.Models
{
    public interface IGenerationCostInfoProvider
    {
        int GenerationCost { get; }
        int GenerationInterval { get; }
    }
}