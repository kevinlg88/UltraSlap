using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

public enum SceneIndexEnum
{
    Menu,
    Game,
    ConstructionLevel,
    TestLevel
}
public class LevelSpawnManager
{
    public SceneIndexEnum currentLevel;
    public async Task StartGame(int sceneIndex)
    {
        currentLevel = (SceneIndexEnum)sceneIndex;
        Time.timeScale = 0;
        await LoadSceneAdditive(sceneIndex);
        await ReloadSceneAdditive((int)SceneIndexEnum.Game);
        Time.timeScale = 1;
    }
    public async Task ReloadLevel(int sceneIndex)
    {
        Time.timeScale = 0;
        await ReloadSceneAdditive(sceneIndex);
        await ReloadSceneAdditive((int)SceneIndexEnum.Game);
        Time.timeScale = 1;
    }

    private async Task ReloadSceneAdditive(int sceneIndex)
    {
        var scene = SceneManager.GetSceneByBuildIndex(sceneIndex);

        if (scene.isLoaded)
        {
            AsyncOperation unloadOp = SceneManager.UnloadSceneAsync(sceneIndex);
            while (!unloadOp.isDone)
                await Task.Yield();
        }
        AsyncOperation loadOp = SceneManager.LoadSceneAsync(sceneIndex, LoadSceneMode.Additive);
        loadOp.allowSceneActivation = true;

        while (!loadOp.isDone)
            await Task.Yield();

        Debug.Log($"Cena {sceneIndex} carregada com sucesso!");
    }
    private async Task LoadSceneAdditive(int sceneIndex)
    {
        var scene = SceneManager.GetSceneByBuildIndex(sceneIndex);

        if (scene.isLoaded)
        {
            AsyncOperation unloadOp = SceneManager.UnloadSceneAsync(sceneIndex);
            while (!unloadOp.isDone)
                await Task.Yield();
        }
        AsyncOperation loadOp = SceneManager.LoadSceneAsync(sceneIndex, LoadSceneMode.Single);
        loadOp.allowSceneActivation = true;

        while (!loadOp.isDone)
            await Task.Yield();

        Debug.Log($"Cena {sceneIndex} carregada com sucesso!");
    }
}
