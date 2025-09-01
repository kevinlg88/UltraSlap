using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using Zenject;
using Zenject.SpaceFighter;
using MoreMountains.Feedbacks;

public class CurrentLevelManager : MonoBehaviour
{
    [SerializeField] List<GameObject> spawnPoints = new List<GameObject>();
    [SerializeField] GameObject playerPrefab;
    [SerializeField] private MMFeedbacks levelSong;

    private List<PlayerController> playersInGame = new List<PlayerController>();


    private PlayerManager _playerManager;
    private GameEvent _gameEvent;



    [Inject]
    public void Construct(PlayerManager playerManager, GameEvent gameEvent)
    {
        _playerManager = playerManager;
        _gameEvent = gameEvent;
        Debug.Log("Players Count: " + _playerManager.Players.Count);
        foreach (PlayerData player in _playerManager.Players)
        {
            GameObject newPlayer = Instantiate(playerPrefab,
                spawnPoints[player.PlayerID].transform.position,
                Quaternion.identity);
            newPlayer.name = player.PlayerName;
            //Adiciona o player movement
            RigidbodyController rbController = newPlayer.GetComponent<RigidbodyController>();
            rbController.SetPlayerId(player.PlayerID);

            //Adiciona a customização do player
            PlayerCustomization playerCustomization = newPlayer.GetComponent<PlayerCustomization>();
            playerCustomization.LoadCharacterVisual(player);

            PlayerController playerController = newPlayer.GetComponent<PlayerController>();
            playerController.PlayerData = player;
            playerController.PlayerMovement = rbController;
            playerController.PlayerCustomization = playerCustomization;
            playerController._gameEvent = gameEvent;
            playersInGame.Add(playerController);
        }

        //Suscribe Events
        _gameEvent.onPlayerDeath.AddListener(OnPlayerDeath);
    }

    void Start()
    {
        startMatch();
    }

    public void startMatch()
    {
        levelSong.PlayFeedbacks(); // Toca música do level
    }

    private void OnPlayerDeath()
    {
        PlayerController player = playersInGame.Find(dead => dead.IsDead);
        //Debug.Log($"Player {player.name} morreu");
        player.gameObject.SetActive(false);
    }
}
