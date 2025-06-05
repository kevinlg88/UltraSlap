using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Zenject;
using Zenject.SpaceFighter;

public class CurrentLevelManager : MonoBehaviour
{
    [SerializeField] List<GameObject> spawnPoints = new List<GameObject>();
    [SerializeField] GameObject playerPrefab;

    PlayerInputManager playerInputManager;

    [Inject]
    private PlayerManager _playerManager;

    private void Awake()
    {
        playerInputManager = GetComponent<PlayerInputManager>();
    }
    void Start()
    {

        int indexPlayer = 0;
        Debug.Log("Players Count: " + _playerManager.Players.Count);
        foreach (PlayerData player in _playerManager.Players)
        {

            PlayerInput newPlayerInput = PlayerInput.Instantiate(playerPrefab, 
                controlScheme: player.PlayerMovementControllerMap, 
                pairWithDevice: player.InputDevice, 
                playerIndex: indexPlayer);

            newPlayerInput.defaultActionMap = player.PlayerMovementControllerMap;
            newPlayerInput.SwitchCurrentActionMap(player.PlayerMovementControllerMap);

            newPlayerInput.gameObject.transform.SetPositionAndRotation(
                spawnPoints[indexPlayer].transform.position,
                Quaternion.identity
            );

            indexPlayer++;
        }
    }
}
