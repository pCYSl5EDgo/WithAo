using UniNativeLinq;

namespace AoAndSugi.Game.Models
{
    public unsafe struct TurnProcessor
    {
        public void ProcessOrderCollection(GameMasterData* master, Turn* turn, NativeEnumerable<Order> orders)
        {
            new LivingJob(master, turn).Execute();
            new OrderJob(master, turn, orders).Execute();
            new GenerateJob(master, turn).Execute();
            new AdvanceJob(master, turn).Execute();
        }
    }
}