namespace AoAndSugi.Game.Models
{
    using Unit;
    public interface IOrder
    {
        TurnId TurnId { get; }
        OrderKind Kind { get; }
        PowerId Power { get; }
        UnitType Type { get; }
        UnitInitialCount Count { get; }
        UnitDestination Destination { get; }
        UnitId UnitId { get; }
    }
}