using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerData
{
    private GameObject _player;
    private string _playerName;
    private int _playerScore;
    private int _playerHealth;
    private int _playerMaxHealth;

    public string PlayerName
    {
        get { return _playerName; }
        set{ _playerName = value; }
    }
    public GameObject Player
    {
        get { return _player; }
        set { _player = value; }
    }
    public int PlayerScore { get { return _playerScore; } set { _playerScore = value; } }
    public int PlayerHealth { get{ return _playerHealth; } set{ _playerHealth = value; } }
    public int PlayerMaxHealth { get{ return _playerMaxHealth; } set{ _playerMaxHealth = value; } }
}
