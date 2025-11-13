using Rewired;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(PlayerSlap))]
public class RigidbodyController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private PlayerController player;
    [SerializeField] private CapsuleCollider capsuleCollider;
    private Rigidbody rb;
    private Animator animator;
    private PlayerSlap playerSlap;

    [Header("Input Setup")]
    [SerializeField] int playerID;

    [Header("Settings")]
    public float moveSpeed = 2f;
    public float inertiaFactor = 1f;
    public float dashForce = 10f;
    public float dashDuration = 0.2f;
    public float jumpForce = 7f;
    public float jumpCooldown = 0.85f;
    public float jumpCooldownTimer;

    [SerializeField] float fallMultiplier = 2.5f; //Variable for better jump/gravity control
    [SerializeField] float lowJumpMultiplier = 2f; //Variable for better jump/gravity control

    [Header("Ground Check")]
    public Transform groundCheck;
    public float groundDistance = 0.2f;
    public LayerMask groundMask;


    bool initialized = false;
    private Player playerInput;
    public bool isJumping, isGrounded;
    Vector3 moveDir;
    Quaternion targetRot;

    void Awake()
    {
        playerSlap = GetComponent<PlayerSlap>();
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody>();
        rb.interpolation = RigidbodyInterpolation.Interpolate;
        rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationY | RigidbodyConstraints.FreezeRotationZ;
    }

    void Initialize()
    {
        playerInput = ReInput.players.GetPlayer(playerID);
        playerSlap.SetPlayer(playerInput);
        initialized = true;
        Debug.Log("RigidbodyController initialized for Player " + playerID);
    }

    void Update()
    {
        if (!ReInput.isReady) return; // Exit if Rewired isn't ready. This would only happen during a script recompile in the editor.
        if (!initialized) Initialize();
        if (!rb || !groundCheck || Camera.main == null) return;
        Debug.Log("Updating RigidbodyController for Player " + playerID);
        if (PauseMenuManager.isPaused) return; //se o jogo está pausado, não precisa verificar esses inputs especificamente

        UpdateGroundedStatus();
        HandleMovementInput();
        HandleJumpInput();
        UpdateAnimator();

        if (jumpCooldownTimer>0)
        {
            jumpCooldownTimer -= Time.deltaTime;
            if (jumpCooldownTimer < 0f) jumpCooldownTimer = 0f; // Garante que não fique negativo
        }

        // Aplicar gravidade extra para deixar a queda mais rápida 
        if (rb.velocity.y < 0)
            rb.velocity += Vector3.up * Physics.gravity.y * (fallMultiplier - 1) * Time.deltaTime;

        else if (rb.velocity.y > 0 && !playerInput.GetButton("Jump"))
            rb.velocity += Vector3.up * Physics.gravity.y * (lowJumpMultiplier - 1) * Time.deltaTime;
    }

    void UpdateGroundedStatus()
    {
        isGrounded = Physics.CheckSphere(groundCheck.position, capsuleCollider.radius, groundMask);
        if (isGrounded) isJumping = false;
    }

    void Jump()
    {
        if (!isGrounded || isJumping || !rb) return;

        rb.velocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);
        rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        if (animator) animator.SetTrigger("Jump");
        isJumping = true;
        jumpCooldownTimer = jumpCooldown;
    }

    void HandleMovementInput()
    {
        if (!isGrounded || !(player.GetCurrentState() == PlayerState.Standing)) //Verifica se está no chão ou não está na condição Standing
            return;

        Debug.Log("Handling movement input for Player " + playerID);
        Vector2 moveInput = new Vector2(playerInput.GetAxis("Move Horizontal"), playerInput.GetAxis("Move Vertical"));
        Vector2 input = playerSlap && !playerSlap.GetIsSlapping() ? moveInput : Vector2.zero;

        if (input.sqrMagnitude > 0.01f)
        {
            Vector3 camForward = Camera.main.transform.forward;
            camForward.y = 0;
            Vector3 camRight = Camera.main.transform.right;
            camRight.y = 0;

            moveDir = (camForward.normalized * input.y + camRight.normalized * input.x).normalized;
            targetRot = Quaternion.LookRotation(moveDir);
        }
        else moveDir = Vector3.zero;
    }

    void HandleJumpInput()
    {
        if (playerInput.GetButtonDown("Dash") && !(player.GetCurrentState() == PlayerState.Standing)) //Verifica se não está na condição Standing
        {
            player.TryingToWakeUp();
            return;
        }

        if (playerInput.GetButtonDown("Dash") && !isJumping && isGrounded && jumpCooldownTimer <= 0)
        {
            Jump();
            return;
        }
    }

    void UpdateAnimator()
    {
        if (animator)
        {
            animator.SetBool("isMoving", moveDir != Vector3.zero);
            if (!isGrounded)
            {
                animator.SetBool("isJumping", true);
                animator.SetBool("isMoving", false);
            }
            else animator.SetBool("isJumping", false);
        }
    }

    void FixedUpdate()
    {
        if (!rb) return;

        if (isJumping)
        {
            PerformDash();
            return;
        }
        ApplyMovement();
        RotateTowardsMovementDirection();
    }

    void PerformDash()
    {
        /*if (!isGrounded) return;

        rb.MovePosition(rb.position + dashDir * (dashForce / dashDuration) * Time.fixedDeltaTime);
        dashTimer -= Time.fixedDeltaTime;
        if (dashTimer <= 0f)
            isJumping = false;*/
    }

    void ApplyMovement()
    {
        if (!isGrounded) return;

        Vector3 currentVel = rb.velocity;
        float newX = Mathf.Lerp(currentVel.x, moveDir.x * moveSpeed, inertiaFactor);
        float newZ = Mathf.Lerp(currentVel.z, moveDir.z * moveSpeed, inertiaFactor);
        rb.velocity = new Vector3(newX, currentVel.y, newZ);
    }

    void RotateTowardsMovementDirection()
    {
        if (!isGrounded) return;
        if (moveDir != Vector3.zero) rb.MoveRotation(targetRot);
    }

    public void SetPlayerId(int id) => playerID = id;
}
