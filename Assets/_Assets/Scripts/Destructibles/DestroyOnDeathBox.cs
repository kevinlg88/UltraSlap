using UnityEngine;

public class DestroyOnDeathBox : MonoBehaviour
{
    [Tooltip("Se marcado, o objeto tamb�m � destru�do caso entre em trigger, n�o s� colis�o.")]
    public bool destroyOnTrigger = true;

    // Detecta colis�o f�sica
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("DeathBox"))
        {
            Destroy(gameObject);
        }
    }

    // Detecta trigger (se voc� usar triggers no DeathBox)
    private void OnTriggerEnter(Collider other)
    {
        if (destroyOnTrigger && other.CompareTag("DeathBox"))
        {
            Destroy(gameObject);
        }
    }
}