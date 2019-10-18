namespace AoAndSugi.Game.Models.Unit
{
    public struct UnitInitialCount
    {
        public uint Value;

        public UnitInitialCount(uint value) => Value = value;

        public override string ToString() => Value.ToString();
    }
}