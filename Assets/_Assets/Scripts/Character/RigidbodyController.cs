﻿using Rewired;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class RigidbodyController : MonoBehaviour
{
    [Header("Components")]
    public Rigidbody rb;
    public Animator animator;
    public PlayerSlap playerSlap;

    [Header("Input Setup")]
    [SerializeField] int playerID;

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


    bool initialized = false;
    private Player player;
    bool isDashing, isGrounded;
    float dashTimer, nextDashTime;
    Vector3 dashDir, moveDir;
    Quaternion targetRot;


    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        rb.interpolation = RigidbodyInterpolation.Interpolate;
        rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationY | RigidbodyConstraints.FreezeRotationZ;
    }

    void Initialize()
    {
        player = ReInput.players.GetPlayer(playerID);
        this.GetComponent<PlayerSlap>().SetPlayer(player);
        initialized = true;
    }

    void Update()
    {
        if (!ReInput.isReady) return; // Exit if Rewired isn't ready. This would only happen during a script recompile in the editor.
        if (!initialized) Initialize();
        if (!rb || !groundCheck || Camera.main == null) return;

        isGrounded = Physics.Raycast(groundCheck.position, Vector3.down, groundDistance, groundMask);

        Vector2 moveInput = new Vector2(player.GetAxis("Move Horizontal"), player.GetAxis("Move Vertical"));
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
        else
        {
            moveDir = Vector3.zero;
        }

        if (player.GetButtonDown("Dash") && !isDashing && isGrounded && Time.time >= nextDashTime)
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

    public void SetPlayerId(int id) => playerID = id;
    
}
