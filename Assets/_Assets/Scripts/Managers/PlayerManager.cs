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

    public void ClearPlayers()
    {
        Players.Clear();
    }
}
