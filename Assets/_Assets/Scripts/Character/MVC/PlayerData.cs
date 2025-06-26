using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerData
{

    private int _playerID;
    private string _playerName;
    private bool _isReady;
    private int _playerHealth;
    private int _playerMaxHealth;
    private int _playerScore;
    private GameObject _playerGameObjectRef;
    private CharacterVisualData _playervisual;

    public int PlayerID { get { return _playerID; } set { _playerID = value; } }
    public string PlayerName { get { return _playerName; } set { _playerName = value; } }
    public bool IsReady { get { return _isReady; } set { _isReady = value; } }
    public int PlayerHealth { get { return _playerHealth; } set { _playerHealth = value; } }
    public int PlayerMaxHealth { get { return _playerMaxHealth; } set { _playerMaxHealth = value; } }
    public int PlayerScore { get { return _playerScore; } set { _playerScore = value; } }
    public GameObject PlayerGameObjectRef { get { return _playerGameObjectRef; } set { _playerGameObjectRef = value; } }
    public CharacterVisualData PlayerVisual { get { return _playervisual; } set { _playervisual = value; } }
}
