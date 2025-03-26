using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MoreMountains.Feedbacks;
using UnityEngine.SceneManagement;
using TMPro; // Importa TextMeshPro

public class LevelManager : MonoBehaviour
{
    [SerializeField] private MMFeedbacks levelSong;
    [SerializeField] private int numberOfPlayers = 2; // Número fixo de jogadores

    [SerializeField] private TextMeshProUGUI matchWinnerText; // Referência ao texto da UI

    public bool[] isAlive; // Verifica quais players estão vivos
    public int[] victoriesCounter; // Contagem de vitória para cada jogador
    public int maxVictories = 3;

    private static LevelManager instance;

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

    public void startMatch()
    {
        levelSong.PlayFeedbacks();
    }

    public void newRound()
    {
        // Garante que todos os jogadores estejam vivos para um novo round
        for (int i = 0; i < numberOfPlayers; i++)
        {
            isAlive[i] = true;
        }

        SceneManager.LoadScene(0); // Recarrega a cena para um novo round
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