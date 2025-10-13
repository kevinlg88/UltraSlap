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
    [SerializeField] private MMF_Player startMatchMMFPlayer;//Refer�ncia ao MMfeedback de carregamento de cena para que o nome da cena a ser carregada possa ser atualizada diretamente no feedback
    private MMF_LoadScene loadSceneFeedback; //Vari�vel que vai receber o feedback Load Scene do MMF_player startMatchMMFPlayer
    [SerializeField] private ValueSelector WinsGameObject;

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
    private MatchData _MatchData;
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
        // SystemPlayer � sempre v�lido e pega input global
        systemPlayer = ReInput.players.SystemPlayer;

        if (startMatchMMFPlayer != null)
        {
            loadSceneFeedback = startMatchMMFPlayer.GetFeedbackOfType<MMF_LoadScene>();
        }


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

            Debug.Log("Bot�o Join apertado (SystemPlayer)");
        }

        if (systemPlayer.GetButtonDown(cancelAction))
        {
            if (currentSetupState == SetupState.MatchSetup)
            {
                Debug.Log("Bot�o Cancel apertado (SystemPlayer), ir para player setup");
                ReturnToPlayerSetupMMFeedbacks.PlayFeedbacks();
            }
        }
    }

    public void SwitchToTransition()
    {
        currentSetupState = SetupState.Transition;

        // Desabilita navega��o de UI dos players
        TogglePlayerMenuNavigators(false);

        // Desativa navega��o de UI do Match Setup
        if (rewiredEventSystem != null)
        {
            rewiredEventSystem.sendNavigationEvents = false;
        }
    }

    public void SwitchToMatchSetup()
    {
        currentSetupState = SetupState.MatchSetup;

        // Desativa navega��o de UI do Match Setup
        if (rewiredEventSystem != null)
        {
            rewiredEventSystem.sendNavigationEvents = true;
        }
    }

    public void SwitchToCharacterSetup()
    {
        currentSetupState = SetupState.CharacterSetup;

        // Habilita navega��o de UI dos players
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

    public async void PrepareToStartGame()
    {
        DefineWins();

        _scoreManager.ResetScores();
        _scoreManager.SetTeams(_MatchData.GetTeams());
        //TODO: Colocar Loading Screen
        //await _levelSpawnManager.StartGame((int)currentLevel);
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
            Debug.LogWarning("Nome de n�vel inv�lido: " + levelName);
        }

        if (loadSceneFeedback != null)
        {
            loadSceneFeedback.DestinationSceneName = levelName;
            Debug.Log("DestinationSceneName atualizado no MMF Feedback -> " + levelName);
        }

    }

    public void DefineWins()  // Pega o n�mero de wins no objeto da interface que carrega essa informa��o, e joga no correspondente em MatchData
    {
        int definedWins;
        bool success = int.TryParse(WinsGameObject.GetCurrentValue(), out definedWins);


        if (success)
        {
            _MatchData.numberOfWins = definedWins;
            Debug.Log("N�mero convertido: " + _MatchData.numberOfWins);
        }
        else
        {
            Debug.Log("N�o foi poss�vel converter a string em n�mero.");
        }
    }

    public void AddRound() => _MatchData.numberOfWins++;
    public void SubtractRound()
    {
        if (_MatchData.numberOfWins > 0)
        {
            _MatchData.numberOfWins--;
        }
    }

}
