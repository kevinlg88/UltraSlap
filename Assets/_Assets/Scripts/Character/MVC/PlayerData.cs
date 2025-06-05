using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerData
{
    private CharacterVisualData _playervisual;

    private InputDevice inputDevice;

    private string _playerUiControllerMap;
    private string _playerMovementControllerMap;
    private string _playerName;
    private int _playerScore;
    private int _playerHealth;
    private int _playerMaxHealth;

    public InputDevice InputDevice
    {
        get { return inputDevice; }
        set { inputDevice = value; }
    }
    public string PlayerName
    {
        get { return _playerName; }
        set { _playerName = value; }
    }
    public CharacterVisualData PlayerVisual
    {
        get { return _playervisual; }
        set { _playervisual = value; }
    }
    public string PlayerUiControllerMap
    {
        get { return _playerUiControllerMap; }
        set { _playerUiControllerMap = value; }
    }
    public string PlayerMovementControllerMap
    {
        get { return _playerMovementControllerMap; }
        set { _playerMovementControllerMap = value; }
    }
    public int PlayerScore { get { return _playerScore; } set { _playerScore = value; } }
    public int PlayerHealth { get{ return _playerHealth; } set{ _playerHealth = value; } }
    public int PlayerMaxHealth { get{ return _playerMaxHealth; } set{ _playerMaxHealth = value; } }
}
