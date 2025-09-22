using System.Threading.Tasks;
using FIMSpace.FProceduralAnimation;
using UnityEngine;
using Zenject;
using MoreMountains.Feedbacks;
using MoreMountains.FeedbacksForThirdParty;


public enum PlayerState
{
    Standing,
    Falling,
    Downed,
}
public class PlayerController : MonoBehaviour
{
    public PlayerData PlayerData { get; set; }
    public RigidbodyController PlayerMovement { get; set; }
    public PlayerCustomization PlayerCustomization { get; set; }

    public GameEvent _gameEvent;
    private bool isDead = false;
    public bool IsDead => isDead;
    RagdollAnimator2 ragdoll;
    Rigidbody rb;
    PlayerSlap playerSlap;

    [Header("Body Drop SFX")]
    [SerializeField] private MMFeedbacks bodyDropMMFeedbacks; // Efeito a ser tocado quando o personagem cai no ch�o
    [SerializeField] private float fallVolumeMultiplier = 14f; // constante para fine tuning
    [SerializeField] private float maxAllowedMultiplier = 2.5f; // limite do multiplicador dos volumes

    #region  ==== PLAYER STATE VARIABLES ====

    [Header("Stability Detection")]
    [SerializeField] private float stabilityCheckTolerance = -1.5f;

    [Header("Health")]
    [SerializeField] private int maxHealth;
    [SerializeField] private int health;
    [SerializeField] private float awakenRecoveredHealth; //Percentual de life que o jogador recupera quando se levanda de um estado negativo de health

    [Header("Standing State")]
    [SerializeField] private float standingTimeToHeal; //Tempo sem receber dano para come�ar a recuperar life enquanto no estado Standing
    [SerializeField] private float standingHealingTimer; //Timer para o pr�ximo heal no estado standing
    [SerializeField] private float standingHealingPercentage; //Percentual de health points a ser recuperado quando sem receber dano durante um tempo no estado Standing

    [Header("Falling State")]
    [SerializeField] private float fallingCheckTolerance = -15f; //Toler�ncia para checagem da queda
    // Vari�veis para c�lculo da velocidade pelo Transform
    [SerializeField] private float currentVerticalSpeed;      // velocidade vertical calculada
    private Vector3 lastPositionForImpact;   // posi��o no frame anterior
    [SerializeField] private float soundImpactMultiplier = 0.025f;

    [Header("Downed State")]
    [SerializeField] private float baseDownedTimer = 5; //Tempo base de tempo no estado ca�do
    [SerializeField] private float definedDownedTimer; //Tempo definido de tempo para o estado ca�do, levando em considera��o quantidade de health negativo
    [SerializeField] private float downedTimer;

    [Header("Trying to Wake Up")]
    [SerializeField] private float wakeUpPressesPerSecond = 5f; // Quantos toques por segundo
    [SerializeField] private float wakeUpTimeReduction = 0.5f;  // Quanto cada grupo de toques reduz do timer
    private float wakeUpPressCounter = 0f; // Contador de toques
    private float wakeUpPressTimer = 0;        // Timer para medir o apertar de bot�o por segundo
    private float wakeUpBaseTimeCheck = 1f;        // Intervalo de tempo em que � verificado quantas vezes o bot�o foi apertado

    [Header("Player Status")]
    [SerializeField] private PlayerState currentState = PlayerState.Standing;

    public PlayerState CurrentState => currentState;

    #endregion

    void Awake()
    {
        ragdoll = GetComponent<RagdollAnimator2>();
        rb = GetComponent<Rigidbody>();
        playerSlap = GetComponent<PlayerSlap>();
    }
    void Start()
    {
        currentState = PlayerState.Standing;
        health = maxHealth;
        isDead = false;
    }
    void Update()
    {
        calculateImpactSpeed();

        UpdatePlayerState();
    }

    void calculateImpactSpeed()
    {
        // Atualiza a velocidade do impacto
        Vector3 deltaPos = transform.position - lastPositionForImpact;
        currentVerticalSpeed = deltaPos.y / Time.deltaTime;
        lastPositionForImpact = transform.position;
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("DeathBox") && !isDead)
        {
            isDead = true;
            _gameEvent.onPlayerDeath.Invoke();
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Floor"))
        {
            Debug.Log($"Impacto no ch�o! Velocidade: {currentVerticalSpeed}");

            if (currentState == PlayerState.Downed || currentState == PlayerState.Falling || health <= 0)
            {
                SetIsDowned();
                float intensity = (fallingCheckTolerance * (-1)); //passando para positivo para que possa agir como um fator multiplicador do som do impacto
                PlayFallSound(intensity);
            }
        }
    }

