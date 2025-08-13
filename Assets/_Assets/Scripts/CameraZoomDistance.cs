using UnityEngine;

public class CameraZoom : MonoBehaviour
{
    public static CameraZoom Instance;

    public Transform[] characters;  // Lista de jogadores na cena
    public Transform cameraTransform; // Transform da câmera

    public float minZoom = 5f; // Distância mínima da câmera
    public float maxZoom = 20f; // Distância máxima da câmera
    public float zoomFactor = 1.2f; // Influencia o ritmo de afastamento/aproximação da câmera quando os personagens se distanciam ou se aproximam
    public float zoomSpeed = 5f; // Velocidade de ajuste do zoom
    public Vector3 offset = new Vector3(0, 5, -10); // Offset da câmera
    public float heightThreshold = -4f; // Altura mínima para considerar um personagem ativo

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);

        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        FindPlayersInScene();
    }

    public void FindPlayersInScene() //Procura todos os jogadores na partida
    {
        // Encontra todas as instâncias do prefab Player baseando-se no script PlayerManager
        RigidbodyController[] playerScripts = FindObjectsOfType<RigidbodyController>();
        characters = new Transform[playerScripts.Length];
        //UnityEngine.Debug.Log($"Jogadores encontrados: {playerScripts.Length}");

        for (int i = 0; i < playerScripts.Length; i++)
        {
            characters[i] = playerScripts[i].transform;
            UnityEngine.Debug.Log($"Jogador {i + 1} encontrado: {characters[i].name}");

        }


        /*if (characters.Length < 2)
        {
            UnityEngine.Debug.LogWarning("Menos de 2 jogadores encontrados! Certifique-se de que os Players foram instanciados corretamente.");
        }
        else
        {
            UnityEngine.Debug.Log("Jogadores encontrados: " + characters.Length);
        }*/
    }

    void Update()
    {
        if (characters == null || characters.Length == 0 || cameraTransform == null)
        {
            FindPlayersInScene(); // Rebusca os players caso não existam
            return;
        }

        // Criar uma nova lista de personagens vivos
        Transform[] aliveCharacters = System.Array.FindAll(characters, c => c != null && c.position.y >= heightThreshold);

        // Destruir os personagens que caíram abaixo do threshold
        foreach (Transform character in characters)
        {
            if (character != null && character.position.y < heightThreshold)
            {
                Debug.Log($"Eliminando {character.name} por cair abaixo do limite!");
                Destroy(character.gameObject);
            }
        }

        if (aliveCharacters.Length == 0) return; // Se ninguém está vivo, não move a câmera

        Vector3 targetPosition = cameraTransform.position;
        float targetZoom = (maxZoom + minZoom) / 2;

        if (aliveCharacters.Length == 1)
        {
            targetPosition = aliveCharacters[0].position + offset.normalized * minZoom;
        }
        else
        {
            Vector3 midPoint = new Vector3();
            foreach (Transform tf in aliveCharacters) midPoint += tf.position;
            midPoint /= aliveCharacters.Length;
            float distance = Vector3.Distance(aliveCharacters[0].position, aliveCharacters[1].position);
            targetZoom = Mathf.Clamp(distance * zoomFactor, minZoom, maxZoom);
            targetPosition = midPoint + offset.normalized * targetZoom;
        }

        cameraTransform.position = Vector3.Lerp(cameraTransform.position, targetPosition, Time.deltaTime * zoomSpeed);
    }
}
