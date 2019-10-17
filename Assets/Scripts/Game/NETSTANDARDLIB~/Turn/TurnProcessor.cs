using System;
using UniNativeLinq;
using Unity.Jobs;

namespace AoAndSugi.Game.Models
{
    using Unit;
    public unsafe struct TurnProcessor
    {
        public void ProcessOrderCollection(GameMasterData* master, Turn* turn, NativeEnumerable<Order> orders)
        {
            new OrderJob(master, turn, orders).Run(20);
        }

    }
}