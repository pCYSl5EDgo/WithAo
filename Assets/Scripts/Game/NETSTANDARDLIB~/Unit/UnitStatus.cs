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
        /// 進軍(後種族行動)
        /// </summary>
        AdvanceAndRole,
        /// <summary>
        /// 進軍後命令待機状態移行
        /// </summary>
        AdvanceAndStop,
        /// <summary>
        /// 索敵
        /// </summary>
        Scouting,
        /// <summary>
        /// 索敵中に敵を見つけたからロックオン
        /// </summary>
        LockOn,
        /// <summary>
        /// 女王専用の状態、子どもを生産する
        /// </summary>
        Generate,
        /// <summary>
        /// 戦闘状態
        /// </summary>
        Battle,
        /// <summary>
        /// 食事中
        /// </summary>
        Eating,
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