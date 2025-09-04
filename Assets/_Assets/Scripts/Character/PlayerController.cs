using FIMSpace.FProceduralAnimation;
using UnityEngine;
using Zenject;

public class PlayerController : MonoBehaviour
{
    public PlayerData PlayerData { get; set; }
    public RigidbodyController PlayerMovement { get; set; }
    public PlayerCustomization PlayerCustomization { get; set; }

    public GameEvent _gameEvent;
    private bool isDead = false;
    public bool IsDead => isDead;


    [Header("Stability Detection")]
    private Vector3 lastPosition;
    [SerializeField] private float stabilityCheckBaseTimer=0.5f;
    [SerializeField] private float stabilityCheckTimer; // Timer para iniciar a checagem se o personagem est� est�vel no ch�o
    [SerializeField] private float transformStabilityThreshold = 0.008f; // velocidade m�nima para considerar parado. Quanto menor � mais f�cil de ser considerado parado.

    [Header("Health")]
    [SerializeField] private int maxHealth;
    [SerializeField] private int health;
    [SerializeField] private RagdollAnimator2 ragdoll;
    [SerializeField] private float awakenRecoveredHealth; //Percentual de life que o jogador recupera quando se levanda de um estado negativo de health

    [Header("Standing State")]
    [SerializeField] private float standingTimeToHeal; //Tempo sem receber dano para come�ar a recuperar life enquanto no estado Standing
    [SerializeField] private float standingHealingTimer; //Timer para o pr�ximo heal no estado standing
    [SerializeField] private float standingHealingPercentage; //Percentual de health points a ser recuperado quando sem receber dano durante um tempo no estado Standing

    [Header("Falling State")]
    [SerializeField] private float timeToFalling; //Tempo "no ar" para mudar de outros estados para falling (quando j� n�o est� em falling)
    [SerializeField] private float fallingTimer; //Timer para contar se o tempo no ar atingiu o tempo especificado para mudar para o estado falling
    [SerializeField] private float fallingCheckTolerance = -0.06f; //Toler�ncia para checagem da queda

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


    public enum PlayerState
    {
        Standing,
        Falling,
        Downed,
    }

    [Header("Player Status")]
    // este � o campo serializado (vai aparecer no Inspector)
    [SerializeField]
    private PlayerState currentState = PlayerState.Standing;

    // esta � a propriedade s� de leitura (usada no c�digo)
    public PlayerState CurrentState => currentState;

    void Start()
    {
        currentState = PlayerState.Standing;
        ragdoll = GetComponent<RagdollAnimator2>();
    }
    void Update()
    {
        // if (currentState == PlayerState.Downed)
        // {
        //     FallingCheck();
        //     downedTimer -= Time.deltaTime;
        //     if (downedTimer <= 0)
        //     {
        //         downedTimer = 0;
        //         SetIsStanding();
        //     }
        //     wakeUpPressTimer += Time.deltaTime;
        // }
        // if (currentState == PlayerState.Falling)
        // {
        //     stabilityCheckTimer -= Time.deltaTime;
        //     if (stabilityCheckTimer <= 0) stabilityCheckTimer = 0;
        //     if (IsTransformStable() && stabilityCheckTimer <= 0) SetIsDowned();
        // }
        // if (currentState == PlayerState.Standing)
        // {
        //     FallingCheck();
        //     if (health < maxHealth)
        //     {
        //         standingHealingTimer -= Time.deltaTime;
        //         if (standingHealingTimer <= 0)
        //         {
        //             health = Mathf.Clamp(health + (int)(maxHealth * standingHealingPercentage), 0, maxHealth);
        //             standingHealingTimer = standingTimeToHeal;
        //         }
        //     }
        // }
        // lastPosition = transform.position;
    }

    public void ResetState()
    {
        //SetIsStanding();
        health = maxHealth;
        isDead = false;
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("DeathBox"))
        {
            isDead = true;
            _gameEvent.onPlayerDeath.Invoke();
        }
    }

    //public int GetMaxHealth() => maxHealth;
    //public int GetHealth() => health;
    //public PlayerState GetCurrentState() => currentState;
    // public void TakeHit(int newHealth)
    // {
    //     if (currentState == PlayerState.Standing) health = newHealth;
    //     if (health <= 0) SetIsFalling();
    //     else if (health > 0) standingHealingTimer = standingTimeToHeal;
    // }

    // private bool IsTransformStable() //Checa se o personagem est� "est�vel" no ch�o
    // {
    //     float posDiff = Vector3.Distance(transform.position, lastPosition);
    //     return posDiff < transformStabilityThreshold;
    // }

    // private void FallingCheck() //Checa se o personagem deve entrar no estado de queda mesmo sem ter levado um tapa
    // {
    //     float deltaY = transform.position.y - lastPosition.y;
    //     if (deltaY < fallingCheckTolerance) // toler�ncia pequena para n�o pegar microvaria��es
    //     {
    //         fallingTimer += Time.deltaTime;
    //         if (fallingTimer >= timeToFalling)
    //         {
    //             SetIsFalling();
    //         }
    //     }
    //     else fallingTimer = 0;
    // }

    // public void SetIsStanding()
    // {
    //     //if (!Physics.Raycast(transform.position, Vector3.down, 0.2f)) return;
        
    //     currentState = PlayerState.Standing;
    //     if(ragdoll != null) ragdoll.RA2Event_SwitchToStand();

    //     if (health < (int)(maxHealth * awakenRecoveredHealth)) // Se, ao se levantar, o life � menor do que a metade do life total, life = metade do life total
    //     {
    //         health = (int)(maxHealth * awakenRecoveredHealth);
    //         standingHealingTimer = standingTimeToHeal; //Seta o timer para come�ar o heal passivo
    //     }
    //     wakeUpPressTimer = 0;
    // }

    // public void SetIsFalling()
    // {
    //     //Executar este bloco em um evento no slap
    //     if (GetComponent<PlayerSlap>().GetIsCharging())
    //     {
    //         GetComponent<PlayerSlap>().stopSlapFeedback();
    //         GetComponent<PlayerSlap>().AnimEvt_SlappingEnd();
    //     }
    //     GetComponent<PlayerSlap>().AnimEvt_SlappingEnd();
    //     //-------------------------------------------

    //     if (currentState == PlayerState.Standing)
    //     {
    //         if (health <= 0) definedDownedTimer = baseDownedTimer + (int)((-1f * health) / 100f);
    //         else definedDownedTimer = baseDownedTimer;
    //     }
    //     if (currentState == PlayerState.Downed || currentState == PlayerState.Falling) definedDownedTimer = downedTimer;
        
    //     currentState = PlayerState.Falling;
    //     ragdoll.RA2Event_SwitchToFall();

    //     lastPosition = transform.position;
    //     stabilityCheckTimer = stabilityCheckBaseTimer;
    //     wakeUpPressTimer = 0;
    // }

    // public void SetIsDowned()
    // {
    //     currentState = PlayerState.Downed;
    //     downedTimer = definedDownedTimer;
    //     wakeUpPressTimer = 0;
    // }

    // public void TryingToWakeUp()
    // {
    //     if (currentState != PlayerState.Downed) return;
    //     // Conta o toque
    //     wakeUpPressCounter += 1f;

    //     if (wakeUpPressTimer >= wakeUpBaseTimeCheck)
    //     {
    //         if (wakeUpPressCounter >= wakeUpPressesPerSecond) downedTimer -= wakeUpTimeReduction;
            
    //         wakeUpPressCounter = 0;
    //         wakeUpPressTimer = 0;
    //     }
    // }
}
