using UnityEngine;

public class CloudBehavior : MonoBehaviour
{
    [SerializeField] private float speed = 2f;       // Velocidade da nuvem no eixo X
    [SerializeField] private float destroyX = 50f;   // Posição X global onde a nuvem será destruída

    void Update()
    {
        // Move a nuvem no eixo X global
        transform.position += Vector3.right * speed * Time.deltaTime;

        // Verifica se já passou do limite
        if (transform.position.x >= destroyX)
        {
            Destroy(gameObject);
        }
    }
}
