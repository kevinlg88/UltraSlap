using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerData
{

    private int _playerID = -1;
    private string _playerName = "Undefined";
    private bool _isReady = false;
    private Team _team = null;
    private int _playerHealth = 0;
    private int _playerMaxHealth = 0;
    private int _playerScore = 0;
    private GameObject _playerGameObjectRef = null;
    private CharacterVisualData _playervisual = null;

    public int PlayerID {get=>_playerID; set=>_playerID = value;}
    public string PlayerName {get=>_playerName; set=>_playerName = value;}
    public bool IsReady {get=>_isReady; set =>_isReady = value;}
    public Team Team { get => _team; set => _team = value;}
    public int PlayerHealth { get => _playerHealth; set => _playerHealth = value; }
    public int PlayerMaxHealth {get=>_playerMaxHealth; set=>_playerMaxHealth = value;}
    public int PlayerScore {get=>_playerScore; set=>_playerScore = value;}
    public GameObject PlayerGameObjectRef {get=>_playerGameObjectRef; set=>_playerGameObjectRef = value;}
    public CharacterVisualData PlayerVisual {get=>_playervisual; set=>_playervisual = value;}
}
