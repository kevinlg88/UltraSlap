using Zenject;

public class GameInstaller: MonoInstaller
{
    public override void InstallBindings()
    {
        Container.Bind<PlayerManager>().AsSingle().NonLazy();
    }
}
