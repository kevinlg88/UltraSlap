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
            if (score.team.TeamEnum == team.TeamEnum)
            {
                score.score += 1;
                Debug.Log($"Team {team.TeamEnum} scored! New score: {score.score}");
                return;
            }
        }
    }
    public List<Team> GetAllTeamsInMatch()
    {
        List<Team> teamsInMatch = new();
        foreach (ScoreData score in scores)
        {
            teamsInMatch.Add(score.team);
        }
        return teamsInMatch;
    }
    public int GetScoreByTeam(Team team)
    {
        foreach (ScoreData score in scores)
        {
            if (score.team.TeamEnum == team.TeamEnum) return score.score;
        }
        return 0;
    }
    public List<ScoreData> GetListScores() => scores;
    public void ResetScores() => scores = new();
}
