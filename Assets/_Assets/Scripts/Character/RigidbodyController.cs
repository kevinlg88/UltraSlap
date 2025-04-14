using System;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.InputSystem;

public class RigidbodyController : MonoBehaviour
{
    public Rigidbody rigidbodyComponent;

    [Header("Movement Settings")]
    public float movementSpeed = 2f;
    [Range(0f, 1f)] public float rotationSpeed = 0.8f;
    [Tooltip("When true, applying rotation using rigidbody.rotation.\nWhen false, applying rotation using angular velocity (smoother interpolation)")]
    public bool useFixedRotation = true;

    [Range(0f, 1f)] public float directMovementFactor = 0f;
    [Range(0f, 1f)] public float inertiaFactor = 1f;

    [Header("Dash Settings")]
    public float dashForce = 10f;
    public float dashDuration = 0.2f;
    public InputActionReference dashInput;

    private bool isDashing = false;
    private float dashTimer = 0f;
    private Vector3 dashDirection;

    [Header("Dash Cooldown Settings")]
    public float dashCooldownTime = 2.0f; // Tempo de cooldown em segundos
    private float nextDashTime = 0f;      // Timestamp para o próximo dash

    [Header("Animator Settings")]
    public Animator animator;

    [Header("Input Settings")]
    public InputActionReference move;
    public bool enableInput = true;

    [Header("Gravity Settings")]
    public float gravityMultiplier = 1f;

    private Quaternion targetRotation;
    private Quaternion targetInstantRotation;
    private float rotationAngle = 0f;
    private float smoothRotationAngle = 0f;

    [SerializeField] private Transform groundCheckPoint; // Um ponto logo abaixo do jogador
    [SerializeField] private float groundCheckDistance = 0.2f; // Distância para considerar "no chão"
    [SerializeField] private LayerMask groundLayer; // Camada do chão

    public bool isGrounded = true;

    [NonSerialized] public Vector2 localMoveDirection = Vector2.zero;
    public Vector3 worldMoveDirection { get; set; }
    public Vector3 currentAcceleration { get; private set; }

    [SerializeField] PlayerSlap PlayerSlap;

    private void Start()
    {
        if (!rigidbodyComponent) rigidbodyComponent = GetComponent<Rigidbody>();

        if (rigidbodyComponent)
        {
            rigidbodyComponent.maxAngularVelocity = 30f;
            if (rigidbodyComponent.interpolation == RigidbodyInterpolation.None)
                rigidbodyComponent.interpolation = RigidbodyInterpolation.Interpolate;

            rigidbodyComponent.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
        }

        isGrounded = true;
        targetRotation = transform.rotation;
        targetInstantRotation = transform.rotation;
        rotationAngle = transform.eulerAngles.y;

        dashInput.action.Enable();
    }

    private void Update()
    {
        if (rigidbodyComponent == null || !enableInput) return;

        CheckGroundStatus();

        HandleMovementInput();
        
        if (dashInput.action.WasPressedThisFrame() && !isDashing && isGrounded && Time.time >= nextDashTime) //Aciona o dash se botão foi apertado, o personagem não está em dash, está no chão, e não está com cooldown ativo
        {
            StartDash();
        }

        UpdateMovement();
    }

    void CheckGroundStatus() //Checa se o personagem está no chão
    {
        isGrounded = Physics.Raycast(groundCheckPoint.position, Vector3.down, groundCheckDistance, groundLayer);
    }

    private void HandleMovementInput()
    {
        localMoveDirection = Vector2.zero;

        if (PlayerSlap.GetIsSlapping() == false) { 

            localMoveDirection = move.action.ReadValue<Vector2>();

            //if (Input.GetKey(KeyCode.A)) localMoveDirection += Vector2.left;
            //if (Input.GetKey(KeyCode.D)) localMoveDirection += Vector2.right;
            //if (Input.GetKey(KeyCode.W)) localMoveDirection += Vector2.up;
            //if (Input.GetKey(KeyCode.S)) localMoveDirection += Vector2.down;
        }

        Quaternion cameraRotation = Quaternion.Euler(0f, Camera.main.transform.eulerAngles.y, 0f);

        if (localMoveDirection != Vector2.zero)
        {
            localMoveDirection.Normalize();
            worldMoveDirection = cameraRotation * new Vector3(localMoveDirection.x, 0f, localMoveDirection.y);
            targetInstantRotation = Quaternion.LookRotation(worldMoveDirection);
        }
        else
        {
            worldMoveDirection = Vector3.zero;
        }
    }

