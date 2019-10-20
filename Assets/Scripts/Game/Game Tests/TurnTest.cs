﻿using System;
using System.Text;
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
            var unitMovePowerDataProviders = GetAssets<CellCommonData>();
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

        private static NativeEnumerable<EnergySupplier> InitializeEnergies(NativeArray<EnergySupplier> energyArray)
        {
            var energies = energyArray.AsRefEnumerableUnsafe();
            for (var i = 0; i < energies.Length; i++)
            {
                ref var energy = ref energies[i];
                energy.Value = 10000;
                energy.Position = new int2(3, i * i);
            }
            return energies;
        }

        [TestCase(4, 4, 1, 1)]
        [TestCase(32, 32, 5, 4)]
        public void CreateOrder(int width, int height, int maxTeamCount, int energyCount)
        {
            var processor = new TurnProcessor();
            var masters = CreateMaster(width, height, maxTeamCount);
            using (var powerArray = new NativeArray<Power>(maxTeamCount, Allocator.Persistent))
            using (var energyArray = new NativeArray<EnergySupplier>(energyCount, Allocator.Persistent))
            using (var orderArray = new NativeArray<Order>(3, Allocator.Persistent)
            {
                [0] = new Order()
                {
                    Type = UnitType.Soldier,
                    Kind = OrderKind.Generate,
                    Power = default,
                    InitialCount = new UnitInitialCount(1),
                    TurnId = default,
                    UnitId = default,
                },
                [1] = new Order()
                {
                    Kind = OrderKind.AdvanceAndStop,
                    Power = default,
                    InitialCount = new UnitInitialCount(4),
                    Destination = new UnitDestination(new int2(3, 0)),
                    TurnId = new TurnId(30),
                    UnitId = new UnitId(1),
                },
                [2] = new Order()
                {
                    Kind = OrderKind.AdvanceAndStop,
                    Power = default,
                    InitialCount = new UnitInitialCount(4),
                    Destination = new UnitDestination(new int2(1, 0)),
                    TurnId = new TurnId(40),
                    UnitId = new UnitId(1),
                }
            })
            using (var board = new Board(new int2(width, height)))
            using (var turnArray = new NativeArray<Turn>(1, Allocator.Persistent))
            {
                var orders = orderArray.AsRefEnumerableUnsafe();
                var turns = turnArray.AsRefEnumerableUnsafe();
                var powers = powerArray.AsRefEnumerableUnsafe();
                for (var i = 0L; i < powers.Length; i++)
                {
                    ref var power = ref powers[i];
                    power = new Power(new PowerId((uint) i), 40);
                    var queenInitialHp = masters[0].GetInitialHp(new SpeciesType(0U), UnitType.Queen);
                    power.CreateNewUnit(new SpeciesType(0U), UnitType.Queen, new UnitInitialCount(1U), queenInitialHp, new UnitPosition(new int2(1 << (int) i, 0)), new TurnId(0U));
                }
                var energies = InitializeEnergies(energyArray);
                turns[0] = new Turn(default, board, powers, energies, 114u);
                try
                {
                    ref var power = ref powers[0];
                    var generateInitialHp = masters[0].GetInitialHp(default, UnitType.Soldier);
                    var queenInitialHp = masters[0].GetInitialHp(default, UnitType.Queen);
                    Assert.AreNotEqual(0U, generateInitialHp.Value);
                    Assert.AreNotEqual(0U, queenInitialHp.Value);
                    Assert.AreEqual((int) queenInitialHp.Value, power.TotalHps[0].Value);
                    for (uint i = 0; i < 501; i++)
                    {
                        turns[0].TurnId = new TurnId(i);
                        processor.ProcessOrderCollection(masters.AsRefEnumerableUnsafe().Ptr, turns.Ptr, orders);
                        var buf = new StringBuilder().Append("TURN : ").Append(turns[0].TurnId.ToString()).Append(", TeamCount : ").Append(power.TeamCount);
                        for (var j = 0; j < power.TeamCount; j++)
                        {
                            buf.Append("\n\tPosition : ").Append(power.Positions[j].Value.ToString())
                                .Append(", Unit Type : ").Append(power.UnitTypes[j].ToString())
                                .Append(", Status : ").Append(power.Statuses[j].ToString())
                                .Append(", Hp : ").Append(power.TotalHps[j].ToString())
                                .Append(", Count : ").Append(power.CalcUnitCountInTeam(j, generateInitialHp))
                                .Append(", Move : ").Append(power.MovePowers[j]);
                        }
                        for (var j = 0; j < 4; j++)
                        {
                            var value = turns[0].Board[width, new int2(j, 0)][0];
                            if (value == 0) continue;
                            buf.Append("\n\t\t (").Append(j).Append(", 0) : ").Append(value);
                        }
                        Debug.Log(buf.ToString());
                        //Debug.Log("T : " + i + ", " + turns[0].Board[width, default][0] + ", Team : " + power.TeamCount + ", Count : " + power.InitialCounts[1]);
                        //Assert.AreEqual(UnitStatus.Generate, power.Statuses[0]);
                        //Assert.AreEqual(1 + ((i + 1) / 15), power.TeamCount);
                        //Assert.AreEqual(queenInitialHp.Value - 1U - i - ((i + 1) / 15), (uint) power.TotalHps[0].Value);
                    }
                }
                finally
                {
                    turns[0].Dispose();
                    Dispose(masters);
                }
            }
        }

        [TestCase(1024, 1024, 4)]
        public void BattleTest(int width, int height, int maxTeamCount)
        {
            var processor = new TurnProcessor();
            using (var masterArray = CreateMaster(width, height, maxTeamCount))
            using (var powerArray = new NativeArray<Power>(maxTeamCount, Allocator.Persistent))
            using (var energyArray = new NativeArray<EnergySupplier>(512, Allocator.Persistent))
            using (var board = new Board(new int2(width, height)))
            using (var turnArray = new NativeArray<Turn>(1, Allocator.Persistent))
            using (var orderArray = new NativeArray<Order>(4, Allocator.Persistent))
            {
                InitializeBoard(board, width, height);
                InitializePowers(powerArray, masterArray, width, height);
                var orders = orderArray.AsRefEnumerable();
                var turns = turnArray.AsRefEnumerableUnsafe();
                var turnPtr = turns.Ptr;
                var powers = powerArray.AsRefEnumerableUnsafe();
            }
        }

        private static void InitializePowers(NativeArray<Power> powerArray, NativeArray<GameMasterData> masterArray, int width, int height)
        {
            var masterPtr = masterArray.AsRefEnumerableUnsafe().Ptr;
            var soldierHp = masterPtr->GetInitialHp(default, UnitType.Soldier);
            InitializePower(width, height, powerArray, 0, new int2(-1, -1), soldierHp);
            InitializePower(width, height, powerArray, 1, new int2(-1, 1), soldierHp);
            InitializePower(width, height, powerArray, 2, new int2(1, -1), soldierHp);
            InitializePower(width, height, powerArray, 3, new int2(1, 1), soldierHp);
        }

        private static void InitializePower(int width, int height, NativeArray<Power> powers, int index, int2 diff, UnitInitialHp soldierHp)
        {
            ref var power = ref powers.AsRefEnumerableUnsafe()[index];
            var basePos = new int2(width >> 1, height >> 1);
            for (var i = 0; i < 10; i++)
            {
                basePos += diff;
                power.CreateNewUnit(default, UnitType.Soldier, new UnitInitialCount(100), soldierHp, new UnitPosition(basePos), new TurnId());
            }
        }

        private static void InitializeBoard(Board board, int width, int height)
        {
            for (var x = width >> 1; --x >= 0;)
            {
                for (var y = height >> 1; --y >= 0;)
                {
                    board[width, new int2(x, y)].AddPaint(0, 50);
                }
                for (var y = height >> 1; y < height; y++)
                {
                    board[width, new int2(x, y)].AddPaint(1, 50);
                }
            }
            for (var x = (height >> 1); x < width; x++)
            {
                for (var y = height >> 1; --y >= 0;)
                {
                    board[width, new int2(x, y)].AddPaint(2, 50);
                }
                for (var y = height >> 1; y < height; y++)
                {
                    board[width, new int2(x, y)].AddPaint(3, 50);
                }
            }
        }
    }
}