using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;
using Zenject.SpaceFighter;

public class CurrentLevelManager : MonoBehaviour
{
    [SerializeField] GameObject playerPrefab;

    [Inject]
    private PlayerManager _playerManager;
    // Start is called before the first frame update
    void Start()
    {
        Debug.Log("Players Count: " + _playerManager.Players.Count);
        foreach (var player in _playerManager.Players)
        {
            GameObject playerObject = Instantiate(playerPrefab);
            //TODO: Set player data to the playerObject
            playerObject.name = player.PlayerName;
        }
    }
}
