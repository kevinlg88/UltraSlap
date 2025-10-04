using UnityEngine;

[RequireComponent(typeof(Collider))]
public class ConveyorBelt : MonoBehaviour
{
    public float speed = 2f; // velocidade da esteira

    void OnCollisionStay(Collision collision)
    {
        Rigidbody rb = collision.rigidbody;
        if (rb != null && !rb.isKinematic)
        {
            // Pega o eixo Z local da esteira (frente)
            //Vector3 dir = transform.forward;
            Vector3 dir = Vector3.ProjectOnPlane(transform.forward, Vector3.up);

            // (opcional) garante que não empurre pra cima/baixo
            dir.y = 0f;
            dir.Normalize();

            // Empurra o objeto ao longo do eixo Z local
            rb.AddForce(dir * speed, ForceMode.VelocityChange);
        }
    }
}