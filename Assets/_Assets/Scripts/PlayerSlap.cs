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

    [SerializeField] float minPower = 250;
    [SerializeField] float maxPower = 1500;     //Variáveis para a definição da intensidade do tapa, de acordo com o tempo de carregamento
    [SerializeField] float growthRate = 1.5f;
    [SerializeField] float midPoint = 2.5f;


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


        // Se o botão estiver pressionado, aumenta o tempo de carga
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

            TriggerGetRagBone.SetSlapPower(minPower + (maxPower - minPower) / (1 + Mathf.Exp(-growthRate * (chargingTime - midPoint)))); // Define o valor do Slap Power para o Slap Hit Box
            UnityEngine.Debug.Log(minPower + (maxPower - minPower) / (1 + Mathf.Exp(-growthRate * (chargingTime - midPoint))));

            if (chargingTime <= quickSlapThreshold && !isSlapping)
            {
               
                QuickSlap();

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
        chargingTime = 0f;
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
        chargingTime = 0f;
        isSlapping = false;
        isCharging = false;
    }
}
