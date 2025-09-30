using UnityEngine;
using MoreMountains.Feedbacks;
using Rewired.Integration.UnityUI;
using Rewired;
using Zenject;
using UnityEngine.SceneManagement;
using System.Threading.Tasks;
using System;
using UnityEngine.EventSystems;
using TMPro;


public enum SetupState
{
    CharacterSetup,
    MatchSetup,
    Transition
}

public class MatchSetupManager : MonoBehaviour
{
    public SetupState currentSetupState = SetupState.CharacterSetup;

    private Player systemPlayer;

    [Header("Start Game Setup")]
    [SerializeField] SceneIndexEnum currentLevel;

    [Header("UI References")]
    [SerializeField] private RewiredEventSystem rewiredEventSystem;

    [Header("Input Setup")]
    [SerializeField] string joinAction = "Join";
    [SerializeField] string cancelAction = "Cancel";

    [Header("MMFeedbacks")]
    [SerializeField] private MMFeedbacks ReturnToPlayerSetupMMFeedbacks;

    [Header("First Selected Options")]
    [SerializeField] private GameObject startingCharacterSetupFirst;
    [SerializeField] private GameObject startingMatchSetupFirst;

    [Inject]
    private PlayerManager _playerManager;
    private ScoreManager _scoreManager;
    private LevelSpawnManager _levelSpawnManager;

    [Inject]
    public void Construct(ScoreManager scoreManager, LevelSpawnManager levelSpawnManager)
    {
        _scoreManager = scoreManager;
        _levelSpawnManager = levelSpawnManager;
    }


    // Start is called before the first frame update
    void Start()
    {
        // SystemPlayer é sempre válido e pega input global
        systemPlayer = ReInput.players.SystemPlayer;

    }


    // Update is called once per frame
    void Update()
    {
        if (currentSetupState == SetupState.MatchSetup)
        {
            GetPlayerInput();

            if (EventSystem.current.currentSelectedGameObject != null)
            {
                Debug.Log("Selecionado: " + rewiredEventSystem.currentSelectedGameObject.name);
            }
            else
            {
                Debug.Log("Nada selecionado");
            }
        }

    }

    private void GetPlayerInput()
    {
        if (systemPlayer.GetButtonDown(joinAction))
        {

            Debug.Log("Botão Join apertado (SystemPlayer)");
        }

        if (systemPlayer.GetButtonDown(cancelAction))
        {
            if (currentSetupState == SetupState.MatchSetup)
            {
                Debug.Log("Botão Cancel apertado (SystemPlayer), ir para player setup");
                ReturnToPlayerSetupMMFeedbacks.PlayFeedbacks();
            }
        }
    }

    public void SwitchToTransition()
    {
        currentSetupState = SetupState.Transition;

        // Desabilita navegação de UI dos players
        TogglePlayerMenuNavigators(false);

        // Desativa navegação de UI do Match Setup
        if (rewiredEventSystem != null)
        {
            rewiredEventSystem.sendNavigationEvents = false;
        }
    }

    public void SwitchToMatchSetup()
    {
        currentSetupState = SetupState.MatchSetup;

        // Desativa navegação de UI do Match Setup
        if (rewiredEventSystem != null)
        {
            rewiredEventSystem.sendNavigationEvents = true;
        }
    }

    public void SwitchToCharacterSetup()
    {
        currentSetupState = SetupState.CharacterSetup;

        // Habilita navegação de UI dos players
        TogglePlayerMenuNavigators(true);

    }


    private void TogglePlayerMenuNavigators(bool enable)
    {
        PlayerMenuNavigator[] navigators = FindObjectsOfType<PlayerMenuNavigator>();
        foreach (var nav in navigators)
        {
            nav.enabled = enable;
        }
    }

    public async void TryStartGame()
    {
        _scoreManager.ResetScores();
        _scoreManager.SetTeams(_playerManager.GetTeams());
        await _levelSpawnManager.StartGame((int)currentLevel);
    }

    public void UpdateCurrentLevel(string levelName)
    {
        // Tenta converter o string para o enum correspondente
        if (Enum.TryParse<SceneIndexEnum>(levelName, out var parsedLevel))
        {
            currentLevel = parsedLevel;
            Debug.Log("Current Level atualizado para -> " + currentLevel);
        }
        else
        {
            Debug.LogWarning("Nome de nível inválido: " + levelName);
        }
    }

}
