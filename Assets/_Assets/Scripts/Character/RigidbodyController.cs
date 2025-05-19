using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody), typeof(PlayerInput))]
public class RigidbodyController : MonoBehaviour
{
    [Header("Components")]
    public Rigidbody rb;
    public Animator animator;
    public PlayerSlap playerSlap;

    [Header("Settings")]
    public float moveSpeed = 2f;
    public float inertiaFactor = 1f;
    public float dashForce = 10f;
    public float dashDuration = 0.2f;
    public float dashCooldown = 2f;

    [Header("Ground Check")]
    public Transform groundCheck;
    public float groundDistance = 0.2f;
    public LayerMask groundMask;

    InputAction moveInput, dashInput;

    bool isDashing, isGrounded;
    float dashTimer, nextDashTime;
    Vector3 dashDir, moveDir;
    Quaternion targetRot;

    void Awake()
    {
        var input = GetComponent<PlayerInput>();
        moveInput = input.currentActionMap.FindAction("Move");
        dashInput = input.currentActionMap.FindAction("Dash");

        rb = GetComponent<Rigidbody>();
        rb.interpolation = RigidbodyInterpolation.Interpolate;
        rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
    }

    void Update()
    {
        if (!rb || !groundCheck || Camera.main == null) return;

        isGrounded = Physics.Raycast(groundCheck.position, Vector3.down, groundDistance, groundMask);

        if(moveInput == null || dashInput == null) return;
        Vector2 input = playerSlap && !playerSlap.GetIsSlapping() ? moveInput.ReadValue<Vector2>() : Vector2.zero;

        if (input.sqrMagnitude > 0.01f)
        {
            Vector3 camForward = Camera.main.transform.forward;
            camForward.y = 0;
            Vector3 camRight = Camera.main.transform.right;
            camRight.y = 0;
            moveDir = (camForward.normalized * input.y + camRight.normalized * input.x).normalized;
            targetRot = Quaternion.LookRotation(moveDir);
        }
        else
        {
            moveDir = Vector3.zero;
        }

        if (dashInput != null && dashInput.WasPressedThisFrame() && !isDashing && isGrounded && Time.time >= nextDashTime)
        {
            isDashing = true;
            dashTimer = dashDuration;
            dashDir = transform.forward;
            nextDashTime = Time.time + dashCooldown;
            if (animator) animator.SetTrigger("Dash");
        }

        if (animator) animator.SetBool("isMoving", moveDir != Vector3.zero);
    }

    void FixedUpdate()
    {
        if (!rb) return;

        if (isDashing)
        {
            rb.MovePosition(rb.position + dashDir * (dashForce / dashDuration) * Time.fixedDeltaTime);
            dashTimer -= Time.fixedDeltaTime;
            if (dashTimer <= 0f) isDashing = false;
            return;
        }

        Vector3 currentVel = rb.velocity;
        float newX = Mathf.Lerp(currentVel.x, moveDir.x * moveSpeed, inertiaFactor);
        float newZ = Mathf.Lerp(currentVel.z, moveDir.z * moveSpeed, inertiaFactor);

        rb.velocity = new Vector3(newX, currentVel.y, newZ);

        if (moveDir != Vector3.zero)
            rb.MoveRotation(targetRot);
    }
}
