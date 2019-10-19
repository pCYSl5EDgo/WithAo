using System;
using AoAndSugi.Game.Models.Unit;
using NUnit.Framework;
using UniNativeLinq;
using Unity.Collections;
using Unity.Mathematics;
using UnityEditor;
using UnityEngine;

namespace AoAndSugi.Game.Models
{
    [TestFixture]
    public sealed unsafe class TurnTest
    {
        private NativeArray<GameMasterData> GameMasterData;

        static T[] GetAssets<T>()
            where T : ScriptableObject
        {
            var guids = AssetDatabase.FindAssets("t:" + typeof(T).Name);
            var answer = guids.LongLength == 0 ? Array.Empty<T>() : new T[guids.LongLength];
            for (int i = 0; i < guids.Length; i++)
            {
                var path = AssetDatabase.GUIDToAssetPath(guids[i]);
                answer[i] = AssetDatabase.LoadAssetAtPath<T>(path);
            }
            return answer;
        }

        private NativeArray<GameMasterData> CreateMaster(int width, int height, int maxTeamCount)
        {
            var speciesUnitInfoProviders = GetAssets<SpeciesCommonData>();
            Debug.Log("SPECIES LENGTH: " + speciesUnitInfoProviders.Length);
            var unitMovePowerDataProviders = GetAssets<CellCommonData>();
            Debug.Log("UNIT MOVE POWER : " + unitMovePowerDataProviders.Length);
            return new NativeArray<GameMasterData>(1, Allocator.Persistent)
            {
                [0] = new MasterDataConverter().Convert(new int2(width, height), maxTeamCount, speciesUnitInfoProviders, unitMovePowerDataProviders)
            };
        }

        private static void Dispose<T>(NativeArray<T> array)
            where T : struct, IDisposable
        {
            array[0].Dispose();
            array.Dispose();
        }

        [TestCase(4, 4, 1)]
        [TestCase(32, 32, 5)]
        [TestCase(256, 256, 10)]
        [TestCase(40, 400, 10)]
        [TestCase(1024, 1024, 20)]
        public void CreateMasterData(int width, int height, int maxTeamCount)
        {
            var masters = CreateMaster(width, height, maxTeamCount);
            try
            {
                Assert.AreEqual(1, masters.Length);
                var enumerable = masters.AsRefEnumerableUnsafe();
                Assert.AreEqual(1L, enumerable.Length);
                ref var master = ref enumerable[0];
                Assert.AreEqual(width, master.Width);
                Assert.AreEqual(height, master.Height);
                Assert.AreEqual(maxTeamCount, master.MaxTeamCount);
                Assert.AreEqual(100U, master.GetInitialHp(new SpeciesType(0), UnitType.Soldier).Value);
                Assert.AreEqual(500U, master.GetInitialHp(new SpeciesType(0), UnitType.Worker).Value);
                Assert.AreEqual(2000U, master.GetInitialHp(new SpeciesType(0), UnitType.Porter).Value);
                Assert.AreEqual(500U, master.GetInitialHp(new SpeciesType(0), UnitType.Queen).Value);
            }
            finally
            {
                Dispose(masters);
            }
        }

        [TestCase(4, 4, 1, 1)]
        [TestCase(32, 32, 5, 4)]
        [TestCase(256, 256, 10, 16)]
        [TestCase(40, 400, 10, 20)]
        [TestCase(1024, 1024, 20, 32)]
        public void CreateTurn(int width, int height, int maxTeamCount, int energyCount)
        {
            var masters = CreateMaster(width, height, maxTeamCount);
            using (var powerArray = new NativeArray<Power>(maxTeamCount, Allocator.Persistent))
            using (var energyArray = new NativeArray<EnergySupplier>(energyCount, Allocator.Persistent))
            {
                var powers = powerArray.AsRefEnumerableUnsafe();
                for (var i = 0L; i < powers.Length; i++)
                {
                    ref var power = ref powers[i];
                    power = new Power(new PowerId((uint) i), 40);
                    var queenInitialHp = masters[0].GetInitialHp(new SpeciesType(0U), UnitType.Queen);
                    power.CreateNewUnit(new SpeciesType(0U), UnitType.Queen, new UnitInitialCount(1U), queenInitialHp, new UnitPosition(new int2(1 << (int) i, 0)), new TurnId(0U));
                    Assert.AreEqual(1, power.TeamCount);
                    Assert.AreEqual(1U, power.InitialCounts[0].Value);
                    Assert.AreEqual(queenInitialHp.Value, (uint) power.TotalHps[0].Value);
                    Assert.AreEqual(new int2(1 << (int) i, 0), power.Positions[0].Value);
                }
                var energies = energyArray.AsRefEnumerableUnsafe();
                for (var i = 0; i < energies.Length; i++)
                {
                    ref var energy = ref energies[i];
                    energy.Value = 1000;
                    energy.Position = new int2(i, i * i);
                }
                try
                {
                    var turn = new Turn()
                    {
                        Board = new Board(new int2(width, height)),
                        Powers = powers,
                        TurnId = default,
                        EnergySuppliers = energies
                    };
                    try
                    {
                    }
                    finally
                    {
                        turn.Board.Dispose();
                    }
                }
                finally
                {
                    Dispose(masters);
                }
            }
        }

