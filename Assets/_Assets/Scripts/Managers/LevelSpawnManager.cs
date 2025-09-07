using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

enum SceneIndexEnum
{
    Menu,
    Game,
    ConstructionLevel
}
public class LevelSpawnManager : MonoBehaviour
{
    [SerializeField] private int currentLevel;
    private int gameScene = 1;
    // Start is called before the first frame update

    public void LoadLevel(int level)
    {
        StartCoroutine(ReloadSceneAdditive((int)SceneIndexEnum.Game));
        StartCoroutine(ReloadSceneAdditive((int)SceneIndexEnum.ConstructionLevel));
    }

    IEnumerator ReloadSceneAdditive(int sceneIndex)
    {
        var scene = SceneManager.GetSceneByBuildIndex(sceneIndex);
        if (scene.isLoaded)
        {
            yield return SceneManager.UnloadSceneAsync(sceneIndex);
        }
        yield return SceneManager.LoadSceneAsync(sceneIndex, LoadSceneMode.Additive);
    }
}
