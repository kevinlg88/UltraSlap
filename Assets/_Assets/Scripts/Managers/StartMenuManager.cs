using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MoreMountains.Feedbacks;
using Rewired;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;
using TMPro;


public enum GameState
{
    Logo,
    Splash,
    StartMenu,
    Options,
    Credits
}

public class StartMenuManager : MonoBehaviour
{
    public GameState currentGameState = GameState.Logo;

    private Player systemPlayer;

    [SerializeField] private GameObject pressAnyBtn;

    [Header("Input Setup")]
    [SerializeField] string joinAction = "Join";
    [SerializeField] string cancelAction = "Cancel";

    [Header("MMFeedbacks")]
    [SerializeField] private MMFeedbacks LogoToSplashMMFeedbacks;
    [SerializeField] private MMFeedbacks SplashToStartMenuMMFeedbacks;

    [Header("First Selected Options")]
    [SerializeField] private GameObject startingMenuFirst;



    // Start is called before the first frame update
    void Start()
    {
        // SystemPlayer é sempre válido e pega input global
        systemPlayer = ReInput.players.SystemPlayer;

        LogoToSplashMMFeedbacks.PlayFeedbacks();

    }

    // Update is called once per frame
    void Update()
    {
        GetPlayerInput();
    }

    private void GetPlayerInput()
    {
        if (systemPlayer.GetButtonDown(joinAction))
        {
            if (pressAnyBtn.activeSelf)
            {
                SplashToStartMenuMMFeedbacks.PlayFeedbacks();
                pressAnyBtn.SetActive(false);
            }

            Debug.Log("Botão Join apertado (SystemPlayer)");
        }

        if (systemPlayer.GetButtonDown(cancelAction))
        {
            Debug.Log("Botão Cancel apertado (SystemPlayer)");
        }

    }

    public void SwitchToSplash()
    {
        currentGameState = GameState.Splash;

    }

    public void SwitchToStartMenu()
    {
        currentGameState = GameState.StartMenu;

        EventSystem.current.SetSelectedGameObject(startingMenuFirst);
    }


    public void OnStartClick()
    {
        SceneManager.LoadScene("Match_Setup");
    }

    public void OnExitClick()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
        Application.Quit();
    }


}