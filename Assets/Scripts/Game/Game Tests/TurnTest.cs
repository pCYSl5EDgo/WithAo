using System;
using AoAndSugi.Game;
using AoAndSugi.Game.Models;
using NUnit.Framework;
using UniNativeLinq;
using Unity.Collections;
using Unity.Mathematics;
using UnityEditor;
using UnityEngine;

namespace AoAndSugi.Game.Models
{
    [TestFixture]
    public sealed class TurnTest
    {
        private NativeArray<GameMasterData> GameMasterData;

        T[] GetAssets<T>()
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

        NativeArray<GameMasterData> CreateMaster(int width, int height, int maxTeamCount)
        {
            return new NativeArray<GameMasterData>(1, Allocator.Persistent)
            {
                [0] = new MasterDataConverter().Convert(new int2(width, height), maxTeamCount, GetAssets<SpeciesCommonData>(), GetAssets<CellCommonData>())
            };
        }

        void Dispose(NativeArray<GameMasterData> masters)
        {
            masters[0].Dispose();
            masters.Dispose();
        }

        [TestCase(4, 4, 1)]
        [TestCase(32, 32, 5)]
        [TestCase(256, 256, 10)]
        [TestCase(40, 400, 10)]
        [TestCase(1024, 1024, 20)]
        public void TurnTestSimplePasses(int width, int height, int maxTeamCount)
        {
            var masters = CreateMaster(width, height, maxTeamCount);
            Assert.AreEqual(1, masters.Length);
            var enumerable = masters.AsRefEnumerableUnsafe();
            Assert.AreEqual(1L, enumerable.Length);
            foreach (ref var datum in enumerable)
            {
            }
            ref var datum0 = ref enumerable[0];
            Assert.AreEqual(width, datum0.Width);
            Assert.AreEqual(height, datum0.Height);
            Assert.AreEqual(maxTeamCount, datum0.MaxTeamCount);
            var units = (UnitType[]) Enum.GetValues(typeof(UnitType));
            foreach (var speciesType in Enumerable.Range(0U, 1))
            {
                Debug.Log("Species : " + speciesType.ToString());
                foreach (var unitType in units)
                {
                    Debug.Log("Unit : " + unitType.ToString());
                    Debug.Log("AttackCost" + datum0.GetAttackCost(new SpeciesType(speciesType), unitType));
                }
            }
            Dispose(masters);
        }
    }
}