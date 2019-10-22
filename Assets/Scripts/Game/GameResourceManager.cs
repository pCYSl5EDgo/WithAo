using System;
using AoAndSugi.Game.Models;
using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using UniNativeLinq;
using Unity.Collections;
using UnityEngine;
using Zenject;

namespace AoAndSugi.Game
{
    public sealed class GameResourceManager : MonoBehaviour
    {
        [Inject] private ISpeciesFacade[] speciesFacades;
        [Inject] private IUnitMovePowerDataProvider[] unitMovePowerDataProviders;
        [Inject] private IMasterDataConverter masterDataConverter;
        [SerializeField] public int DistanceFromEnergySupplier;
        [NonSerialized] private Room room;
        private NativeArray<GameMasterData> masters;
        private NativeArray<Turn> turns;
        private int index = 0;
        public ref Turn CurrentTurn => ref turns.AsRefEnumerableUnsafe()[index];
        private ref readonly GameMasterData master => ref masters.AsRefEnumerableUnsafe()[0];

        private void Awake()
        {
            masters = new NativeArray<GameMasterData>(1, Allocator.Persistent);
            room = PhotonNetwork.CurrentRoom;
//            { "DisplayName", $"{ roomName }" },
//            { "PlayerCount", new MaxTeamCount(){ Value = (int)(playerCount.Value) } },
//            { "NpcCount", new MaxTeamCount(){ Value = (int)(npcCount.Value) } },
//            { "BordSize", new BoardSize(){ Value = new int2(){ x = width.Value, y = height.Value }} },
//            { "MatchTime", new MatchTime(){ Value = matchTime.Value } },
//            { "FoodStorageCount", foodStorageCount },
//            { "EnergySupplyLocationCount", energySupplyLocationCount }
            var hashTable = room.CustomProperties;
            var maxTeamCount = ((MaxTeamCount) hashTable["PlayerCount"]).Value + ((MaxTeamCount) hashTable["NpcCount"]).Value;
            var size = ((BoardSize) hashTable[nameof(BoardSize)]).Value;
            uint energySupplierCount;
            if (hashTable.TryGetValue("EnergySupplyLocationCount", out var _object_))
            {
                energySupplierCount = (uint) _object_;
            }
            else
            {
                energySupplierCount = (uint) maxTeamCount;
                energySupplierCount *= energySupplierCount;
            }
            uint maxTurnCount;
            if (hashTable.TryGetValue("MatchTime", out _object_))
            {
                maxTurnCount = (uint) ((MatchTime) _object_).Value * 60 * 15;
            }
            else
            {
                maxTurnCount = 2 * 60 * 15;
            }
            masters[0] = masterDataConverter.Convert(size, maxTeamCount, energySupplierCount, maxTurnCount, speciesFacades, unitMovePowerDataProviders);

            turns = new NativeArray<Turn>(1, Allocator.Persistent);

            if (!PhotonNetwork.IsMasterClient) return;
            var randomSeed = (uint) DateTime.Now.Ticks.GetHashCode();
            CurrentTurn = default(TurnInitializer).Create(masters, new TurnId(default), randomSeed, DistanceFromEnergySupplier);
            var raiseEventOptions = new RaiseEventOptions
            {
                Receivers = ReceiverGroup.Others
            };
            PhotonNetwork.RaiseEvent(DefineRandomSeed, randomSeed, raiseEventOptions, SendOptions.SendReliable);
        }

        private void OnEnable() => PhotonNetwork.NetworkingClient.EventReceived += NetworkingClientOnEventReceived;
        private void OnDisable() => PhotonNetwork.NetworkingClient.EventReceived -= NetworkingClientOnEventReceived;

        private const byte DefineRandomSeed = 0;

        private void NetworkingClientOnEventReceived(EventData obj)
        {
            switch (obj.Code)
            {
                case DefineRandomSeed:
                    {
                        var randomSeed = (uint) obj.CustomData;
                        CurrentTurn = default(TurnInitializer).Create(masters, new TurnId(default), randomSeed, DistanceFromEnergySupplier);
                    }
                    break;
            }
        }

        private void OnDestroy()
        {
            if (masters.IsCreated)
            {
                masters.AsRefEnumerableUnsafe()[0].Dispose();
                masters.Dispose();
            }
            if (turns.IsCreated)
            {
                CurrentTurn.Board.Dispose();
                foreach (ref var power in CurrentTurn.Powers)
                {
                    power.Dispose();
                }
                CurrentTurn.Powers.Dispose(Allocator.Persistent);
                CurrentTurn.EnergySuppliers.Dispose(Allocator.Persistent);
                CurrentTurn.Dispose();
                turns.Dispose();
            }
        }
    }
}