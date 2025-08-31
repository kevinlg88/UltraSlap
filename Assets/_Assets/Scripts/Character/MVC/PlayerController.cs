using FIMSpace.FProceduralAnimation;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using Zenject;

public class PlayerController : MonoBehaviour
{
    public PlayerData PlayerData { get; set; }
    public RigidbodyController PlayerMovement { get; set; }
    public PlayerCustomization PlayerCustomization { get; set; }


    private bool isDead = false;
    public bool IsDead => isDead;

    public GameEvent _gameEvent;

    [Header("Stability Detection")]
    private Vector3 lastPosition;
    [SerializeField] private float stabilityCheckBaseTimer=0.5f;
    [SerializeField] private float stabilityCheckTimer; // Timer para iniciar a checagem se o personagem está estável no chão
    [SerializeField] private float transformStabilityThreshold = 0.008f; // velocidade mínima para considerar parado. Quanto menor é mais fácil de ser considerado parado.

    [Header("Health")]
    [SerializeField] private int maxHealth, health;
    [SerializeField] private RagdollAnimator2 ragdoll;
    [SerializeField] private float awakenRecoveredHealth; //Percentual de life que o jogador recupera quando se levanda de um estado negativo de health

    [Header("Standing State")]
    [SerializeField] private float standingTimeToHeal; //Tempo sem receber dano para começar a recuperar life enquanto no estado Standing
    [SerializeField] private float standingHealingTimer; //Timer para o próximo heal no estado standing
    [SerializeField] private float standingHealingPercentage; //Percentual de health points a ser recuperado quando sem receber dano durante um tempo no estado Standing

    [Header("Downed State")]
    [SerializeField] private float baseDownedTimer = 5; //Tempo base de tempo no estado caído
    [SerializeField] private float definedDownedTimer; //Tempo definido de tempo para o estado caído, levando em consideração quantidade de health negativo
    [SerializeField] private float downedTimer;
    [SerializeField] private float downedTimePerDamageUnit = 1;
    [SerializeField] private int downedDamageUnit = 100;

    public enum PlayerState
    {
        Standing,
        Falling,
        Downed,
    }

    [Header("Player Status")]
    // este é o campo serializado (vai aparecer no Inspector)
    [SerializeField]
    private PlayerState currentState = PlayerState.Standing;

    // esta é a propriedade só de leitura (usada no código)
    public PlayerState CurrentState => currentState;

    // [Inject]
    // public void Construct(GameEvent gameEvent)
    // {
    //     _gameEvent = gameEvent;
    //     Debug.Log("GameEvent Instalado");
    // }

    // Start is called before the first frame update
    void Start()
    {
        currentState = PlayerState.Standing;
        ragdoll = GetComponent<RagdollAnimator2>();
    }

    void OnEnable()
    {
        health = maxHealth;
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("DeathBox"))
        {
            isDead = true;
            _gameEvent.onPlayerDeath.Invoke();
        }
    }

    void Update()
    {

        if (currentState == PlayerState.Downed)
        {
            downedTimer -= Time.deltaTime;
            if (downedTimer <= 0)
            {
                downedTimer = 0;
                SetIsStanding();
            }
                
        }

        if (currentState == PlayerState.Falling)
        {
            stabilityCheckTimer -= Time.deltaTime;

            if (stabilityCheckTimer <= 0)
                stabilityCheckTimer = 0;

            if (IsTransformStable() && stabilityCheckTimer <= 0)
            {
                SetIsDowned();
            }

            lastPosition = transform.position;
        }

        if (currentState == PlayerState.Standing)
        {
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

    public int GetMaxHealth()
    {
        return maxHealth;
    }

    public int GetHealth()
    {
        return health;
    }

    public void TakeHit(int newHealth)
    {
        health = newHealth;


        if (health <= 0) //Se o life é menor ou igual a zero o player tem que cair
        {
            SetIsFalling();
        }
        else if (health > 0) //E recebeu dano, mas o life ainda é maior do que zero, deve-se reiniciar o timer para o heal passivo
        {
            standingHealingTimer = standingTimeToHeal; //Seta o timer para começar o heal passivo
        }
         
    }


    bool IsTransformStable()
    {
        float posDiff = Vector3.Distance(transform.position, lastPosition);

        return posDiff < transformStabilityThreshold;
    }

    public void SetIsStanding()
    {
        currentState = PlayerState.Standing;
        ragdoll.User_TransitionToStandingMode();

        if (health < (int)(maxHealth * awakenRecoveredHealth)) // Se, ao se levantar, o life é menor do que a metade do life total, life = metade do life total
        {
            health = (int)(maxHealth * awakenRecoveredHealth);

            standingHealingTimer = standingTimeToHeal; //Seta o timer para começar o heal passivo
        }
    }

    public void SetIsFalling()
    {
        if (currentState == PlayerState.Standing)
        {
            if (health <= 0)
            {
                definedDownedTimer = baseDownedTimer + (int)((-1f * health) / 100f);
            }
            else
            {
                definedDownedTimer = baseDownedTimer;
            }
        }
        if (currentState == PlayerState.Downed || currentState == PlayerState.Falling)
        {
            definedDownedTimer = downedTimer;
        }
      

        currentState = PlayerState.Falling;
        ragdoll.User_SwitchFallState();

        lastPosition = transform.position;

        stabilityCheckTimer = stabilityCheckBaseTimer;


    }

    public void SetIsDowned()
    {
        currentState = PlayerState.Downed;
        downedTimer = definedDownedTimer;
    }
}
