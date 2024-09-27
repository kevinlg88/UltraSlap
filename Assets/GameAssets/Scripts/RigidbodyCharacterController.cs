using FIMSpace.FProceduralAnimation;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class RigidbodyCharacterController : MonoBehaviour
{
    [Header("Movement Setup")]
    public float moveSpeed = 5f;
    public float rotationSpeed = 720f; // Velocidade de rotação
    public float jumpForce = 7f;

    [Header("References")]
    [SerializeField] Animator animator;
    [SerializeField] RagdollAnimator ragdoll;
    private Rigidbody rb;
    private bool isGrounded;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true; // Evitar que o personagem caia ao colidir
    }

    private void Update()
    {
        // Verifica se o personagem está no chão
        isGrounded = Physics.Raycast(transform.position, Vector3.down, 1.1f);

        // Movimento horizontal
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        Vector3 movement = new Vector3(horizontal, 0, vertical).normalized * moveSpeed;

        if (movement.magnitude > 0.1f)
        {
            // Movimenta o personagem com base na direção da câmera
            Move(movement);
        }

        // Pular
        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            Jump();
        }
    }

    private void Move(Vector3 movement)
    {
        // Transforma o movimento do espaço local para o mundo com base na direção da câmera
        Vector3 moveDirection = Camera.main.transform.TransformDirection(movement);
        moveDirection.y = 0; // Manter o movimento apenas no plano XZ

        // Aplicar o movimento diretamente à velocidade do Rigidbody
        rb.velocity = new Vector3(moveDirection.x, rb.velocity.y, moveDirection.z);

        // Rotacionar o personagem na direção do movimento
        Quaternion targetRotation = Quaternion.LookRotation(moveDirection);
        transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
    }

    private void Jump()
    {
        //rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        animator.SetTrigger("Punch");
    }
}
