using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MoreMountains.Feedbacks;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.UI;
using MaskTransitions;
using static System.Net.Mime.MediaTypeNames;
using System.Diagnostics;


public class LevelManager : MonoBehaviour
{
    [SerializeField] private MMFeedbacks levelSong;
    private CameraZoom cameraZoom; // Referência para CameraZoom

    [SerializeField] private int numberOfPlayers = 2; // Número fixo de jogadores
    [SerializeField] private PlayerManager[] players; // Jogadores na partida

    [SerializeField] private TextMeshProUGUI matchWinnerText; // Referência ao texto da UI

    public bool[] isAlive; // Verifica quais players estão vivos
    public int[] victoriesCounter; // Contagem de vitória para cada jogador
    public int maxVictories = 3;
    public GameObject victoryHandPrefab; // Prefab do ícone de vitória ser instaanciado para representar quantas vezes cada player venceu
    public GameObject[] victoryCounterGroup; //Objeto onde fica o contador visual de vitórias, contendo o prefab do ícone de vitória
    public List<int> teamVictoryGroupMap = new List<int>(); // Mapeia o ID de cada time para sua posição correspondente no array 'victoryCounterGroup'
    [SerializeField] private GameObject transitionUI;

    private static LevelManager instance;

    public KeyCode continueKey;  // A tecla que vai despausar o jogo e executar a transição
    public float transitionTime = 3f;  // Exemplo de tempo de transição

    public UnityEngine.UI.Image[] TeamTagsIcons = new UnityEngine.UI.Image[8]; // Array de TeamTags para times de players por cor (material)
    public Renderer[] playerRenderers; // Referência para os Renderers dos players (para pegar a cor do material)

    [Header("List of Pre-defined Teams Materials")]
    [SerializeField] private Material[] teamMaterials; // 8 possíveis team materiais



    void Awake()
    {
        if (instance != null)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
        DontDestroyOnLoad(gameObject);
    }

    void Start()
    {

        matchWinnerText.gameObject.SetActive(false);

        // Inicializar os arrays com base no número de jogadores
        isAlive = new bool[numberOfPlayers];
        victoriesCounter = new int[numberOfPlayers];


        // Definir todos os jogadores como vivos no início
        for (int i = 0; i < numberOfPlayers; i++)
        {
            isAlive[i] = true;
        }


        startMatch();
    }

    private void Update()
    {
        // Verifica se o botão continuar foi pressionada quando o jogador está na tela de transição
        if (Input.GetKeyDown(continueKey) && Time.timeScale == 0f)
        {
            SceneManager.LoadScene(0); // Recarrega a cena para um novo round

            
            transitionUI.SetActive(false); //Faz a tela de transição, com TeamTagsIcons e outros botões, ficar inativa


            StartCoroutine(WaitForSceneInitialization());


            // Chama o método para executar a transição de fim
            TransitionManager.Instance.PlayEndHalfTransition(transitionTime);

            // Despausa o jogo
            Time.timeScale = 1f;

        }


    }

    private IEnumerator WaitForSceneInitialization()
    {
        yield return new WaitForSeconds(0.4f);

        players = FindObjectsOfType<PlayerManager>();  // Atualiza a lista de jogadores APÓS o carregamento da cena

        UnityEngine.Debug.Log($"Jogadores encontrados após cena recarregada: {players.Length}");

        AssignTeamMaterials();

        cameraZoom = FindObjectOfType<CameraZoom>();
        if (cameraZoom != null)
        {
            cameraZoom.FindPlayersInScene();
        }

        yield return new WaitForSeconds(0.6f);

    }


    public void startMatch()
    {
        levelSong.PlayFeedbacks();

        // Encontra todos os objetos que possuem o script PlayerManager
        players = FindObjectsOfType<PlayerManager>();

        // Ajusta o número de jogadores dinamicamente com base nos objetos encontrados
        numberOfPlayers = players.Length;

        // Redimensiona o array de renderizadores
        playerRenderers = new Renderer[numberOfPlayers];

        for (int i = 0; i < numberOfPlayers; i++)
        {
            // Busca pelo filho chamado "Cube.002" dentro do player
            Transform child = players[i].transform.Find("Cube.002");

            if (child != null)
            {
                playerRenderers[i] = child.GetComponent<Renderer>();
            }
            else
            {
                UnityEngine.Debug.LogWarning($"Cube.002 não encontrado no Player {i}");
            }
        }

        int tagIndex = 0;
        List<Material> assignedMaterials = new List<Material>();  // Lista para rastrear materiais já atribuídos

        AssignTeamMaterials();

        for (int i = 0; i < numberOfPlayers; i++)
        {
            if (playerRenderers[i] != null && tagIndex < TeamTagsIcons.Length)
            {
                Material playerMaterial = playerRenderers[i].material;

                // Verifica se o material já foi atribuído
                if (!assignedMaterials.Contains(playerMaterial))
                {
                    TeamTagsIcons[tagIndex].color = playerMaterial.color;  // Define a cor da tag para o material do jogador
                    assignedMaterials.Add(playerMaterial);  // Adiciona o material à lista

                    //teamVictoryGroupMap[x] = playerRenderers[i].GetComponentInParent<PlayerManager>().team; //Mapeia o ID de cada time para sua posição correspondente no array 'victoryCounterGroup'

                    int teamID = playerRenderers[i].GetComponentInParent<PlayerManager>().team;
                    teamVictoryGroupMap.Add(teamID);

                    tagIndex++;  // Avança para a próxima tag
                }
            }
        }


    }

