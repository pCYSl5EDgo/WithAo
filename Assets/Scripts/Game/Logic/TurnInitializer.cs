using AoAndSugi.Game.Models.Unit;
using UniNativeLinq;
using Unity.Collections;
using Unity.Mathematics;
using Random = Unity.Mathematics.Random;

namespace AoAndSugi.Game.Models
{
    public struct TurnInitializer
    {
        public Turn Create(NativeArray<GameMasterData> masterData, TurnId turnId, uint randomSeed, int energySupplierNeighborDistance)
        {
            ref var master = ref masterData.AsRefEnumerableUnsafe()[0];
            var powers = NativeEnumerable.Create<Power>(master.MaxPowerCount, Allocator.Persistent);
            var random = new Random(randomSeed);
            {
                var index = 0U;
                foreach (ref var power in powers)
                {
                    power = new Power(new PowerId(index++), 16);
                    var seeds = random.NextUInt3();
                    random.state = seeds.x;
                    var posX = random.NextInt(0, master.Width - 1);
                    random.state = seeds.y;
                    var posY = random.NextInt(0, master.Height - 1);
                    random.state = seeds.z;
                    power.CreateNewUnit(new SpeciesType(), UnitType.Queen, new UnitInitialCount(1), master.GetInitialHp(new SpeciesType(), UnitType.Queen), new UnitPosition(new int2(posX, posY)), turnId);
                }
            }
            var energySuppliers = NativeEnumerable.Create<EnergySupplier>(master.EnergySupplierCount, Allocator.Persistent);
            energySuppliers.ForEach((ref EnergySupplier supplier) => supplier = default);
            var answer = new Turn(turnId, new Board(new int2(master.Width, master.Height)), powers, energySuppliers, random.state);
            answer.ReFillEnergySuppliers(ref master);
            {
                var take = energySuppliers.Take(master.MaxPowerCount);
                for (var i = 0L; i < take.Length; i++)
                {
                    var seeds = random.NextUInt2();
                    random.state = seeds.x;
                    var diff = random.NextInt2(-energySupplierNeighborDistance, energySupplierNeighborDistance);
                    random.state = seeds.y;
                    take[i].Position = powers[i].Positions[0].Value + diff;
                }
            }
            answer.RandomSeed = random.state;
            return answer;
        }
    }
}