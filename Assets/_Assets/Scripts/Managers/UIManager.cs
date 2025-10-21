using System.Collections.Generic;
using MoreMountains.Feedbacks;
using System.Threading.Tasks;
using UnityEngine;
using Zenject;
using TMPro;
using Unity.VisualScripting;

public class UIManager : MonoBehaviour
{
    [Header("Score UI References")]
    [SerializeField] private TextMeshProUGUI team1ScoreText;
    [SerializeField] private TextMeshProUGUI team2ScoreText;
    [SerializeField] private TextMeshProUGUI team3ScoreText;
    [SerializeField] private TextMeshProUGUI team4ScoreText;
    [SerializeField] private TextMeshProUGUI team5ScoreText;
    [SerializeField] private TextMeshProUGUI team6ScoreText;
    [SerializeField] private TextMeshProUGUI team7ScoreText;
    [SerializeField] private TextMeshProUGUI team8ScoreText;

    [Header("UI RoundTransition References")]
    [SerializeField] private MMFeedbacks callRoundStatistics; //Game Object com o MMF Player que dá start na transição de rounds
    [SerializeField] private MMFeedbacks RoundTransition; //Game Object com o MMF Player que inicia o novo round
    [SerializeField] private GameObject ContinueButton; //Referência para o gameObject do botão "Continue" Só deve permitir passar para o próximo round se esse game object estiver ativo.

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


    private ScoreManager _scoreManager;
    private Team winningTeam;

    [Inject]
    public void Construct(GameEvent gameEvent, ScoreManager scoreManager)
    {
        _scoreManager = scoreManager;
        gameEvent.onRoundEnd.AddListener(OnRoundEnd);
    }
    private async void OnRoundEnd(Team winnerTeam)
    {
        Debug.Log("ITS OVER!!!");
        winningTeam = winnerTeam;
        await Task.Delay(1000);
        await StartMatchTransitionAnim();
    }

    private void UpdateScoreUI()
    {
        List<Team> activeTeams = _scoreManager.GetAllTeamsInMatch();
        Debug.Log($"Active Teams Count: {activeTeams.Count}");
        // Ativa as tags correspondentes
        foreach (Team team in activeTeams)
        {
            switch (team.TeamEnum)
            {
                case TeamEnum.Team1:
                    team1ScoreText.text = _scoreManager.GetScoreByTeam(team).ToString();
                    Debug.Log($"Team 1 Score Updated: {_scoreManager.GetScoreByTeam(team)}");
                    break;
                case TeamEnum.Team2:
                    team2ScoreText.text = _scoreManager.GetScoreByTeam(team).ToString();
                    Debug.Log($"Team 2 Score Updated: {_scoreManager.GetScoreByTeam(team)}");
                    break;
                case TeamEnum.Team3:
                    team3ScoreText.text = _scoreManager.GetScoreByTeam(team).ToString();
                    Debug.Log($"Team 3 Score Updated: {_scoreManager.GetScoreByTeam(team)}");
                    break;
                case TeamEnum.Team4:
                    team4ScoreText.text = _scoreManager.GetScoreByTeam(team).ToString();
                    Debug.Log($"Team 4 Score Updated: {_scoreManager.GetScoreByTeam(team)}");
                    break;
                case TeamEnum.Team5:
                    team5ScoreText.text = _scoreManager.GetScoreByTeam(team).ToString();
                    Debug.Log($"Team 5 Score Updated: {_scoreManager.GetScoreByTeam(team)}");
                    break;
                case TeamEnum.Team6:
                    team6ScoreText.text = _scoreManager.GetScoreByTeam(team).ToString();
                    Debug.Log($"Team 6 Score Updated: {_scoreManager.GetScoreByTeam(team)}");
                    break;
                case TeamEnum.Team7:
                    team7ScoreText.text = _scoreManager.GetScoreByTeam(team).ToString();
                    Debug.Log($"Team 7 Score Updated: {_scoreManager.GetScoreByTeam(team)}");
                    break;
                case TeamEnum.Team8:
                    team8ScoreText.text = _scoreManager.GetScoreByTeam(team).ToString();
                    Debug.Log($"Team 8 Score Updated: {_scoreManager.GetScoreByTeam(team)}");
                    break;
            }
        }
    }
    

    private void AddWinnerPoint(Team winnerTeam)
    {
        _scoreManager.AddScore(winnerTeam);
        UpdateScoreUI();
    }
   

    /*public void AddWinnerPoint()
    {
        Debug.Log($"O time campeão foi: {winningTeam}");
        _scoreManager.AddScore(winningTeam);
        UpdateScoreUI();
    }*/

    private async Task StartMatchTransitionAnim()
    {
        Debug.Log("Começou transição");
        UpdateScoreUI();
        // Pega os times ativos
        List<Team> activeTeams = _scoreManager.GetAllTeamsInMatch();

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

        callRoundStatistics.PlayFeedbacks();
    }

    public void WinnerUIAnim()
    {
        Debug.Log("Começou a transição de winner UI");
        AddWinnerPoint(winningTeam);

        switch (winningTeam.TeamEnum)
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

    public void Unpause()
    {
        Time.timeScale = 1;
    }
}