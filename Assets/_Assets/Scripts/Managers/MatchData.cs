using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Zenject;

public class MatchData
{
    public List<PlayerData> Players { get; private set; } = new List<PlayerData>();
    public int RoundNumber { get; set; } = 0;

    public void AddPlayer(PlayerData playerData)
    {
        Players.Add(playerData);
    }

    public void RemovePlayer(PlayerData playerData)
    {
        if (Players.Contains(playerData))
        {
            Players.Remove(playerData);
        }
    }
    public void ClearPlayers()
    {
        Players.Clear();
    }

    public List<Team> GetTeams()
    {
        return Players
            .Where(p => p.Team != null)
            .GroupBy(p => p.Team.TeamEnum)
            .Select(g => g.First().Team)
            .ToList();
    }
}
