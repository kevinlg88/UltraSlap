using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class PlayerManager
{
    public List<PlayerData> Players { get; private set; } = new List<PlayerData>();

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

    public List<TeamEnum> GetTeams()
    {
        HashSet<TeamEnum> teams = new HashSet<TeamEnum>();
        foreach (PlayerData player in Players)
        {
            if(!teams.Contains(player.Team))
                teams.Add(player.Team);
        }
        return new List<TeamEnum>(teams);
    }
}
