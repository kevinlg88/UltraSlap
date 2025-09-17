using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MoreMountains.Feedbacks;
using Rewired;

public class StartMenuManager : MonoBehaviour
{
    private Player systemPlayer;

    [SerializeField] private GameObject pressAnyBtn;

    [Header("Input Setup")]
    [SerializeField] string joinAction = "Join";
    [SerializeField] string cancelAction = "Cancel";

    [Header("MMFeedbacks")]
    [SerializeField] private MMFeedbacks LogoToSplashMMFeedbacks;
    [SerializeField] private MMFeedbacks SplashToStartMenuMMFeedbacks;

    // Start is called before the first frame update
    void Start()
    {
        // SystemPlayer � sempre v�lido e pega input global
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

            Debug.Log("Bot�o Join apertado (SystemPlayer)");
        }

        if (systemPlayer.GetButtonDown(cancelAction))
        {
            Debug.Log("Bot�o Cancel apertado (SystemPlayer)");
        }

    }
}
