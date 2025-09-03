using System.Collections.Generic;
using UnityEngine;
using Zenject;
using MoreMountains.Feedbacks;
using FIMSpace.FProceduralAnimation;

public class LevelManager : MonoBehaviour
{
    [SerializeField] List<GameObject> spawnPoints = new();
    [SerializeField] GameObject playerPrefab;
    [SerializeField] private MMFeedbacks levelSong;

    private List<PlayerController> playersInGame = new();


    private PlayerManager _playerManager;
    private GameEvent _gameEvent;



    [Inject]
    public void Construct(PlayerManager playerManager, GameEvent gameEvent)
    {
        _playerManager = playerManager;
        _gameEvent = gameEvent;
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

        //Events
        _gameEvent.onRoundStart.AddListener(OnRoundStart);
        _gameEvent.onSetupNextRound.AddListener(OnSetupNextRound);
        _gameEvent.onPlayerDeath.AddListener(OnPlayerDeath);

        _gameEvent.onPlayersJoined.Invoke(playersInGame);
    }

    void Start()
    {
        levelSong.PlayFeedbacks(); //Create a Audio manager for this
    }
    private void OnRoundStart() => Time.timeScale = 1;
    private void OnPlayerDeath()
    {
        PlayerController player = playersInGame.Find(dead => dead.IsDead);
        Debug.Log($"player {player} morreu");
        player.gameObject.SetActive(false);
        CheckIfRoundIsOver();
    }
    private void OnSetupNextRound()
    {
        foreach (PlayerController player in playersInGame)
        {
            player.gameObject.transform.position = spawnPoints[player.PlayerData.PlayerID].transform.position;
            player.gameObject.SetActive(true);
        }
    }
    private void CheckIfRoundIsOver()
    {
        List<Team> teams = GetTeamsInGame();
        if (teams.Count <= 1)
        {
            Time.timeScale = 0;
            _gameEvent.onRoundEnd.Invoke(teams[0]);
        }
    }
    private List<Team> GetTeamsInGame()
    {
        HashSet<Team> teams = new();
        foreach (PlayerController player in playersInGame)
        {
            if (player.PlayerData != null && player.gameObject.activeSelf && !teams.Contains(player.PlayerData.Team))
            {
                teams.Add(player.PlayerData.Team);
            }
        }
        return new List<Team>(teams);
    }
}
