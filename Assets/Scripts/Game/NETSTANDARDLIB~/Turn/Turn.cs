using System;
using UniNativeLinq;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Random = Unity.Mathematics.Random;
using UnsafeUtilityEx = UniNativeLinq.UnsafeUtilityEx;

namespace AoAndSugi.Game.Models
{
    public unsafe struct Turn : IDisposable
    {
        public uint RandomSeed;
        public TurnId TurnId;
        public Board Board;
        public NativeEnumerable<Power> Powers;
        public NativeEnumerable<EnergySupplier> EnergySuppliers;
        private NativeEnumerable<Order> ordersForLaterTurn;
        private long ordersForLaterTurnCount;
        public NativeEnumerable<Order> OrdersForLaterTurn => ordersForLaterTurn.Take(ordersForLaterTurnCount);

        public void AddOrder(Order order)
        {
            if (ordersForLaterTurn.Length == 0)
            {
                ordersForLaterTurn = NativeEnumerable<Order>.Create(UnsafeUtilityEx.Malloc<Order>(16, Allocator.Persistent), 16);
                UnsafeUtility.MemClear(ordersForLaterTurn.Ptr + 1, 15 * sizeof(Order));
            }
            else if (ordersForLaterTurnCount == ordersForLaterTurn.Length)
            {
                var newEnumerable = NativeEnumerable<Order>.Create(UnsafeUtilityEx.Malloc<Order>(ordersForLaterTurnCount << 1, Allocator.Persistent), ordersForLaterTurnCount << 1);
                UnsafeUtilityEx.MemCpy(newEnumerable.Ptr, ordersForLaterTurn.Ptr, ordersForLaterTurnCount);
                UnsafeUtility.MemClear(newEnumerable.Ptr + ordersForLaterTurnCount + 1, (ordersForLaterTurnCount - 1) * sizeof(Order));
            }
            ordersForLaterTurn[ordersForLaterTurnCount++] = order;
        }

        public Turn(TurnId turnId, Board board, NativeEnumerable<Power> powers, NativeEnumerable<EnergySupplier> energySuppliers, uint randomSeed)
        {
            this.TurnId = turnId;
            this.Board = board;
            this.Powers = powers;
            this.EnergySuppliers = energySuppliers;
            RandomSeed = randomSeed;
            this.ordersForLaterTurn = default;
            this.ordersForLaterTurnCount = default;
        }

        public ref Power this[PowerId id] => ref Powers[id.Value];

        public unsafe void CopyToDeep(ref Turn value)
        {
            if (!Board.CopyTo(ref value.Board))
            {
#if DEBUG
                throw new NullReferenceException();
#endif
            }
            var end = 20L;
            if (end > Powers.Length)
                end = Powers.Length;
            if (end > value.Powers.Length)
                end = value.Powers.Length;
            for (var i = 0L; i != end; i++)
            {
                Powers[i].CopyTo(ref value.Powers[i]);
            }
            value.EnergySuppliers.ReAlloc(EnergySuppliers.Length);
            EnergySuppliers.CopyTo(value.EnergySuppliers.Ptr);
        }

        public void Dispose()
        {
            if (ordersForLaterTurnCount == 0) return;
            ordersForLaterTurn.Dispose(Allocator.Persistent);
        }

        public void ReFillEnergySuppliers(ref GameMasterData master)
        {
            var random = new Random(RandomSeed);
            foreach (ref var energySupplier in EnergySuppliers)
            {
                if (energySupplier.Value > 0) continue;
                var _ = random.NextUInt4();
                random.state = _.x;
                energySupplier.Position.x = random.NextInt(0, master.Width - 1);
                random.state = _.y;
                energySupplier.Position.y = random.NextInt(0, master.Height - 1);
                random.state = _.z;
                energySupplier.Value = random.NextInt(500, 20000);
                random.state = _.w;
            }
            RandomSeed = random.state;
        }
    }
}