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

    [Inject]
    private PlayerManager _playerManager;
    void Start()
    {
        Debug.Log("Players Count: " + _playerManager.Players.Count);
        foreach (PlayerData player in _playerManager.Players)
        {
            GameObject newPlayer = Instantiate(playerPrefab,
                spawnPoints[player.PlayerID].transform.position,
                Quaternion.identity);
            newPlayer.name = player.PlayerName;
            RigidbodyController rbController = newPlayer.GetComponent<RigidbodyController>();
            rbController.SetPlayerId(player.PlayerID);
        }
    }
}
