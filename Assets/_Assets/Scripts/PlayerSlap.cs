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

        // Se o botão estiver pressionado, aumenta o tempo de carga
        if (Input.GetKey(keySlap) && isCharging)
        {
            chargingTime += Time.deltaTime;

        }

        if (Input.GetKeyUp(keySlap))
        {
            isCharging = false;

            if (chargingTime <= quickSlapThreshold && !isSlapping)
            {
                // Play immediate slap feedback (ou a animação correspondente ao quick slap)
                QuickSlap();
                isCharging = false;
                isSlapping = true;
            }
            else if (chargingTime > quickSlapThreshold && !isSlapping) // Caso contrário, é um charge slap
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
            animator.speed = 0;  // Pausa a animação
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
            // Retoma a animação ao soltar o botão  
            animator.speed = 1f;
            slapWhoosh.PlayFeedbacks();
            isCharging = false;
            isSlapping = true;
        }
    }

    void SlappingEnd()
    {
        isSlapping = false;
    }
}
