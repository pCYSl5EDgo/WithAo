namespace AoAndSugi.Game.Models
{
    public unsafe struct Cell
    {
        public CellType CellTypeValue;
        public int TeamFlags;
        public fixed byte TeamValues[20];
    }

    public enum CellType
    {
        None,
    }
}