using System.Collections.Generic;
using UnityEngine;
using Zenject;
using MoreMountains.Feedbacks;
using System.Threading.Tasks;

public class LevelManager : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private MMFeedbacks levelSong;
    [SerializeField] List<GameObject> spawnPoints = new();
    [SerializeField] GameObject playerPrefab;

    [Header("Setup")]
    [SerializeField] Vector3 spawnOffset = new(0, 0.34f, 0);
    private List<PlayerController> playersInGame = new();
    private PlayerManager _playerManager;
    private GameEvent _gameEvent;
    private LevelSpawnManager _levelSpawnManager;


    [Inject]
    public void Construct(PlayerManager playerManager, LevelSpawnManager levelSpawnManager, GameEvent gameEvent)
    {
        _playerManager = playerManager;
        _gameEvent = gameEvent;
        _levelSpawnManager = levelSpawnManager;
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
        _gameEvent.onPlayerDeath.AddListener(OnPlayerDeath);
        _gameEvent.onRoundRestart.AddListener(OnRoundRestart);

        _gameEvent.onPlayersJoined.Invoke(playersInGame);
    }

    void Start()
    {
        levelSong.PlayFeedbacks(); //TODO: Create a Audio manager for this
    }
    private async void OnRoundRestart()
    {
        await _levelSpawnManager.StartGame((int)SceneIndexEnum.ConstructionLevel);
    }
    private void OnPlayerDeath()
    {
        PlayerController player = playersInGame.Find(dead => dead.IsDead);
        player.gameObject.transform.position = new Vector3(0, 1000, 0);
        CheckIfRoundIsOver();
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
            if (player.PlayerData != null && !player.IsDead && !teams.Contains(player.PlayerData.Team))
            {
                teams.Add(player.PlayerData.Team);
            }
        }
        return new List<Team>(teams);
    }
}
