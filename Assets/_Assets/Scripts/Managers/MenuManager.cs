using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Zenject;

public class MenuManager : MonoBehaviour
{
    [Inject]
    private PlayerManager _playerManager;
    //[Inject]
    /* public void Construct(PlayerManager playerManager)
    {
        _playerManager = playerManager;
    } */
    PlayerInputManager playerInputManager;
    int playersNumbers = 0;
    private void Awake()
    {
        playerInputManager = GetComponent<PlayerInputManager>();
    }
    void Start()
    {
        _playerManager.ClearPlayers();
        Debug.Log("MenuManager: PlayerManager cleared.");
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.K))
        {
            playerInputManager.JoinPlayer(
                controlScheme: "Keyboard2",
                pairWithDevice: Keyboard.current
            );
        }
    }

    public void OnPlayerJoined(PlayerInput playerInput)
    {
        Debug.Log("Player joined: " + playerInput.playerIndex);
        playersNumbers++;
        if (playerInput.devices[0] is Keyboard)
        {
            if (playersNumbers == 1) playerInput.SwitchCurrentActionMap("UIKeyboard1");
            else if (playersNumbers == 2) playerInput.SwitchCurrentActionMap("UIKeyboard2");
        }
        else if (playerInput.devices[0] is Gamepad)
        {
            playerInput.SwitchCurrentActionMap("UIGamepad");   
        }
    }
}