    private void UpdateMovement()
    {
        bool isMoving = worldMoveDirection != Vector3.zero;

        if (isDashing)
        {
            float dashSpeed = dashForce / dashDuration; // Converte a força em uma velocidade constante
            rigidbodyComponent.MovePosition(rigidbodyComponent.position + dashDirection * dashSpeed * Time.deltaTime);

            dashTimer -= Time.deltaTime;

            if (dashTimer <= 0f)
            {
                isDashing = false;
            }

            return; // Ignora o resto do movimento durante o dash
        }

        if (rotationSpeed > 0f && currentAcceleration != Vector3.zero)
        {
            rotationAngle = Mathf.SmoothDampAngle(rotationAngle, targetInstantRotation.eulerAngles.y, ref smoothRotationAngle, Mathf.Lerp(0.5f, 0.01f, rotationSpeed));
            targetRotation = Quaternion.Euler(0f, rotationAngle, 0f);
        }

        if (animator)
        {
            animator.SetBool("isMoving", isMoving);
        }

        // ❌ Se não está no chão, não atualiza a movimentação
        if (!isGrounded)
        {
            worldMoveDirection = Vector3.zero; // limpa o input
            return;
        }

        float speed = movementSpeed;
        float acceleration = isMoving ? 5f * movementSpeed : 7f * movementSpeed;

        if (isGrounded)
        {
            // ✅ Aplica aceleração normalmente se estiver no chão
            if (inertiaFactor < 1f)
            {
                currentAcceleration = Vector3.Lerp(
                    Vector3.Slerp(currentAcceleration, worldMoveDirection * speed, Time.deltaTime * acceleration),
                    Vector3.MoveTowards(currentAcceleration, worldMoveDirection * speed, Time.deltaTime * acceleration),
                    inertiaFactor);
            }
            else
            {
                currentAcceleration = Vector3.MoveTowards(currentAcceleration, worldMoveDirection * speed, Time.deltaTime * acceleration);
            }
        }
        else
        {
            // ✅ No ar, desacelera suavemente até parar (ou com air control, se quiser)
            currentAcceleration = Vector3.Lerp(currentAcceleration, Vector3.zero, Time.deltaTime * 2f);
        }

        worldMoveDirection = Vector3.zero;
    }

    private void FixedUpdate()
    {
        if (rigidbodyComponent == null) return;

        Vector3 targetVelocity = currentAcceleration;
        float yAngleDifference = Mathf.DeltaAngle(rigidbodyComponent.rotation.eulerAngles.y, targetInstantRotation.eulerAngles.y);
        float adjustedDirectMovement = directMovementFactor * Mathf.Lerp(1f, Mathf.InverseLerp(180f, 50f, Mathf.Abs(yAngleDifference)), inertiaFactor);
        targetVelocity = Vector3.Lerp(targetVelocity, transform.forward * targetVelocity.magnitude, adjustedDirectMovement);

        // Apply gravity
        targetVelocity.y += Physics.gravity.y * gravityMultiplier * Time.fixedDeltaTime;

        rigidbodyComponent.velocity = targetVelocity;

        if (useFixedRotation)
        {
            rigidbodyComponent.rotation = targetRotation;
        }
    }

    private void StartDash()
    {
        // Ativa o estado de dash
        isDashing = true;
        dashTimer = dashDuration;

        // Define a direção do dash como a direção que o personagem está olhando
        dashDirection = transform.forward.normalized;

        // Cancela aceleração normal durante o dash
        currentAcceleration = Vector3.zero;

        // Dispara animação se existir
        if (animator != null)
            animator.SetTrigger("Dash");

        // Log para depuração
        UnityEngine.Debug.Log("🚀 Dash iniciado!");

        nextDashTime = Time.time + dashCooldownTime; //inicia o cooldown do dash
    }
}
