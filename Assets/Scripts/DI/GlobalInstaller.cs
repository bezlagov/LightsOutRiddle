using Zenject;
using Assets.Scripts.Services;

/// <summary>
/// The class is responsible for injecting services
/// </summary>
public class GlobalInstaller : MonoInstaller
{
    public override void InstallBindings()
    {
        Container.BindInterfacesAndSelfTo<TimerService>().AsSingle().NonLazy();
        Container.BindInterfacesAndSelfTo<CounterService>().AsSingle().NonLazy();
        Container.BindInterfacesAndSelfTo<GameplayPageController>().FromComponentInHierarchy().AsCached();
        Container.BindInterfacesAndSelfTo<WinPopupController>().FromComponentInHierarchy().AsCached();
        Container.BindInterfacesAndSelfTo<GamefieldController>().FromComponentInHierarchy().AsCached();
        Container.BindInterfacesAndSelfTo<GameService>().AsSingle();
    }
}
