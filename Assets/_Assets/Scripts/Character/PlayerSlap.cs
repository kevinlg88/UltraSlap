using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MoreMountains.Feedbacks;
using System.Diagnostics;
using System.Runtime;
using System.Threading.Tasks;

public class PlayerSlap : MonoBehaviour
{
    [SerializeField] KeyCode keySlap;
    [SerializeField] private MMFeedbacks slapWhoosh;
    [SerializeField] private MMFeedbacks chargingSlap;
    [SerializeField] private MMFeedbacks chargingSlapEnd;
    Animator animator;

    [SerializeField] bool isCharging = false;
    [SerializeField] bool isSlapping = false;
    [SerializeField] bool buttonReleasedBeforeKeyframe = false;
    [SerializeField] float chargingTime = 0f;

    [SerializeField] float minPower = 250;
    [SerializeField] float maxPower = 1500;
    [SerializeField] float power;               //Vari�veis para a defini��o da intensidade do tapa, de acordo com o tempo de carregamento
    [SerializeField] float growthRate = 1.5f;
    [SerializeField] float midPoint = 2.5f;

    [SerializeField] float minCooldown = 0.1f;
    [SerializeField] float maxCooldown = 0.8f;
    [SerializeField] bool isOnCooldown = false;  //Defini��o e controle de slap cooldown
    [SerializeField] float cooldownTimer = 0f;
    [SerializeField] float cooldownGrowthRate = 0.001f;
    [SerializeField] float cooldown;

    [SerializeField] private float quickSlapThreshold = 0.1f;
    [SerializeField] private float maxSlapThreshold = 5f;

    [SerializeField] TriggerHitbox TriggerGetRagBone;


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
            animator.SetTrigger("triggerSlap");
            animator.SetBool("isChargingSlap", true);
        }


        // Se o bot�o estiver pressionado, aumenta o tempo de carga
        else if (Input.GetKey(keySlap) && isCharging && !isOnCooldown)
        {
            chargingTime += Time.deltaTime;
        }

        else if (isOnCooldown)
        {
            cooldownTimer -= Time.deltaTime;
            if (cooldownTimer <= 0)
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


            if (chargingTime <= quickSlapThreshold && !isSlapping) // Verifica se � um quick slap
            {
               
                QuickSlap();

            }
            else if (chargingTime > quickSlapThreshold && !isSlapping) // Caso contr�rio, � um charge slap
            {
                
                ChargedSlap();
            }

        }

    }

    public void PauseChargeAnimation()
    {

        UnityEngine.Debug.Log("carregando");
        if (isCharging)
        {
            //UnityEngine.Debug.Log("carregando");
            //animator.speed = 0;  // Pausa a anima��o
            chargingSlap.PlayFeedbacks();

        }
    }


    void QuickSlap()
    {
        if (!isSlapping) {
            animator.SetBool("isChargingSlap", false);
            animator.speed = 1f;        // Retoma a anima��o ao soltar o bot�o  
            chargingTime = 0f;
            isSlapping = true;          // Bloqueia novos slaps at� o cooldown
            isCharging = false;

            chargingSlap.StopFeedbacks(); // Para com os feedbacks de carregamento do tapa
        }
    }

    void ChargedSlap()
    {

        if (!isSlapping)
        {
            animator.SetBool("isChargingSlap", false);
            animator.speed = 1f;
            isSlapping = true; // Bloqueia novos slaps
            isCharging = false;

            chargingSlap.StopFeedbacks(); // Para com os feedbacks de carregamento do tapa
        }
    }

    public void SlapFeedback()
    {
        slapWhoosh.PlayFeedbacks();
    }

    void SlappingEnd()  //Define que n�o tem mais nenhum tapa ativo
    {
        animator.speed = 1f;

        chargingTime = 0f;

        cooldown = Mathf.Min(minCooldown + (power - minPower) * cooldownGrowthRate, maxCooldown);
        cooldownTimer = cooldown;

        isSlapping = false;
        isCharging = false;
        isOnCooldown = true;
        chargingSlapEnd.PlayFeedbacks();
        animator.SetBool("isChargingSlap", false);


    }

    public bool GetIsSlapping()
    {
        return isSlapping;
    }

    public float GetMinPower()
    {
        return minPower;
    }

    public float GetMaxPower()
    {
        return maxPower;
    }
}
