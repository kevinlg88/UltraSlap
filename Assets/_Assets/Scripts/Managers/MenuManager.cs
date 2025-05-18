using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class MenuManager : MonoBehaviour
{
    [Inject]
    private PlayerManager _playerManager;
    // Start is called before the first frame update
    void Start()
    {
        _playerManager.ClearPlayers();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
