namespace AoAndSugi.Game
{
    public interface IAttackInfoProvider
    {
        int AttackCost { get; }
        int AttackPoint { get; }
        /// <summary>
        /// Damage Calculation Formula index
        /// </summary>
        int AttackCalculationFormulaIndex { get; }
        /// <summary>
        /// If this is zero, every turn they attack their enemies.
        /// </summary>
        uint AttackInterval { get; }
        uint AttackRange { get; }
    }
}