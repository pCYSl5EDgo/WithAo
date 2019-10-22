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

        [SerializeField] public Material UnitDrawer;
        [SerializeField] public Shader FieldDrawer;
        
        public override void InstallBindings()
        {
            Container.Bind<ISpeciesFacade[]>().FromInstance(SpeciesUnitInfoProviders);
            Container.Bind<IUnitMovePowerDataProvider[]>().FromInstance(UnitMovePowerDataProviders);
            Container.BindInstance(UnitDrawer).WithId(nameof(UnitDrawer));
            Container.BindInstance(FieldDrawer).WithId(nameof(FieldDrawer));
        }
    }
}