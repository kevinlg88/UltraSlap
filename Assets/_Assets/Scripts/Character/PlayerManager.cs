using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{

    private LevelManager levelManager;
    public int playerIndex;
    public int team;

    [Header("body parts to set up materials")]
    [SerializeField] private Renderer leftArm;
    [SerializeField] private Renderer rightArm;
    [SerializeField] private Renderer body;
    [SerializeField] private Renderer hat;

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

    public void ApplyTeamMaterial(Material material)
    {
        leftArm.material = material;
        rightArm.material = material;
        body.material = material;
        hat.material = material;
    }
}
