using System.Collections.Generic;
using UnityEngine.Events;

public class GameEvent
{
    public UnityEvent<List<PlayerController>> onPlayersJoined = new();
    public UnityEvent onPlayerDeath = new();
    public UnityEvent onRoundRestart = new();
    public UnityEvent<Team> onRoundEnd = new();
}