    public void AssignTeamMaterials()
    {
        // Encontra todos os jogadores ativos na cena
        PlayerManager[] players = FindObjectsOfType<PlayerManager>();

        foreach (PlayerManager player in players)
        {
            int teamIndex = Mathf.Clamp(player.team, 0, teamMaterials.Length - 1);
            Material selectedMaterial = teamMaterials[teamIndex];

            player.ApplyTeamMaterial(selectedMaterial);
        }
    }

    public void newRound()
    {

        // Garante que todos os jogadores estejam vivos para um novo round
        for (int i = 0; i < numberOfPlayers; i++)
        {
            isAlive[i] = true;
        }

        // ⚠️ IMPORTANTE: Atualiza a lista de jogadores para evitar referências nulas
        players = FindObjectsOfType<PlayerManager>();

        TransitionManager.Instance.PlayStartHalfTransition(3f, 0f, () =>
        {
            // Pausa o jogo quando a transição estiver completa
            Time.timeScale = 0f;

            transitionUI.SetActive(true); //Faz a tela de transição, com TeamTagsIcons e outros botões, ficar ativa
        });
    }


    public void roundVictory(int playerIndex, int playerTeam)
    {
        isAlive[playerIndex] = false; // Marca o jogador como eliminado

        int alivePlayers = 0;
        int winnerIndex = -1;

        UnityEngine.Debug.Log($"Time que ganhou originalmente: {playerTeam}");

        // Conta quantos jogadores ainda estão vivos
        for (int i = 0; i < numberOfPlayers; i++)
        {
            if (isAlive[i])
            {
                alivePlayers++;
                winnerIndex = i; // Último jogador vivo
            }
        }

        if (alivePlayers == 1) // Se apenas um jogador restou, ele venceu
        {
            // Verifica se players está atualizado e winnerIndex é válido
            if (players == null || winnerIndex < 0 || winnerIndex >= players.Length || players[winnerIndex] == null)
            {
                UnityEngine.Debug.LogError($"Erro no roundVictory: Índice inválido ({winnerIndex}) ou referência nula.");
                return;
            }
            
            //UnityEngine.Debug.Log($"Time que ganhou originalmente: {playerTeam}");
            //UnityEngine.Debug.Log($"Outro: {teamVictoryGroupMap.IndexOf(playerTeam)}");

            victoriesCounter[teamVictoryGroupMap.IndexOf(playerTeam)]++; // Incrementa a contagem de vitórias

            // Apenas atualiza os contadores se o jogador ainda existir
            if (victoriesCounter[teamVictoryGroupMap.IndexOf(playerTeam)] > 0)
            {
                UpdateVictoryCounters(players[winnerIndex].team);
            }

            if (victoriesCounter[teamVictoryGroupMap.IndexOf(playerTeam)] >= maxVictories)
            {
                matchWinnerText.text = $"Time {playerTeam} Venceu a Partida!";
                matchWinnerText.gameObject.SetActive(true);

                TransitionManager.Instance.PlayEndHalfTransition(transitionTime);
            }
            else
            {
                newRound(); // Inicia um novo round
            }
        }

    }

    public void UpdateVictoryCounters(int winnerTeam)
    {
        //UnityEngine.Debug.Log($"Time que ganhou originalmente: {winnerTeam}");
        //Procura o índice do time vencedor dentro da lista mapeada
        int counterIndex = teamVictoryGroupMap.IndexOf(winnerTeam);
        UnityEngine.Debug.Log($"Qual posição do icontag do ganhador: {teamVictoryGroupMap.IndexOf(winnerTeam)}");

        // Se encontrou um índice válido, instancia a vitória no local correto
        if (counterIndex >= 0 && counterIndex < victoryCounterGroup.Length)
        {
            GameObject counterGroup = victoryCounterGroup[counterIndex];
            Instantiate(victoryHandPrefab, counterGroup.transform);
        }
        else
        {
            UnityEngine.Debug.LogError($"Não foi possível encontrar um grupo de vitória para o time {winnerTeam}");
        }
    }


}