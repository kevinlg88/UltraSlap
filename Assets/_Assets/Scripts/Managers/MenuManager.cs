using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using Zenject;

public class MenuManager : MonoBehaviour
{
    [SerializeField] List<GameObject> spawnPoints = new List<GameObject>();

    [Inject]
    private PlayerManager _playerManager;
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

        if (Input.GetKeyDown(KeyCode.Return))
        {
            SceneManager.LoadScene("Game");
        }
    }

    public void OnPlayerJoined(PlayerInput playerInput)
    {
        PlayerData playerData = new PlayerData();
        Debug.Log("Player joined: " + playerInput.playerIndex);
        playersNumbers++;
        if (playerInput.devices[0] is Keyboard)
        {
            if (playersNumbers == 1)
            {
                // playerInput.defaultActionMap = "UIKeyboard1";
                playerInput.defaultActionMap = "Keyboard1";
                playerInput.SwitchCurrentActionMap("Keyboard1");
                playerData.PlayerUiControllerMap = "UIKeyboard1";
                playerData.PlayerMovementControllerMap = "Keyboard1";
                playerData.InputDevice = playerInput.devices[0];
            }
            else if (playersNumbers == 2)
            {
                // playerInput.defaultActionMap = "UIKeyboard2";
                playerInput.defaultActionMap = "Keyboard2";
                playerInput.SwitchCurrentActionMap("Keyboard2");
                playerData.PlayerUiControllerMap = "UIKeyboard2";
                playerData.PlayerMovementControllerMap = "Keyboard2";
                playerData.InputDevice = playerInput.devices[0];
            }
        }
        else if (playerInput.devices[0] is Gamepad)
        {
            // playerInput.defaultActionMap = "UIGamepad";
            playerInput.defaultActionMap = "Gamepad";
            playerInput.SwitchCurrentActionMap("Gamepad");
            playerData.PlayerUiControllerMap = "UIGamepad";
            playerData.PlayerMovementControllerMap = "Gamepad";
            playerData.InputDevice = playerInput.devices[0];        
        }
        //playerData.PlayerVisual = playerInput.gameObject;
        playerData.PlayerName = "Player " + playersNumbers;
        _playerManager.AddPlayer(playerData);
        //Debug.Log("Player added: " + playerData.Player);

        playerInput.gameObject.transform.position = spawnPoints[playersNumbers - 1].transform.position;
    }
}
