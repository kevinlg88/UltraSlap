using UnityEngine;
using Rewired;
using System.Collections.Generic;
using Zenject;
using UnityEngine.SceneManagement;

public class RewiredJoinManager : MonoBehaviour
{
    [SerializeField] GameObject playerPrefab;
    [SerializeField] List<GameObject> spawnPoints = new List<GameObject>();

    [Inject]
    private PlayerManager _playerManager;
    string joinActionName = "Join";

    void Start()
    {
        _playerManager.ClearPlayers();
        Debug.Log("JoinManager: PlayerManager cleared.");
    }
    private void Update()
    {
        if (!ReInput.isReady && _playerManager == null) return;
        if (Input.GetKeyDown(KeyCode.Return))
        {
            SceneManager.LoadScene("Game");
        }
        AddSystemPlayerAllControllers();
        if (!ReInput.players.SystemPlayer.GetButtonDown(joinActionName)) return;

        if (ReInput.controllers.Keyboard.GetAnyButtonDown())
        {
            Player player = FindAvailablePlayer();

            if (player != null && !IsKeyboardAlreadyAssigned())
            {
                player.controllers.AddController(ReInput.controllers.Keyboard, false);
                Debug.Log($"Teclado atribuído ao Player {player.id}");
                OnPlayerJoined(player);
            }
        }

        foreach (Joystick joy in ReInput.controllers.Joysticks)
        {
            if (joy.GetAnyButtonDown() && !IsJoystickAlreadyAssigned(joy))
            {
                Player player = FindAvailablePlayer();
                if (player != null)
                {
                    player.controllers.AddController(joy, false);
                    Debug.Log($"Joystick {joy.name} atribuído ao Player {player.id}");
                    OnPlayerJoined(player);
                }
            }
        }
    }
    private Player FindAvailablePlayer()
    {
        foreach (Player player in ReInput.players.GetPlayers())
        {
            if (player.controllers.joystickCount > 0 ||
                player.controllers.hasKeyboard)
            {
                continue;
            }
            return player;
        }
        return null;

    }
    private bool IsJoystickAlreadyAssigned(Joystick joystick)
    {
        foreach (Player player in ReInput.players.GetPlayers())
        {
            if (player.controllers.ContainsController(joystick))
                return true;
        }
        return false;
    }

    private bool IsKeyboardAlreadyAssigned()
    {
        foreach (Player player in ReInput.players.GetPlayers())
        {
            if (player.controllers.hasKeyboard) return true;
        }
        return false;
    }

    private void AddSystemPlayerAllControllers()
    {
        var systemPlayer = ReInput.players.SystemPlayer;
        if (systemPlayer == null) return;

        foreach (var controller in ReInput.controllers.Joysticks)
        {
            if (!systemPlayer.controllers.ContainsController(controller))
            {
                systemPlayer.controllers.AddController(controller, false);
                systemPlayer.controllers.AddController(ReInput.controllers.Keyboard, false);
            }
        }
    }

    private void OnPlayerJoined(Player player)
    {
        PlayerData playerData = new PlayerData();
        Debug.Log("Player joined: " + player.id);
        playerData.PlayerID = player.id;
        playerData.PlayerName = "Player " + player.id;
        _playerManager.AddPlayer(playerData);
        GameObject newPlayer = Instantiate(playerPrefab,
            spawnPoints[player.id].transform.position,
            Quaternion.identity);
        newPlayer.name = "Player " + player.id;
        RigidbodyController rbController = newPlayer.GetComponent<RigidbodyController>();
        rbController.SetPlayerId(player.id);
    }

}

