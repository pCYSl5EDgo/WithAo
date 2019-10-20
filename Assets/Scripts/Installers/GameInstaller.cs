using System;
using AoAndSugi.Game;
using AoAndSugi.Game.Models;
using Unity.Collections;
using Unity.Mathematics;
using UnityEngine;
using Zenject;

namespace AoAndSugi.Installer
{
    public sealed class GameInstaller : MonoInstaller
    {
        [SerializeField] public SpeciesCommonData[] SpeciesUnitInfoProviders;
        [SerializeField] public CellCommonData[] UnitMovePowerDataProviders;
        //private NativeArray<GameMasterData> GameMasterData;

        public override void InstallBindings()
        {
            Container.Bind<ISpeciesFacade[]>().FromInstance(SpeciesUnitInfoProviders);
            Container.Bind<IUnitMovePowerDataProvider[]>().FromInstance(UnitMovePowerDataProviders);
//            var size = Container.Resolve<BoardSize>();
//            GameMasterData = new NativeArray<GameMasterData>(1, Allocator.Persistent)
//            {
//                [0] = new MasterDataConverter().Convert(size.Value, Container.Resolve<MaxTeamCount>().Value, SpeciesUnitInfoProviders, UnitMovePowerDataProviders)
//            };
//            Container.BindInstance(this.GameMasterData);
        }

        private void OnDestroy()
        {
//            if(this.GameMasterData.IsCreated)
//            {
//                this.GameMasterData[0].Dispose();
//                this.GameMasterData.Dispose();
//            }
        }
    }
}