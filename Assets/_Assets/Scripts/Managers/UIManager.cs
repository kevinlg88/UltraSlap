using System.Collections.Generic;
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
    [SerializeField] private GameObject teamPrefab;
    [SerializeField] private GameObject scorePrefab;
    [SerializeField] private Button scoreContinueBttn;
    [Header("UI RoundTransition")]
    [SerializeField] private GameObject uiRoundTransitionMain;
    [SerializeField] private GameObject maskHandTransparent;
    [SerializeField] private GameObject handBlackScreen;

    [Header("UI RoundTransition Setup")]
    [SerializeField] private int delayStartTransition = 1000;
    [SerializeField] private Vector3 maskHandFinalScale;
    [SerializeField] private float animDuration;
    [SerializeField] Ease easeType;

    private List<Team> teams = new();
    private GameEvent _gameEvent;
    [Inject]
    public void Construct(GameEvent gameEvent)
    {
        _gameEvent = gameEvent;
        gameEvent.onPlayersJoined.AddListener(OnPlayersJoined);
        gameEvent.onRoundEnd.AddListener(OnRoundEnd);
    }

    private void OnPlayersJoined(List<PlayerController> players)
    {
        HashSet<Team> teams = new();
        foreach (PlayerController player in players)
        {
            if (player.PlayerData != null && !teams.Contains(player.PlayerData.Team))
            {
                teams.Add(player.PlayerData.Team);
            }
        }
        this.teams = new List<Team>(teams);
    }
    private async void OnRoundEnd(Team winnerTeam)
    {
        Debug.Log("ITS OVER!!!");
        await Task.Delay(2000);
        await StartMatchTransitionAnim();
        await SetTeamScore(winnerTeam);
        await Task.Delay(5000);
        _gameEvent.onSetupNextRound.Invoke();
        await FadeIn();
        await Task.Delay(1000);
        uiScoreMain.SetActive(false);
        await FadeOut();
        _gameEvent.onRoundStart.Invoke();
    }
    private async Task SetTeamScore(Team winnerTeam)
    {
        Debug.Log("SetTeamScore");
        uiScoreMain.SetActive(true);
        if (teamsField.transform.childCount == 0)
        {
            foreach (Team team in teams)
            {
                GameObject go = Instantiate(teamPrefab, teamsField.transform);
                go.GetComponent<Image>().color = team.Color;
                team.GameObjectUI = go;
                if (team == winnerTeam)
                {
                    team.Score += 1;
                    Instantiate(scorePrefab, go.transform);
                }
            }
        }
        else
        {
            foreach (Team team in teams)
            {
                if (team == winnerTeam)
                {
                    team.Score += 1;
                    Instantiate(scorePrefab, team.GameObjectUI.transform);
                }
            }
            //Check Win Match (Verificar scores dos times com score maximo de rounds)
        }
        await Task.CompletedTask;
    }

    #region ==== Slap Transition Anim ====

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


    #endregion
}