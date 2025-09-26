using UnityEngine;

public class CloudSpawner : MonoBehaviour
{
    [SerializeField] private GameObject[] cloudPrefabs; // vetor de nuvens
    [SerializeField] private float minSpawnInterval = 30f;
    [SerializeField] private float maxSpawnInterval = 60f;

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
        if (meshRenderer == null || cloudPrefabs == null) return;

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

        // Sorteia um prefab aleatório
        int randomIndex = Random.Range(0, cloudPrefabs.Length);
        GameObject chosenCloud = cloudPrefabs[randomIndex];

        // Instancia a nuvem
        GameObject cloud = Instantiate(chosenCloud, worldPos, Quaternion.identity);

        // Rotação: spawner + 90° no eixo Z local
        cloud.transform.rotation = transform.rotation * Quaternion.Euler(0f, 0f, 90f);
    }

    void SetNextSpawnTime()
    {
        timer = 0f;
        nextSpawnTime = Random.Range(minSpawnInterval, maxSpawnInterval);
    }
}