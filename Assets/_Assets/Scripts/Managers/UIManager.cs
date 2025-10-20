using System.Collections.Generic;
using MoreMountains.Feedbacks;
using System.Threading.Tasks;
using DG.Tweening;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Zenject;

public class UIManager : MonoBehaviour
{
    [Header("UI Score")]
    [SerializeField] private GameObject uiScoreMain;
    [SerializeField] private GameObject teamsField;

    [Header("UI RoundTransition")]
    [SerializeField] private GameObject uiRoundTransitionMain;
    [SerializeField] private GameObject maskHandTransparent;
    [SerializeField] private GameObject handBlackScreen;

    [Header("UI RoundTransition Setup")]
    [SerializeField] private int delayStartTransition = 1000;
    [SerializeField] private Vector3 maskHandFinalScale;
    [SerializeField] private float animDuration;
    [SerializeField] Ease easeType;
    [SerializeField] private LevelManager levelManager; // para poder acessar informações dos times na partida


    [Header("UI RoundTransition References")]
    [SerializeField] private MMFeedbacks roundTransitionIn; //Game Object com o MMF Player que dá start na transição de rounds
    [SerializeField] private GameObject orangeTeamTag;
    [SerializeField] private GameObject brownTeamTag;
    [SerializeField] private GameObject pinkTeamTag;
    [SerializeField] private GameObject blueTeamTag;   //Tags dos teams, que deverão ser habilitadas de acordo com as cores de times presentes na partida
    [SerializeField] private GameObject greenTeamTag;
    [SerializeField] private GameObject yellowTeamTag;
    [SerializeField] private GameObject purpleTeamTag;
    [SerializeField] private GameObject redTeamTag;

    [Header("Slap Winner References")]
    [SerializeField] private GameObject orangeWinner;
    [SerializeField] private GameObject brownWinner;
    [SerializeField] private GameObject pinkWinner;
    [SerializeField] private GameObject blueWinner;   //Referências para o efeito de vitória de cada um dos times
    [SerializeField] private GameObject greenWinner;
    [SerializeField] private GameObject yellowWinner;
    [SerializeField] private GameObject purpleWinner;
    [SerializeField] private GameObject redWinner;


    private GameEvent _gameEvent;
    private ScoreManager _scoreManager;
    [Inject]
    public void Construct(GameEvent gameEvent, ScoreManager scoreManager)
    {
        _gameEvent = gameEvent;
        _scoreManager = scoreManager;
        gameEvent.onRoundEnd.AddListener(OnRoundEnd);
    }
    private async void OnRoundEnd(Team winnerTeam)
    {
        Debug.Log("ITS OVER!!!");
        await Task.Delay(2000);
        await StartMatchTransitionAnim();
        await SetTeamScore(winnerTeam);
        //await Task.Delay(5000);
        //await FadeIn();
        //await Task.Delay(1000);
        //uiScoreMain.SetActive(false);
        //await FadeOut();
        //_gameEvent.onRoundRestart.Invoke();
    }
    private async Task SetTeamScore(Team winnerTeam)
    {
        Debug.Log("SetTeamScore");
        /*
        //uiScoreMain.SetActive(true);
        _scoreManager.AddScore(winnerTeam);

        //Check Win Match (Verificar scores dos times com score maximo de rounds)
        await Task.CompletedTask;
        */
    }

    #region ==== Slap Transition Anim ====


    private async Task StartMatchTransitionAnim()
    {
        Debug.Log("Começou transição");

        // Pega os times ativos
        List<Team> activeTeams = levelManager.GetAllTeamsInMatch();

        // Ativa as tags correspondentes
        foreach (Team team in activeTeams)
        {
            switch (team.TeamEnum)
            {
                case TeamEnum.Team1: blueTeamTag.SetActive(true); break;
                case TeamEnum.Team2: yellowTeamTag.SetActive(true); break;
                case TeamEnum.Team3: greenTeamTag.SetActive(true); break;
                case TeamEnum.Team4: redTeamTag.SetActive(true); break;
                case TeamEnum.Team5: pinkTeamTag.SetActive(true); break;
                case TeamEnum.Team6: purpleTeamTag.SetActive(true); break;
                case TeamEnum.Team7: brownTeamTag.SetActive(true); break;
                case TeamEnum.Team8: orangeTeamTag.SetActive(true); break;
            }
        }

        //await Task.CompletedTask;


        roundTransitionIn.PlayFeedbacks();
    }


    public void WinnerUIAnim()
    {
        Debug.Log("Começou a transição de winner UI");

        // Pega os times ativos
        List<Team> activeTeams = levelManager.GetTeamsInGame();

        // Ativa as tags correspondentes
        foreach (Team team in activeTeams)
        {
            switch (team.TeamEnum)
            {
                case TeamEnum.Team1: blueWinner.SetActive(true); break;
                case TeamEnum.Team2: yellowWinner.SetActive(true); break;
                case TeamEnum.Team3: greenWinner.SetActive(true); break;
                case TeamEnum.Team4: redWinner.SetActive(true); break;
                case TeamEnum.Team5: pinkWinner.SetActive(true); break;
                case TeamEnum.Team6: purpleWinner.SetActive(true); break;
                case TeamEnum.Team7: brownWinner.SetActive(true); break;
                case TeamEnum.Team8: orangeWinner.SetActive(true); break;
            }
        }

        //await Task.CompletedTask;


        //roundTransitionIn.PlayFeedbacks();
    }

    /*
    private async Task FadeIn()
    {
        uiRoundTransitionMain.SetActive(true);
        await Task.Delay(delayStartTransition);
        await HandBlackScreenTransitionAnim();
    }
    private async Task FadeOut()
    {
        uiRoundTransitionMain.SetActive(true);
        await Task.Delay(delayStartTransition);
        await MaskScreenTransitionAnim();
    }
    private async Task StartMatchTransitionAnim()
    {
        Debug.Log("Começou transição");
        uiRoundTransitionMain.SetActive(true);
        await Task.Delay(delayStartTransition);
        await HandBlackScreenTransitionAnim();
        uiScoreMain.SetActive(true);
        await MaskScreenTransitionAnim();
    }
    private async Task HandBlackScreenTransitionAnim()
    {
        await handBlackScreen.transform
            .DOScale(maskHandFinalScale, animDuration)
            .SetEase(easeType)
            .SetUpdate(true)
            .AsyncWaitForCompletion();
    }

    private async Task MaskScreenTransitionAnim()
    {
        maskHandTransparent.SetActive(true);

        await maskHandTransparent.transform
            .DOScale(maskHandFinalScale, animDuration)
            .SetEase(easeType)
            .SetUpdate(true)
            .AsyncWaitForCompletion();

        maskHandTransparent.transform.localScale = Vector3.zero;
        handBlackScreen.transform.localScale = Vector3.zero;

        maskHandTransparent.SetActive(false);
        uiRoundTransitionMain.SetActive(false);

        Debug.Log("Terminou");
    }
    */

    #endregion
}