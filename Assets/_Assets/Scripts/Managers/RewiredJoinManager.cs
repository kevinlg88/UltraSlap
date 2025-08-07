using UnityEngine;
using Rewired;
using System.Collections.Generic;
using Zenject;
using UnityEngine.SceneManagement;

public class RewiredJoinManager : MonoBehaviour
{
    [SerializeField] GameObject playerPrefab;
    [SerializeField] List<GameObject> spawnPoints = new List<GameObject>();
    [SerializeField] int minPlayers = 2;

    [Header("Action Names")]
    [SerializeField] string joinAction = "Join";
    [SerializeField] string cancelAction = "Cancel";

    [Inject]
    private PlayerManager _playerManager;

    void Start()
    {
        _playerManager.ClearPlayers();
        Debug.Log("JoinManager: PlayerManager cleared.");
    }
    private void Update()
    {
        if (!ReInput.isReady && _playerManager == null) return;
        AddSystemPlayerAllControllers();
        KeyboardJoinInput();
        JoystickJoinInput();
        GetPlayerInput();
    }

    private void KeyboardJoinInput()
    {
        if (!ReInput.players.SystemPlayer.GetButtonDown(joinAction)) return;
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
    }
    private void JoystickJoinInput()
    {
        if (!ReInput.players.SystemPlayer.GetButtonDown(joinAction)) return;
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


        //Spawning Player UI
        GameObject newPlayer = Instantiate(playerPrefab,
            spawnPoints[player.id].transform.position,
            Quaternion.identity);
        newPlayer.name = "Player " + player.id;
        PlayerMenuNavigator playerMenuNav = newPlayer.GetComponent<PlayerMenuNavigator>();
        playerMenuNav.SetPlayerId(player.id);

        playerData.PlayerGameObjectRef = newPlayer;

        _playerManager.AddPlayer(playerData);
    }

    private void GetPlayerInput()
    {
        if (_playerManager.Players.Count == 0) return;
        foreach (PlayerData playerData in _playerManager.Players)
        {
            Player player = ReInput.players.GetPlayer(playerData.PlayerID);
            if (player.GetButtonDown(joinAction))
            {
                Debug.Log("Player " + playerData.PlayerID + " menu Join");
                if (playerData.IsReady) TryStartGame();
                else
                {
                    playerData.IsReady = true;
                    PlayerMenuNavigator playerMenuNav = playerData.PlayerGameObjectRef.GetComponent<PlayerMenuNavigator>();
                    PlayerCustomization playerCustomization = playerData.PlayerGameObjectRef.GetComponent<PlayerCustomization>();
                    playerCustomization.SaveCharacterVisual(playerData);
                    playerMenuNav.SetReady(true);
                }
            }
            else if (player.GetButtonDown(cancelAction))
            {
                Debug.Log("Player " + playerData.PlayerID + " menu Cancel");
                if (playerData.IsReady)
                {
                    playerData.IsReady = false;
                    PlayerMenuNavigator playerMenuNav = playerData.PlayerGameObjectRef.GetComponent<PlayerMenuNavigator>();
                    playerMenuNav.SetReady(false);
                }
                else
                {
                    ExitPlayer(playerData.PlayerID);
                    break;
                }
            }
        }
    }

    private void TryStartGame()
    {
        if (_playerManager.Players.Count < minPlayers) return;
        foreach (PlayerData playerData in _playerManager.Players)
        {
            if (!playerData.IsReady) return;
        }
        SceneManager.LoadScene("Game", LoadSceneMode.Single);
    }

    public void ExitPlayer(int id)
    {
        ReInput.players.GetPlayer(id).controllers.ClearAllControllers();
        ReInput.players.GetPlayer(id).controllers.hasKeyboard = false;
        PlayerData playerData = _playerManager.Players.Find(p => p.PlayerID == id);
        Destroy(playerData.PlayerGameObjectRef);
        _playerManager.RemovePlayer(playerData);
    }

}

