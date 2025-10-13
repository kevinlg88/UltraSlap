using UnityEngine;

public class DestroyOnDeathBox : MonoBehaviour
{
    [Tooltip("Se marcado, o objeto também é destruído caso entre em trigger, não só colisão.")]
    public bool destroyOnTrigger = true;

    // Detecta colisão física
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("DeathBox"))
        {
            Destroy(gameObject);
        }
    }

    // Detecta trigger (se você usar triggers no DeathBox)
    private void OnTriggerEnter(Collider other)
    {
        if (destroyOnTrigger && other.CompareTag("DeathBox"))
        {
            Destroy(gameObject);
        }
    }
}