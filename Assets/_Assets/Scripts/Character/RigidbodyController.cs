using System;
using UnityEngine;

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

    [Header("Animator Settings")]
    public Animator animator;

    [Header("Input Settings")]
    public bool enableInput = true;

    [Header("Gravity Settings")]
    public float gravityMultiplier = 1f;

    private Quaternion targetRotation;
    private Quaternion targetInstantRotation;
    private float rotationAngle = 0f;
    private float smoothRotationAngle = 0f;
    private bool isGrounded = true;

    [NonSerialized] public Vector2 localMoveDirection = Vector2.zero;
    public Vector3 worldMoveDirection { get; set; }
    public Vector3 currentAcceleration { get; private set; }

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
    }

    private void Update()
    {
        if (rigidbodyComponent == null || !enableInput) return;

        HandleMovementInput();
        UpdateMovement();
    }

    private void HandleMovementInput()
    {
        localMoveDirection = Vector2.zero;

        if (Input.GetKey(KeyCode.A)) localMoveDirection += Vector2.left;
        if (Input.GetKey(KeyCode.D)) localMoveDirection += Vector2.right;
        if (Input.GetKey(KeyCode.W)) localMoveDirection += Vector2.up;
        if (Input.GetKey(KeyCode.S)) localMoveDirection += Vector2.down;

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

        if (rotationSpeed > 0f && currentAcceleration != Vector3.zero)
        {
            rotationAngle = Mathf.SmoothDampAngle(rotationAngle, targetInstantRotation.eulerAngles.y, ref smoothRotationAngle, Mathf.Lerp(0.5f, 0.01f, rotationSpeed));
            targetRotation = Quaternion.Euler(0f, rotationAngle, 0f);
        }

        if (animator)
        {
            animator.SetBool("isMoving", isMoving);
        }

        float speed = movementSpeed;
        float acceleration = isMoving ? 5f * movementSpeed : 7f * movementSpeed;

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
}
