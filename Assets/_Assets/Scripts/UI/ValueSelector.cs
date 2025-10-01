using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using Rewired;
using Rewired.Integration.UnityUI;

[RequireComponent(typeof(UnityEngine.UI.Selectable))] // Garantir que seja um bot�o ou elemento selecion�vel
public class ValueSelector : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private TextMeshProUGUI valueText;
    [SerializeField] private RewiredEventSystem rewiredEventSystem;

    [Header("Options")]
    [SerializeField] private string[] options;

    [Header("Rewired Input")]
    [SerializeField] private string uiLeft = "UI_Left";
    [SerializeField] private string uiRight = "UI_Right";

    // Refer�ncia para o EventSystem espec�fico
    [SerializeField] private EventSystem targetEventSystem;

    private int currentIndex = 2;
    private Player systemPlayer;


    private void Start()
    {
        systemPlayer = ReInput.players.SystemPlayer;
        UpdateValueText();
    }


    private void Update()
    {
        // S� reage se o GameObject que cont�m esse script estiver selecionado
        if (EventSystem.current.currentSelectedGameObject == this.gameObject)
        {
            HandleInput();
        }

    }

    private void HandleInput()
    {
        if (systemPlayer.GetButtonDown(uiLeft))
        {
            currentIndex = (currentIndex - 1 + options.Length) % options.Length;
            UpdateValueText();
            OnValueChanged();
            //Debug.Log("UI_Left acionado");
        }

        if (systemPlayer.GetButtonDown(uiRight))
        {
            currentIndex = (currentIndex + 1) % options.Length;
            UpdateValueText();
            OnValueChanged();
            //Debug.Log("UI_Right acionado");
        }
    }

    private void UpdateValueText()
    {
        if (valueText != null)
            valueText.text = options[currentIndex];
    }

    // Chamado sempre que o valor muda
    private void OnValueChanged()
    {
        Debug.Log($"{gameObject.name} selecionado: {options[currentIndex]}");
        // Aqui voc� pode notificar outro script, por exemplo MatchSetupManager
    }

    public int GetCurrentIndex() => currentIndex;
    public string GetCurrentValue() => options[currentIndex];
}