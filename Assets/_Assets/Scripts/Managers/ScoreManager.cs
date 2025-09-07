using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class ScoreManager
{
    private List<ScoreData> scores = new();

    public void SetTeams(List<Team> teams)
    {
        foreach (Team team in teams)
        {
            ScoreData scoreData = new();
            scoreData.team = team;
            scores.Add(scoreData);
        }
    }
    public void AddScore(Team team)
    {
        foreach (ScoreData score in scores)
        {
            if (score.team == team)
            {
                score.score += 1;
                return;
            }
        }
        Debug.LogError($"Team {team.TeamEnum.DisplayName()} not setted");
    }
    public int GetScoreByTeam(Team team)
    {
        foreach (ScoreData score in scores)
        {
            if (score.team == team) return score.score;
        }
        return 0;
    }
    public List<ScoreData> GetListScores() => scores;
    public void ResetScores() => scores = new();
}
