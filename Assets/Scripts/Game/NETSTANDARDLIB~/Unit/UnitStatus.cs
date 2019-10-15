namespace AoAndSugi.Game.Models.Unit
{
    public enum UnitStatus
    {
        /// <summary>
        /// 命令待機
        /// </summary>
        Idle,
        /// <summary>
        /// 目的地入力モード
        /// </summary>
        AskingForDestination,
        /// <summary>
        /// 命令受諾状態
        /// </summary>
        Ordered,
        /// <summary>
        /// 進軍
        /// </summary>
        Advance,
        /// <summary>
        /// 索敵
        /// </summary>
        Scouting,
        /// <summary>
        /// 戦闘状態
        /// </summary>
        Battle,
        /// <summary>
        /// 帰還
        /// </summary>
        Return,
        /// <summary>
        /// 死亡アニメーション中
        /// </summary>
        Dying,
        /// <summary>
        /// 存在しないか、死んだ後
        /// </summary>
        Dead,
    }
}