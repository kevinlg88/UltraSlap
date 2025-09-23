using UnityEngine;
using MoreMountains.Feedbacks;
using Rewired;

public class PlayerSlap : MonoBehaviour
{
    [Header("References")]
    [SerializeField] TriggerHitbox TriggerGetRagBone;

    [Header("Feel References Temp")]
    [SerializeField] private MMFeedbacks slapWhoosh;
    [SerializeField] private MMFeedbacks chargingSlap;
    [SerializeField] private MMFeedbacks wooshChargingSlap; // Som da girada de braço enquanto carregando
    [SerializeField] private MMFeedbacks chargingSlapEnd;

    [Header("Setup Power")]
    [SerializeField] float minPower = 250;
    [SerializeField] float maxPower = 1500;
    [SerializeField] float growthRate = 1.5f;
    [SerializeField] float midPoint = 2.5f;

    [Header("Setup Timer")]
    [SerializeField] float minCooldown = 0.1f;
    [SerializeField] float maxCooldown = 0.8f;
    [SerializeField] float cooldownTimer = 0f;
    [SerializeField] float cooldownGrowthRate = 0.001f;

    [Header("Setup Threshold")]
    [SerializeField] private float quickSlapThreshold = 0.2f;
    [SerializeField] private float maxSlapThreshold = 5f;

    [Header("Debug")]
    [SerializeField] bool isCharging = false;
    [SerializeField] bool isSlapping = false;
    [SerializeField] bool isOnCooldown = false;  //Definição e controle de slap cooldown
    [SerializeField] bool buttonReleasedBeforeKeyframe = false;
    [SerializeField] float chargingTime = 0f;
    [SerializeField] float power;               //Variáveis para a definição da intensidade do tapa, de acordo com o tempo de carregamento
    [SerializeField] float cooldown;



    private Player playerInput;
    private Animator animator;
    private PlayerController playerController;

    void Awake()
    {
        playerController = GetComponent<PlayerController>();
        animator = GetComponent<Animator>();
    }
    void Update()
    {
        if (playerInput == null) return;
        if (playerInput.GetButtonDown("Slap") && !isCharging && !isSlapping && !isOnCooldown)
        {
            if (!(playerController.GetCurrentState() == PlayerState.Standing)) //Verifica se não está na condição Standing
                return;

            chargingTime = 0f;
            isCharging = true;
            animator.SetBool("isInterrupted", false);
            animator.SetTrigger("triggerSlap");
            animator.SetBool("isChargingSlap", true);
        }
        
        // Se o botão estiver pressionado, aumenta o tempo de carga
        else if (playerInput.GetButton("Slap") && isCharging && !isOnCooldown) chargingTime += Time.deltaTime;

        else if (isOnCooldown)
        {
            cooldownTimer -= Time.deltaTime;
            if (cooldownTimer <= 0) {
                cooldownTimer = 0;
                isOnCooldown = false;
            }
            
        }

        else if (!playerInput.GetButtonDown("Slap") && isSlapping && !animator.GetCurrentAnimatorStateInfo(0).IsTag("Slap"))
            isSlapping = false;
        
        else if (playerInput.GetButtonUp("Slap") && !isOnCooldown)
        {
            power = minPower + (maxPower - minPower) / (1 + Mathf.Exp(-growthRate * (chargingTime - midPoint))); // Calcula o valor de power

            if (chargingTime <= quickSlapThreshold && !isSlapping) QuickSlap();
            else if (chargingTime > quickSlapThreshold && !isSlapping) ChargedSlap();
        }

    }

    public void PauseChargeAnimation(){ if (isCharging) chargingSlap.PlayFeedbacks();}
    
    private void QuickSlap()
    {
        if (!isSlapping)
        {
            animator.SetBool("isChargingSlap", false);
            animator.speed = 1f;        // Retoma a animação ao soltar o botão  
            chargingTime = 0f;
            isSlapping = true;          // Bloqueia novos slaps até o cooldown
            isCharging = false;

            isOnCooldown = true;
            cooldown = Mathf.Min(minCooldown + (power - minPower) * cooldownGrowthRate, maxCooldown);
            cooldownTimer = cooldown;

            StopSlapFeedback();
        }
    }

    private void ChargedSlap()
    {
        if (!isSlapping)
        {
            animator.SetBool("isChargingSlap", false);
            animator.speed = 1f;
            isSlapping = true; // Bloqueia novos slaps
            isCharging = false;

            isOnCooldown = true;
            cooldown = Mathf.Min(minCooldown + (power - minPower) * cooldownGrowthRate, maxCooldown);
            cooldownTimer = cooldown;

            StopSlapFeedback();
        }
    }

    public void AnimEvt_SlappingEnd()
    {
        chargingTime = 0f;
        isSlapping = false;
        isCharging = false;

        StopSlapFeedback();
        chargingSlapEnd.PlayFeedbacks();
        animator.SetBool("isChargingSlap", false);

        animator.speed = 1f;

    }
    public void WooshChargingSlap() => wooshChargingSlap.PlayFeedbacks();
    public void StopSlapFeedback() => chargingSlap.StopFeedbacks();
    public void AnimEvt_SlapFeedback() => slapWhoosh.PlayFeedbacks();
    public bool GetIsSlapping() => isSlapping;
    public bool GetIsCharging() => isCharging;
    public float GetMinPower() => minPower;
    public float GetMaxPower() => maxPower;
    public float GetPower() => power;
    public float GetQuickSlapThreshold() => quickSlapThreshold;
    public float GetChargingTime() => chargingTime;
    public TriggerHitbox GetTriggerGetRagBone() => TriggerGetRagBone;
    public void ActivateHitbox() => TriggerGetRagBone.gameObject.SetActive(true);
    public void DeactivateHitbox() => TriggerGetRagBone.gameObject.SetActive(false);
    public void SetPlayer(Player player) => this.playerInput = player;
}
