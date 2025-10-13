using UnityEngine;
using MoreMountains.Feedbacks;

[RequireComponent(typeof(Rigidbody))]
public class ResponsiveProp : MonoBehaviour
{
    [Header("General Settings")]
    public bool detached = true; // define se o objeto est� solto
    public float impactThreshold = 3f; // velocidade m�nima para reagir
    public float cooldown = 0.1f; // tempo m�nimo entre rea��es (evita spam)

    [Header("Feedbacks")]
    public MMFeedbacks impactFeedback; // som, part�culas, etc.

    private Rigidbody rb;
    private float lastImpactTime;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        lastImpactTime = -cooldown; // permite rea��o imediata na primeira colis�o
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (!detached) return; // s� reage se estiver solto

        float impactForce = collision.relativeVelocity.magnitude;

        if (impactForce >= impactThreshold && Time.time - lastImpactTime >= cooldown)
        {
            lastImpactTime = Time.time;

            // Dispara o feedback (som, part�culas, etc.)
            if (impactFeedback != null)
            {
                impactFeedback.PlayFeedbacks(transform.position);
            }
        }
    }
}
