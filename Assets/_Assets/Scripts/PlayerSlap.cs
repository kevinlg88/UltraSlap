using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MoreMountains.Feedbacks;
using System.Diagnostics;
using System.Runtime;

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
    [SerializeField] float maxPower = 1500;
    [SerializeField] float power;               //Variáveis para a definição da intensidade do tapa, de acordo com o tempo de carregamento
    [SerializeField] float growthRate = 1.5f;
    [SerializeField] float midPoint = 2.5f;

    [SerializeField] float minCooldown = 0.1f;
    [SerializeField] float maxCooldown = 0.8f;
    [SerializeField] bool isOnCooldown = false;  //Definição e controle de slap cooldown
    [SerializeField] float cooldownTimer = 0f;
    [SerializeField] float cooldownGrowthRate = 0.001f;
    [SerializeField] float cooldown;

    [SerializeField] private float quickSlapThreshold = 0.1f;
    [SerializeField] private float maxSlapThreshold = 5f;

    [SerializeField] TriggerGetRagBone TriggerGetRagBone;


    private void Start()
    {
        animator = GetComponent<Animator>();

    }
    void Update()
    {


        if (Input.GetKeyDown(keySlap) && !isCharging && !isSlapping && !isOnCooldown) 
        {
            chargingTime = 0f;
            isCharging = true;

            
            animator.SetTrigger("triggerTapa");
        }


        // Se o botão estiver pressionado, aumenta o tempo de carga
        else if (Input.GetKey(keySlap) && isCharging && !isOnCooldown)
        {
            chargingTime += Time.deltaTime;

        }

        else if (isOnCooldown)
        {
            cooldownTimer += Time.deltaTime;
            if (cooldownTimer >= cooldown)
            {
                isOnCooldown = false;
            }
        }

        else if (!Input.GetKeyDown(keySlap) && isSlapping && !animator.GetCurrentAnimatorStateInfo(0).IsTag("Slap"))
        {
            isSlapping = false;
        }


        else if (Input.GetKeyUp(keySlap) && !isOnCooldown)
        {

            power = minPower + (maxPower - minPower) / (1 + Mathf.Exp(-growthRate * (chargingTime - midPoint))); // Calcula o valor de power
            TriggerGetRagBone.SetSlapPower(power); // Define o valor do Slap Power para o Slap Hit Box

            //UnityEngine.Debug.Log("Slap:",power);


            if (chargingTime <= quickSlapThreshold && !isSlapping) // Verifica se é um quick slap
            {
               
                QuickSlap();

            }
            else if (chargingTime > quickSlapThreshold && !isSlapping) // Caso contrário, é um charge slap
            {
                
                ChargedSlap();
            }

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
        if (!isSlapping) {

            animator.speed = 1f;        // Retoma a animação ao soltar o botão  
            chargingTime = 0f;
            isSlapping = true;          // Bloqueia novos slaps até o cooldown
            isCharging = false;
        }
    }

    void ChargedSlap()
    {

        if (!isSlapping)
        {

            animator.speed = 1f;
            isCharging = false;
            isSlapping = true; // Bloqueia novos slaps
            isCharging = false;
        }
    }

    public void SlapFeedback()
    {
        slapWhoosh.PlayFeedbacks();
    }

    void SlappingEnd()
    {
        cooldown = Mathf.Min(minCooldown + (power - minPower) * cooldownGrowthRate, maxCooldown);

        animator.speed = 1f;

        chargingTime = 0f;
        cooldownTimer = 0f;
        isSlapping = false;
        isCharging = false;
        isOnCooldown = true;

    }
}
