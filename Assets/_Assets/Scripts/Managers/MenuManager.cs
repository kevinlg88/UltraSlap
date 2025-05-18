using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Zenject;

public class MenuManager : MonoBehaviour
{
    private PlayerManager _playerManager;

    [Inject]
    public void Construct(PlayerManager playerManager)
    {
        _playerManager = playerManager;
        _playerManager.ClearPlayers();
        Debug.Log("MenuManager: PlayerManager cleared.");

    }
    void Start()
    {
        int gamepadCount = Gamepad.all.Count;
        Debug.Log("Gamepad count: " + gamepadCount);
    }

    void Update()
    {
        
    }
}
