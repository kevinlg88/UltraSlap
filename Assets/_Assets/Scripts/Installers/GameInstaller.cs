using Zenject;
using UnityEngine;

public class GameInstaller: MonoInstaller
{
    public override void InstallBindings()
    {
        Debug.Log("GameInstaller: PlayerManager bound as single instance.");
        Container.Bind<PlayerManager>().AsSingle().NonLazy();
        Container.Bind<GameEvent>().AsSingle();
        Container.Bind<ScoreManager>().AsSingle();
    }
}
