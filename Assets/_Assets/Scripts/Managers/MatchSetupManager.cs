using UnityEngine;
using MoreMountains.Feedbacks;
using Rewired.Integration.UnityUI;
using Rewired;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;
using TMPro;


public enum SetupState
{
    CharacterSetup,
    MatchSetup
}

public class MatchSetupManager : MonoBehaviour
{
    public SetupState currentSetupState = SetupState.CharacterSetup;

    private Player systemPlayer;

    [Header("UI References")]
    [SerializeField] private RewiredEventSystem rewiredEventSystem;

    [Header("Input Setup")]
    [SerializeField] string joinAction = "Join";
    [SerializeField] string cancelAction = "Cancel";

    [Header("MMFeedbacks")]
    [SerializeField] private MMFeedbacks ReadyConfirmMMFeedbacks;

    [Header("First Selected Options")]
    [SerializeField] private GameObject startingCharacterSetupFirst;
    [SerializeField] private GameObject startingMatchSetupFirst;


    // Start is called before the first frame update
    void Start()
    {
        // SystemPlayer é sempre válido e pega input global
        systemPlayer = ReInput.players.SystemPlayer;

    }


    // Update is called once per frame
    void Update()
    {
        GetPlayerInput();

        if (EventSystem.current.currentSelectedGameObject != null)
        {
            Debug.Log("Selecionado: " + rewiredEventSystem.currentSelectedGameObject.name);
        }
        else
        {
            Debug.Log("Nada selecionado");
        }
    }

    private void GetPlayerInput()
    {
        if (systemPlayer.GetButtonDown(joinAction))
        {

            Debug.Log("Botão Join apertado (SystemPlayer)");
        }
    }

    public void SwitchToMatchSetup()
    {
        currentSetupState = SetupState.MatchSetup;

        //EventSystem.current.SetSelectedGameObject(startingMatchSetupFirst); //Seleciona o primeiro botão
    }

    public void SwitchToCharacterSetup()
    {
        currentSetupState = SetupState.CharacterSetup;
        //EventSystem.current.SetSelectedGameObject(startingCharacterSetupFirst);
    }


}
