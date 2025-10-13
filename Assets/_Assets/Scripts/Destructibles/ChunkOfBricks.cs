using UnityEngine;
using System.Collections.Generic;

public class ChunkOfBricks : MonoBehaviour
{
    [Header("Chunk Monitoring")]
    public bool chunkReleased = false;
    public float releaseDelay = 0.2f; // pequeno atraso opcional para soltar tudo junto

    private List<Rigidbody> brickBodies = new List<Rigidbody>();
    private List<Collider> brickColliders = new List<Collider>();
    private List<ResponsiveProp> responsiveProps = new List<ResponsiveProp>();

    void Awake()
    {
        foreach (Transform child in transform)
        {
            var rb = child.GetComponent<Rigidbody>();
            var col = child.GetComponent<Collider>();
            var prop = child.GetComponent<ResponsiveProp>();

            if (rb && col)
            {
                rb.isKinematic = true; // começa travado
                col.enabled = true;
                brickBodies.Add(rb);
                brickColliders.Add(col);
            }

            if (prop)
                responsiveProps.Add(prop);
        }
    }

    void Update()
    {
        if (chunkReleased) return;

        // verifica se algum tijolo virou detached
        foreach (var prop in responsiveProps)
        {
            if (prop != null && prop.detached)
            {
                ReleaseAllBricks();
                return;
            }
        }
    }

    void ReleaseAllBricks()
    {
        chunkReleased = true;

        StartCoroutine(DelayedRelease());
    }

    System.Collections.IEnumerator DelayedRelease()
    {
        yield return new WaitForSeconds(releaseDelay);

        for (int i = 0; i < brickBodies.Count; i++)
        {
            if (brickBodies[i] != null)
            {
                brickBodies[i].isKinematic = false;
            }
        }

        // feedbackzinho extra opcional
        foreach (var rb in brickBodies)
        {
            if (rb != null)
                rb.AddExplosionForce(150f, transform.position, 2.5f);
        }
    }
}