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
        Debug.Log("player count: " + playerInputManager.playerCount);
/*         PlayerData playerData = new PlayerData();
        playerData.PlayerName = "Player " + (playerInput.playerIndex); */
        //TODO: Enum Input Maps 
        //input.actions.actionMaps["Keyboard2"].Enable();
        //_playerManager.AddPlayer(playerData);
        //Debug.Log("Player added: " + playerData.PlayerName);
    }

    private void CheckGamepad()
    {
        if (Gamepad.all.Count == 0)
        {
            Debug.Log("No Gamepad detected.");
            // Handle keyboard for 2
        }
        else if (Gamepad.all.Count == 1)
        {
            Debug.Log("One Gamepad detected.");
            // Handle keyboard and gamepad for 2
        }
        else
        {
            Debug.Log("more gamepads detected.");
            // Handle Gamepad for 2
        }
    }
}
