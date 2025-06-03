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
        PlayerData playerData = new PlayerData();
        //playerData.PlayerVisual = playerInput.gameObject;
        playerData.PlayerName = "Player " + playersNumbers;
        _playerManager.AddPlayer(playerData);
        //Debug.Log("Player added: " + playerData.Player);

        playerInput.gameObject.transform.position = spawnPoints[playersNumbers - 1].transform.position;
    }
}
