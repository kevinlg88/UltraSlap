using UnityEngine;
using MoreMountains.Feedbacks;

[RequireComponent(typeof(Rigidbody))]
public class ResponsiveProp : MonoBehaviour
{
    [Header("General Settings")]
    public bool detached = true; // define se o objeto está solto
    public float impactThreshold = 3f; // velocidade mínima para reagir
    public float cooldown = 0.1f; // tempo mínimo entre reações (evita spam)

    [Header("Feedbacks")]
    public MMFeedbacks impactFeedback; // som, partículas, etc.

    private Rigidbody rb;
    private float lastImpactTime;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        lastImpactTime = -cooldown; // permite reação imediata na primeira colisão
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (!detached) return; // só reage se estiver solto

        float impactForce = collision.relativeVelocity.magnitude;

        if (impactForce >= impactThreshold && Time.time - lastImpactTime >= cooldown)
        {
            lastImpactTime = Time.time;

            // Dispara o feedback (som, partículas, etc.)
            if (impactFeedback != null)
            {
                impactFeedback.PlayFeedbacks(transform.position);
            }
        }
    }
}
