namespace AoAndSugi.Game
{
    public interface IColorPainterInfoProvider
    {
        int PaintCost { get; }
        int PaintPoint { get; }
        /// <summary>
        /// If this is zero, every turn they attack their enemies.
        /// </summary>
        uint PaintInterval { get; }
    }
}