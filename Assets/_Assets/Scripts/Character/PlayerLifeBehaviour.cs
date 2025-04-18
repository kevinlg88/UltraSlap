using FIMSpace.FProceduralAnimation;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerLifeBehaviour : MonoBehaviour
{
    [Header("Player Life")]
    [SerializeField] int playerLife;
    [SerializeField] float timeToHeal;
    [SerializeField] int healingAmount;

    [Space(5)]

    [Header("Debug")]
    [SerializeField] int currentPlayerLife;
    [SerializeField] float currentTimerecovery;

    RagdollAnimator2 ragdoll;
    RigidbodyController controller;
    

    private void Awake()
    {
        ragdoll = GetComponent<RagdollAnimator2>();
        controller = GetComponent<RigidbodyController>();
    }
    
    void Start()
    {
        //Subscribe in event player state (Get Hit) 
        currentPlayerLife = playerLife;
    }

    void Update()
    {
        //RagdollHandler.EAnimatingMode.Sleep;
        var x = ragdoll.GetRagdollHandler.AnimatingMode;
        if (ragdoll.IsInFallingOrSleepMode)
        {
            //Verify if player input 5 presses = reduce +0.5 seconds currentTimerecovery
        }
        else 
        {
            if(currentTimerecovery >= timeToHeal)
            {
                StartCoroutine(Healing());
            }
            else
            {
                StopCoroutine(Healing());
            }
        }
    }

    IEnumerator Healing()
    {
        if (currentPlayerLife < playerLife)
        {
            currentPlayerLife += healingAmount;
            if (currentPlayerLife > playerLife) currentPlayerLife = playerLife;
        }       
        yield return new WaitForSeconds(1f);
    }
}
