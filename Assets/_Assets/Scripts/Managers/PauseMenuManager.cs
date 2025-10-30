using UnityEngine;
using UnityEngine.UI;
using Rewired;
using Rewired.Integration.UnityUI;

public class PauseMenuManager : MonoBehaviour
{
    [SerializeField] private RewiredEventSystem rewiredEventSystem;

    [Header("UI References")]
    [SerializeField] private GameObject pauseMenuUI;
    [SerializeField] private Button resumeButton;
    [SerializeField] private Button optionsButton;
    [SerializeField] private Button quitButton;

    public static bool isPaused = false;

    void Start()
    {
        pauseMenuUI.SetActive(false);

        resumeButton.onClick.AddListener(ResumeGame);
        optionsButton.onClick.AddListener(OpenOptions);
        quitButton.onClick.AddListener(QuitGame);
    }

    void Update()
    {
        if (!UIManager.canPause) return; // Ignora inputs de pausa se não permitido

        // por enquanto: global input
        if (ReInput.players.GetSystemPlayer().GetButtonDown("Pause"))
        {
            if (!isPaused) PauseGame();
            else ResumeGame();
        }
    }

    private void PauseGame()
    {
        isPaused = true;
        pauseMenuUI.SetActive(true);
        Time.timeScale = 0f;
        
        //resumeButton.Select(); // selecionar o botão inicial

        rewiredEventSystem.SetSelectedGameObject(resumeButton.gameObject); // selecionar o botão inicial
    }

    public void ResumeGame()
    {
        foreach (PlayerSlap slap in FindObjectsOfType<PlayerSlap>())
        {
            slap.BlockNewSlapsAfterUnpause();
        }

        isPaused = false;
        pauseMenuUI.SetActive(false);
        Time.timeScale = 1f;
    }

    private void OpenOptions()
    {
        Debug.Log("Open Options clicked.");
    }

    private void QuitGame()
    {
        Debug.Log("Quit Game clicked.");
        Application.Quit();
    }
}
