using System.Collections.Generic;
using UnityEngine.Events;

public class GameEvent
{
    public UnityEvent<List<PlayerController>> onPlayersJoined = new();
    public UnityEvent onPlayerDeath = new();
    public UnityEvent onRoundStart = new();
    public UnityEvent<Team> onRoundEnd = new();
    public UnityEvent onSetupNextRound = new();
}
