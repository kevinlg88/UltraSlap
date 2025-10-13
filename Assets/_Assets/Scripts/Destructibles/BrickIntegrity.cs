using UnityEngine;

public class BrickIntegrity : MonoBehaviour
{
    [Header("Integrity Settings")]
    public float minIntegrity = 50f;   // massa mínima enquanto não é "detached"
    public float detachedMass = 4f;    // massa quando solto
    public float decayRate = 10f;      // quanto de massa perde por tapa forte
    private ResponsiveProp responsiveProp;

    [Header("Impact Thresholds")]
    public float impactThreshold = 3f; // impacto mínimo para afetar
    public float detachThreshold = 1.0f; // quão longe pode se mover antes de soltar

    private Rigidbody rb;
    private float currentIntegrity;
    private Vector3 initialPosition;



    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        currentIntegrity = rb.mass;
        initialPosition = transform.position;

        responsiveProp = GetComponent<ResponsiveProp>();
    }

    void FixedUpdate()
    {
        // Se o tijolo se mover além do limite, considera-se solto
        if (!responsiveProp.detached && Vector3.Distance(transform.position, initialPosition) > detachThreshold)
        {
            Detach();
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (responsiveProp.detached) return;

        float impact = collision.relativeVelocity.magnitude;

        if (impact > impactThreshold)
        {
            currentIntegrity -= impact * decayRate * Time.deltaTime;
            currentIntegrity = Mathf.Max(currentIntegrity, minIntegrity);
            rb.mass = currentIntegrity;
        }
    }

    void Detach()
    {

        responsiveProp.detached = true;
        rb.mass = detachedMass;
        rb.isKinematic = false;
        rb.AddExplosionForce(100f, transform.position, 1f);
    }
}
