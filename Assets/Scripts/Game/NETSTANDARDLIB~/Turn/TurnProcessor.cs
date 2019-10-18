using UniNativeLinq;
using Unity.Jobs;

namespace AoAndSugi.Game.Models
{
    public unsafe struct TurnProcessor
    {
        public void ProcessOrderCollection(GameMasterData* master, Turn* turn, NativeEnumerable<Order> orders)
        {
            new LivingJob(master, turn).Run(master->MaxTeamCount);
            new OrderJob(master, turn, orders).Run(master->MaxTeamCount);
            new GenerateJob(master, turn).Run(master->MaxTeamCount);
            new AdvanceJob(master, turn).Run(master->MaxTeamCount);
        }
    }
}