    #region  ==== PLAYER STATE FUNCTIONS ====
    private void UpdatePlayerState()
    {
        if (currentState == PlayerState.Downed)
        {
            FallingCheck();
            downedTimer -= Time.deltaTime;
            if (downedTimer <= 0)
            {
                downedTimer = 0;
                SetIsStanding();
            }
            wakeUpPressTimer += Time.deltaTime;
        }

        if (currentState == PlayerState.Standing)
        {
            FallingCheck();
            if (health < maxHealth)
            {
                standingHealingTimer -= Time.deltaTime;
                if (standingHealingTimer <= 0)
                {
                    health = Mathf.Clamp(health + (int)(maxHealth * standingHealingPercentage), 0, maxHealth);
                    standingHealingTimer = standingTimeToHeal;
                }
            }
        }
    }


    private void FallingCheck() //Checa se o personagem deve entrar no estado de queda mesmo sem ter levado um tapa
    {
        // --- Est� caindo (movimento para baixo al�m da toler�ncia) ---
        if (currentVerticalSpeed < fallingCheckTolerance) //como a velocidade � negativa para quedas, o valor tem que ser "menor que"
        {
            SetIsFalling();
        }

    }

    private void SetIsStanding()
    {
        //if (!Physics.Raycast(transform.position, Vector3.down, 0.2f)) return;

        currentState = PlayerState.Standing;
        if (ragdoll != null) ragdoll.RA2Event_SwitchToStand();

        if (health < (int)(maxHealth * awakenRecoveredHealth)) // Se, ao se levantar, o life � menor do que a metade do life total, life = metade do life total
        {
            health = (int)(maxHealth * awakenRecoveredHealth);
            standingHealingTimer = standingTimeToHeal; //Seta o timer para come�ar o heal passivo
        }
        wakeUpPressTimer = 0;
    }

    private void SetIsDowned()
    {
        currentState = PlayerState.Downed;
        downedTimer = definedDownedTimer;
        wakeUpPressTimer = 0;
    }

    public int GetHealth() => health;
    public PlayerState GetCurrentState() => currentState;
    public void TakeHit(int newHealth)
    {
        Debug.Log($"Minha vida ?: {GetHealth()} agr a vida ? {newHealth}");
        if (currentState == PlayerState.Standing) health = newHealth;
        if (health <= 0) SetIsFalling();
        else if (health > 0) standingHealingTimer = standingTimeToHeal;
    }
    public void SetIsFalling()
    {
        //Executar este bloco em um evento no slap
        if (playerSlap.GetIsCharging())
        {
            playerSlap.StopSlapFeedback();
            playerSlap.AnimEvt_SlappingEnd();
        }
        playerSlap.AnimEvt_SlappingEnd();
        //-------------------------------------------

        if (currentState == PlayerState.Standing)
        {
            if (health <= 0) definedDownedTimer = baseDownedTimer + (int)((-1f * health) / 100f);
            else definedDownedTimer = baseDownedTimer;
        }
        if (currentState == PlayerState.Downed || currentState == PlayerState.Falling) definedDownedTimer = downedTimer;

        currentState = PlayerState.Falling;
        ragdoll.User_SwitchFallState();

        wakeUpPressTimer = 0;
    }
    public void TryingToWakeUp()
    {
        if (currentState != PlayerState.Downed) return;
        // Conta o toque
        wakeUpPressCounter += 1f;

        if (wakeUpPressTimer >= wakeUpBaseTimeCheck)
        {
            if (wakeUpPressCounter >= wakeUpPressesPerSecond) downedTimer -= wakeUpTimeReduction;

            wakeUpPressCounter = 0;
            wakeUpPressTimer = 0;
        }
    }

    #endregion

    private void PlayFallSound(float intensity)
    {
        if (bodyDropMMFeedbacks == null) return;



        foreach (var feedback in bodyDropMMFeedbacks.Feedbacks)
        {
            if (feedback is MMFeedbackMMSoundManagerSound soundFeedback)
            {
                float originalMinVolume = soundFeedback.MinVolume;
                float originalMaxVolume = soundFeedback.MaxVolume;

                float volumeScale = Mathf.Clamp(intensity * soundImpactMultiplier, 1f, maxAllowedMultiplier);
                soundFeedback.MinVolume *= volumeScale;
                soundFeedback.MaxVolume *= volumeScale;

                //Debug.Log($"Impacto no ch�o! Velocidade: {rb.velocity.y} | Intensidade: {intensity} | Volume: {volumeScale}");

                bodyDropMMFeedbacks.PlayFeedbacks();

                Debug.Log($"VolumeScale: {volumeScale} | MinVolume: {soundFeedback.MinVolume} | MaxVolume: {soundFeedback.MaxVolume}");

                soundFeedback.MinVolume = originalMinVolume;
                soundFeedback.MaxVolume = originalMaxVolume;
            }
        }

    }
}
