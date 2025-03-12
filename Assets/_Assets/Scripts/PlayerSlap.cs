using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MoreMountains.Feedbacks;
using System.Diagnostics;

public class PlayerSlap : MonoBehaviour
{
    [SerializeField] KeyCode keySlap;
    [SerializeField] private MMFeedbacks slapWhoosh;
    Animator animator;

    [SerializeField] bool isCharging = false;
    [SerializeField] bool isSlapping = false;
    [SerializeField] bool buttonReleasedBeforeKeyframe = false;
    [SerializeField] float chargingTime = 0f;

    [SerializeField] private float quickSlapThreshold = 0.1f;
    [SerializeField] private float maxSlapThreshold = 5f;

    [SerializeField] TriggerGetRagBone TriggerGetRagBone;


    private void Start()
    {
        animator = GetComponent<Animator>();

    }
    void Update()
    {


        if (Input.GetKeyDown(keySlap) && !isCharging && !isSlapping) 
        {
            chargingTime = 0f;
            isCharging = true;

            animator.SetTrigger("triggerTapa");
        }


        // Se o bot�o estiver pressionado, aumenta o tempo de carga
        else if (Input.GetKey(keySlap) && isCharging)
        {
            chargingTime += Time.deltaTime;

        }

        else if (!Input.GetKeyDown(keySlap) && isSlapping && !animator.GetCurrentAnimatorStateInfo(0).IsTag("Slap"))
        {
            isSlapping = false;
        }


        if (Input.GetKeyUp(keySlap))
        {
            isCharging = false;

            TriggerGetRagBone.SetSlapPower(chargingTime*200); // Define o valor do Slap Power para o Slap Hit Box

            if (chargingTime <= quickSlapThreshold && !isSlapping)
            {
               
                QuickSlap();

            }
            else if (chargingTime > quickSlapThreshold && !isSlapping) // Caso contr�rio, � um charge slap
            {
                ChargedSlap();
            }

            chargingTime = 0f;
        }


    }

    public void PauseChargeAnimation()
    {
        if (isCharging)
        {
            animator.speed = 0;  // Pausa a anima��o
        }
    }

    void QuickSlap()
    {
        animator.speed = 1f;
        slapWhoosh.PlayFeedbacks();
        isCharging = false;
        isSlapping = true;
    }

    void ChargedSlap()
    {
        if (animator.speed == 0f && !isCharging)
        {
            // Retoma a anima��o ao soltar o bot�o  
            animator.speed = 1f;
            slapWhoosh.PlayFeedbacks();
            isCharging = false;
            isSlapping = true;
        }
    }

    void SlappingEnd()
    {
        chargingTime = 0f;
        isSlapping = false;
        isCharging = false;
    }
}
