using UnityEngine;

public class CloudSpawner : MonoBehaviour
{
    [SerializeField] private GameObject cloudPrefab;
    [SerializeField] private float minSpawnInterval = 1f;
    [SerializeField] private float maxSpawnInterval = 3f;

    private float timer;
    private float nextSpawnTime;
    private MeshRenderer meshRenderer;

    void Start()
    {
        meshRenderer = GetComponent<MeshRenderer>();
        SetNextSpawnTime();
    }

    void Update()
    {
        timer += Time.deltaTime;
        if (timer >= nextSpawnTime)
        {
            SpawnCloud();
            SetNextSpawnTime();
        }
    }

    void SpawnCloud()
    {
        if (meshRenderer == null || cloudPrefab == null) return;

        // Tamanho do plano em local space
        Vector3 localSize = transform.InverseTransformVector(meshRenderer.bounds.size);
        Vector3 localCenter = transform.InverseTransformPoint(meshRenderer.bounds.center);

        // Sorteia posição aleatória em X e Z locais
        float randX = Random.Range(-localSize.x / 2f, localSize.x / 2f);
        float randZ = Random.Range(-localSize.z / 2f, localSize.z / 2f);

        // Posição local (sempre no topo do Y local do plano)
        Vector3 localPos = new Vector3(localCenter.x + randX, localCenter.y + localSize.y / 2f, localCenter.z + randZ);

        // Converte para posição global
        Vector3 worldPos = transform.TransformPoint(localPos);

        // Instancia a nuvem
        GameObject cloud = Instantiate(cloudPrefab, worldPos, Quaternion.identity);

        // Rotação: spawner + 90° no eixo X local
        cloud.transform.rotation = transform.rotation * Quaternion.Euler(90f, 0f, 0f);
    }

    void SetNextSpawnTime()
    {
        timer = 0f;
        nextSpawnTime = Random.Range(minSpawnInterval, maxSpawnInterval);
    }
}