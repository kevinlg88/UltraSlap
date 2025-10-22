using UnityEngine;
using FIMSpace.FProceduralAnimation;

public class OneWayPlatform : MonoBehaviour
{
    [Header("References")]
    [Tooltip("Objeto que será movido pela plataforma. Deve ter Rigidbody.")]
    public Transform target;

    [Tooltip("Limite menor no eixo X.")]
    public Transform lowLimit;

    [Tooltip("Limite maior no eixo X.")]
    public Transform highLimit;

    [Header("Settings")]
    [Tooltip("Velocidade de movimento no eixo X.")]
    public float moveSpeed = 2f;

    [Tooltip("Multiplicador da força aplicada aos ragdolls.")]
    public float ragdollSpeedMultiplier = 1f;

    [Tooltip("Direção inicial do movimento (true = positivo/para a direita, false = negativo).")]
    public bool startMovingPositive = true;

    [Tooltip("Se true, durante o teleporte o collider do target é desativado momentaneamente para evitar esmagamento.")]
    public bool disableColliderDuringTeleport = true;

    [Tooltip("Se verdadeiro, a plataforma irá inverter a direção ao invés de se teletransportar.")]
    public bool twoWayPlatform = false;

    private Rigidbody rb;
    private Collider targetCollider;

    private enum MoveState { Stopped, MovingPositive, MovingNegative }
    private MoveState currentState;

    private Vector3 lastPosition;

    private void Awake()
    {
        currentState = startMovingPositive ? MoveState.MovingPositive : MoveState.MovingNegative;

        if (target == null)
        {
            Debug.LogError("OneWayPlatform: target não atribuído!");
            enabled = false;
            return;
        }

        // Garantir Rigidbody no target
        rb = target.GetComponent<Rigidbody>();
        if (rb == null)
        {
            rb = target.gameObject.AddComponent<Rigidbody>();
            rb.mass = 100f;
        }

        targetCollider = target.GetComponent<Collider>();

        // Ignorar colisões com layer "Floor" (se existir)
        int floorLayer = LayerMask.NameToLayer("Floor");
        if (targetCollider != null && floorLayer != -1)
        {
            Collider[] allColliders = FindObjectsOfType<Collider>();
            foreach (Collider col in allColliders)
            {
                if (col.gameObject.layer == floorLayer)
                    Physics.IgnoreCollision(targetCollider, col);
            }
        }

        lastPosition = rb.position;
    }

    private void FixedUpdate()
    {
        if (target == null || lowLimit == null || highLimit == null) return;

        Vector3 newPos = rb.position;

        // Movimento da plataforma
        if (currentState == MoveState.MovingPositive)
        {
            newPos += Vector3.right * moveSpeed * Time.fixedDeltaTime;
            if (newPos.x >= highLimit.position.x)
            {
                if (twoWayPlatform)
                {
                    // Inverte direção
                    currentState = MoveState.MovingNegative;
                }
                else
                {
                    TeleportTo(lowLimit.position.x);
                    return;
                }
            }
        }
        else if (currentState == MoveState.MovingNegative)
        {
            newPos += Vector3.left * moveSpeed * Time.fixedDeltaTime;
            if (newPos.x <= lowLimit.position.x)
            {
                if (twoWayPlatform)
                {
                    // Inverte direção
                    currentState = MoveState.MovingPositive;
                }
                else
                {
                    TeleportTo(highLimit.position.x);
                    return;
                }
            }
        }

        Vector3 delta = newPos - rb.position;
        rb.MovePosition(newPos);


        lastPosition = newPos;
    }


    private void OnCollisionStay(Collision collision)
    {
        Debug.Log("Chegou a verificação de ragdolls");

        // Tenta pegar o RagdollAnimator2 diretamente no objeto que bateu
        RagdollAnimator2 ragdoll = collision.gameObject.GetComponent<RagdollAnimator2>();
        if (ragdoll != null)
        {
            Debug.Log("Achou um ragdoll");

            Rigidbody rb = ragdoll.GetComponent<Rigidbody>();
            if (rb != null)
            {

                // Aplica força proporcional à velocidade da plataforma
                Vector3 dir = currentState == MoveState.MovingPositive ? Vector3.right : Vector3.left;
                rb.AddForce(dir * moveSpeed * ragdollSpeedMultiplier, ForceMode.VelocityChange);
            }
        }
    }

    private void TeleportTo(float x)
    {
        if (targetCollider != null && disableColliderDuringTeleport)
            targetCollider.enabled = false;

        Vector3 p = rb.position;
        p.x = x;
        rb.position = p;

        if (targetCollider != null && disableColliderDuringTeleport)
            targetCollider.enabled = true;
    }

    // Controle público
    public void MovePositiveX() => currentState = MoveState.MovingPositive;
    public void MoveNegativeX() => currentState = MoveState.MovingNegative;
    public void Stop() => currentState = MoveState.Stopped;
}