        [TestCase(4, 4, 1, 1)]
        [TestCase(32, 32, 5, 4)]
        public void CreateOrder(int width, int height, int maxTeamCount, int energyCount)
        {
            var processor = new TurnProcessor();
            var masters = CreateMaster(width, height, maxTeamCount);
            using (var powerArray = new NativeArray<Power>(maxTeamCount, Allocator.Persistent))
            using (var energyArray = new NativeArray<EnergySupplier>(energyCount, Allocator.Persistent))
            using (var orderArray = new NativeArray<Order>(1, Allocator.Persistent)
            {
                [0] = new Order()
                {
                    ForQueenGenerateType = UnitType.Soldier,
                    Kind = OrderKind.Generate,
                    Power = default,
                    InitialCount = new UnitInitialCount(1),
                    TurnId = default,
                    UnitId = default,
                }
            })
            using (var board = new Board(new int2(width, height)))
            using (var turnArray = new NativeArray<Turn>(1, Allocator.Persistent))
            {
                var powers = powerArray.AsRefEnumerableUnsafe();
                for (var i = 0L; i < powers.Length; i++)
                {
                    ref var power = ref powers[i];
                    power = new Power(new PowerId((uint) i), 40);
                    var queenInitialHp = masters[0].GetInitialHp(new SpeciesType(0U), UnitType.Queen);
                    Debug.Log("QUEEN: " + queenInitialHp);
                    power.CreateNewUnit(new SpeciesType(0U), UnitType.Queen, new UnitInitialCount(1U), queenInitialHp, new UnitPosition(new int2(1 << (int) i, 0)), new TurnId(0U));
                    Debug.Log("OLD HP: " + power.TotalHps[0].Value);
                }
                var energies = InitializeEnergies(energyArray);
                turnArray.AsRefEnumerableUnsafe()[0] = new Turn()
                {
                    Board = board,
                    Powers = powers,
                    TurnId = default,
                    EnergySuppliers = energies
                };
                try
                {
                    ref var power = ref powers[0];
                    Assert.AreNotEqual(0, masters[0].GetInitialHp(default, UnitType.Soldier));
                    Debug.Log("OLD HP: " + power.TotalHps[0].Value);
                    processor.ProcessOrderCollection(masters.AsRefEnumerableUnsafe().Ptr, turnArray.AsRefEnumerableUnsafe().Ptr, orderArray.AsRefEnumerable());

                    Debug.Log("STATUS: " + power.Statuses[0]);
                    Debug.Log("TEAM: " + power.TeamCount);
                    Debug.Log("POS: " + power.Positions[0].Value);
                    Debug.Log("NEW HP: " + power.TotalHps[0].Value);
                }
                finally
                {
                    Dispose(masters);
                }
            }
        }

        private static NativeEnumerable<EnergySupplier> InitializeEnergies(NativeArray<EnergySupplier> energyArray)
        {
            var energies = energyArray.AsRefEnumerableUnsafe();
            for (var i = 0; i < energies.Length; i++)
            {
                ref var energy = ref energies[i];
                energy.Value = 1000;
                energy.Position = new int2(i, i * i);
            }
            return energies;
        }
    }
}