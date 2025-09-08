using Zenject;
using UnityEngine;

public class GameInstaller: MonoInstaller
{
    public override void InstallBindings()
    {
        Debug.Log("GameInstaller: PlayerManager bound as single instance.");
        Container.Bind<PlayerManager>().AsSingle().NonLazy();
        Container.Bind<ScoreManager>().AsSingle();
        Container.Bind<LevelSpawnManager>().AsSingle();
        Container.Bind<GameEvent>().AsSingle();
    }
}
