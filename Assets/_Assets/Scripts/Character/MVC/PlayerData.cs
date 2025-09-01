using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerData
{

    private int _playerID;
    private string _playerName;
    private bool _isReady;
    private TeamEnum _team;
    private int _playerHealth;
    private int _playerMaxHealth;
    private int _playerScore;
    private GameObject _playerGameObjectRef;
    private CharacterVisualData _playervisual;

    public int PlayerID {get=>_playerID; set=>_playerID = value;}
    public string PlayerName {get=>_playerName; set=>_playerName = value;}
    public bool IsReady {get=>_isReady; set =>_isReady = value;}
    public TeamEnum Team { get => _team; set => _team = value;}
    public int PlayerHealth { get => _playerHealth; set => _playerHealth = value; }
    public int PlayerMaxHealth {get=>_playerMaxHealth; set=>_playerMaxHealth = value;}
    public int PlayerScore {get=>_playerScore; set=>_playerScore = value;}
    public GameObject PlayerGameObjectRef {get=>_playerGameObjectRef; set=>_playerGameObjectRef = value;}
    public CharacterVisualData PlayerVisual {get=>_playervisual; set=>_playervisual = value;}
}
