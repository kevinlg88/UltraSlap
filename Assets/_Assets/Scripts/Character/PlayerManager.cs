using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{

    private LevelManager levelManager;
    [SerializeField] private int playerIndex;

    // Start is called before the first frame update
    void Start()
    {

        
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    void OnDestroy()
    {
        levelManager = FindObjectOfType<LevelManager>();
        if(levelManager != null )
            levelManager.roundVictory(playerIndex); //Verifica se o match acabou
    }
}
