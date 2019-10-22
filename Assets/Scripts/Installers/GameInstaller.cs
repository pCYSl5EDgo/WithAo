using AoAndSugi.Game;
using AoAndSugi.Game.Models;
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
        [SerializeField] public Mesh QuadMesh;
        
        public override void InstallBindings()
        {
            Container.Bind<ISpeciesFacade[]>().FromInstance(SpeciesUnitInfoProviders);
            Container.Bind<IUnitMovePowerDataProvider[]>().FromInstance(UnitMovePowerDataProviders);
            Container.Bind<IMasterDataConverter>().FromInstance(new MasterDataConverter());
            Container.BindInstance(UnitDrawer).WithId(nameof(UnitDrawer));
            Container.BindInstance(FieldDrawer).WithId(nameof(FieldDrawer));
            Container.BindInstance(QuadMesh).WithId(nameof(QuadMesh));
        }
    }
}