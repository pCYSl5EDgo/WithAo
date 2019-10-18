namespace AoAndSugi.Game
{
    public interface ILivingCostInfoProvider
    {
        int LivingCost { get; }
        /// <summary>
        /// If this is zero, every turn they attack their enemies.
        /// </summary>
        uint LivingInterval { get; }
    }
}