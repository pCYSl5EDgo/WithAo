namespace AoAndSugi.Game
{
    public interface IHPInfoProvider
    {
        uint InitialHP { get; }
        uint MaxHP { get; }
    }
}