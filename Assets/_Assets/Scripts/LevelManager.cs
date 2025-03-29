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

    [SerializeField] private TextMeshProUGUI matchWinnerText; // Referência ao texto da UI

    public bool[] isAlive; // Verifica quais players estão vivos
    public int[] victoriesCounter; // Contagem de vitória para cada jogador
    public int maxVictories = 3;

    private static LevelManager instance;

    public KeyCode continueKey;  // A tecla que vai despausar o jogo e executar a transição
    public float transitionTime = 3f;  // Exemplo de tempo de transição

    public UnityEngine.UI.Image[] TeamTags = new UnityEngine.UI.Image[8]; // Array de TeamTags para times de players por cor (material)
    public Renderer[] playerRenderers; // Referência para os Renderers dos players (para pegar a cor do material)



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

            // Varre todas as TeamTags e as torna invisíveis
            for (int i = 0; i < TeamTags.Length; i++)
            {
                if (TeamTags[i] != null)
                {
                    TeamTags[i].gameObject.SetActive(false); // Torna a tag invisível
                }
            }

            StartCoroutine(WaitForSceneInitialization());



            // Despausa o jogo
            Time.timeScale = 1f;

            // Chama o método para executar a transição de fim
            TransitionManager.Instance.PlayEndHalfTransition(transitionTime);
        }
    }

    private IEnumerator WaitForSceneInitialization()
    {
        yield return new WaitForSeconds(0.5f);  // Adiciona uma pequena pausa para garantir que todos os objetos foram carregados                                      
        // Encontrar a instância da Camera na cena
        cameraZoom = FindObjectOfType<CameraZoom>();
        if (cameraZoom != null)                 // Chamar o método FindPlayersInScene para setar posição dos players na camera
        {
            cameraZoom.FindPlayersInScene();
        }
    }


    public void startMatch()
    {
        levelSong.PlayFeedbacks();

        // Encontra todos os objetos que possuem o script PlayerManager
        PlayerManager[] players = FindObjectsOfType<PlayerManager>();

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

        for (int i = 0; i < numberOfPlayers; i++)
        {
            if (playerRenderers[i] != null && tagIndex < TeamTags.Length)
            {
                Material playerMaterial = playerRenderers[i].material;

                // Verifica se o material já foi atribuído
                if (!assignedMaterials.Contains(playerMaterial))
                {
                    TeamTags[tagIndex].color = playerMaterial.color;  // Define a cor da tag para o material do jogador
                    assignedMaterials.Add(playerMaterial);  // Adiciona o material à lista
                    tagIndex++;  // Avança para a próxima tag
                }
            }
        }

    }

    public void newRound()
    {
        // Garante que todos os jogadores estejam vivos para um novo round
        for (int i = 0; i < numberOfPlayers; i++)
        {
            isAlive[i] = true;
        }

        TransitionManager.Instance.PlayStartHalfTransition(3f, 0f, () =>
        {
            // Pausa o jogo quando a transição estiver completa
            Time.timeScale = 0f;

            // Varre todas as TeamTags e torna visíveis apenas as que possuem uma cor diferente de branco
            for (int i = 0; i < TeamTags.Length; i++)
            {
                if (TeamTags[i].color != Color.white)
                {
                    TeamTags[i].gameObject.SetActive(true); // Torna a tag visível
                }
                else
                {
                    TeamTags[i].gameObject.SetActive(false); // Torna invisível se ainda for branca
                }
            }
        });
    }


    public void roundVictory(int playerIndex)
    {
        isAlive[playerIndex] = false; // Marca o jogador como eliminado

        int alivePlayers = 0;
        int winnerIndex = -1;

        // Conta quantos jogadores ainda estão vivos
        for (int i = 0; i < numberOfPlayers; i++)
        {
            if (isAlive[i])
            {
                alivePlayers++;
                winnerIndex = i; // Último jogador vivo
            }
        }

        // Se restar apenas um jogador, ele vence a rodada
        if (alivePlayers == 1 && winnerIndex != -1)
        {
            victoriesCounter[winnerIndex]++; // Incrementa vitórias do jogador vencedor
            UnityEngine.Debug.Log($"Player {winnerIndex + 1} venceu a rodada! Vitórias: {victoriesCounter[winnerIndex]}");

            // Verifica se o jogador atingiu o número máximo de vitórias
            if (victoriesCounter[winnerIndex] >= maxVictories)
            {
                UnityEngine.Debug.Log($"Player {winnerIndex + 1} Wins!");
                // Aqui pode adicionar uma tela de vitória ou resetar o jogo

                ShowMatchWinner(winnerIndex);
            }
            else
            {
                newRound(); // Reinicia a rodada
            }
        }


    }

    void ShowMatchWinner(int winnerIndex) //Exibe na tela quem ganhou a partida
    {
        if (matchWinnerText != null)
        {
            matchWinnerText.text = $"Player {winnerIndex + 1} Wins!";
            matchWinnerText.gameObject.SetActive(true);
        }
    }
}