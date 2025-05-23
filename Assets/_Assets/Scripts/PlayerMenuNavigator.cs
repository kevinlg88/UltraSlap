using UnityEngine;
using UnityEngine.InputSystem;
using MoreMountains.Feedbacks;
using System.Diagnostics;

public class PlayerMenuNavigator : MonoBehaviour
{
    public GameObject[] customizationOptions; // Lista de GameObjects vazios representando partes customiz�veis
    public int currentOptionIndex = 0;

    public InputActionAsset inputActions; // Refer�ncia ao InputActionAsset configurado no Unity (Player1)

    private InputAction moveAction; //Para mover entre op��es
    private InputAction confirmAction; //Para confirmar escolhas
    private InputAction backAction; //Para desfazer escolhas

    [SerializeField] private MMFeedbacks scrollCategory;
    [SerializeField] private MMFeedbacks switchOption;

    [SerializeField] private float arrowBlinkDuration = 0.2f;

    // Refer�ncia para a CustomizationLibrary
    public CustomizationLibrary customizationLibrary; // A biblioteca de customiza��o do personagem
    public int currentSkinColorIndex = 0; // �ndice para a lista de skinMaterials
    [SerializeField] private TeamMaterialLibrary teamMaterialLibrary; //Library de team materials

    // Refer�ncias para a cor da pele do personagem
    public Renderer headRenderer;

    [SerializeField] private GameObject playerModel;
    [SerializeField] private GameObject customizationUI;
    [SerializeField] private GameObject readyTxt;

    void Start()
    {
        // Busca a a��es de navega��o, Confirm e Back, dentro do Action Map chamado "UI"
        moveAction = inputActions.FindActionMap("UI").FindAction("Navigation");
        confirmAction = inputActions.FindActionMap("UI").FindAction("Confirm");
        backAction = inputActions.FindActionMap("UI").FindAction("Back");

        moveAction.Enable();
        moveAction.performed += ctx => Navigate(ctx.ReadValue<Vector2>());

        confirmAction.Enable();
        backAction.Enable();
        confirmAction.performed += ctx => OnConfirm();
        backAction.performed += ctx => OnBack();

        UpdateCustomizationSelection(); // Inicializa visibilidade correta

        //TODO: Implement player view
        //playerModel.GetComponent<PlayerManager>().ApplyTeamMaterial(teamMaterialLibrary.teamMaterials[playerModel.GetComponent<PlayerManager>().team].material);


    }

    void OnDisable()
    {
        moveAction.Disable();
        confirmAction.Disable();
        backAction.Disable();
    }

    void OnConfirm()
    {
        if (!playerModel.activeSelf)
            playerModel.SetActive(true);
        else {
            readyTxt.SetActive(true);
            customizationUI.SetActive(false);
        }

        
    }

    void OnBack()
    {
        if (playerModel.activeSelf && !readyTxt.activeSelf)
            playerModel.SetActive(false);
        else if (playerModel.activeSelf && readyTxt.activeSelf)
        {
            readyTxt.SetActive(false);
            customizationUI.SetActive(true);
        }

        currentOptionIndex = 0;
        UpdateCustomizationSelection();
    }

    void Navigate(Vector2 direction)
    {
        if (!playerModel.activeSelf)
        {
            return;
        }


        // Detecta a movimenta��o para cima ou para baixo (op��es de navega��o de menu)
        if (direction.y > 0.5f)
        {
            // Movimento para cima: Descer o �ndice
            currentOptionIndex = Mathf.Max(0, currentOptionIndex - 1);
            UpdateCustomizationSelection();

            scrollCategory.PlayFeedbacks();

        }
        else if (direction.y < -0.5f)
        {
            // Movimento para baixo: Subir o �ndice
            currentOptionIndex = Mathf.Min(customizationOptions.Length - 1, currentOptionIndex + 1);
            UpdateCustomizationSelection();

            scrollCategory.PlayFeedbacks();
        }
        else if (direction.x < -0.5f)
        {
            // Movimento para a esquerda (alterar a op��o)
            switchOption.PlayFeedbacks();
            BlinkArrow("LeftArrow");

            // Caso esteja na categoria de SkinColor, altera o material
            if (currentOptionIndex == 1) // Corrigido para currentSkinColorIndex
            {
                ChangeSkinMaterial(false); // Passar para o pr�ximo material
            }
        }
        else if (direction.x > 0.5f)
        {
            // Movimento para a direita (alterar a op��o)
            switchOption.PlayFeedbacks();
            BlinkArrow("RightArrow");

            // Caso esteja na categoria de SkinColor, altera o material
            if (currentOptionIndex == 1) // Corrigido para currentSkinColorIndex
            {
                ChangeSkinMaterial(true); // Passar para o material anterior
            }
        }
    }

    void UpdateCustomizationSelection()
    {
        for (int i = 0; i < customizationOptions.Length; i++)
        {
            customizationOptions[i].SetActive(i == currentOptionIndex);
        }
    }

    void BlinkArrow(string arrowName)
    {
        Transform arrow = customizationOptions[currentOptionIndex].transform.Find(arrowName);
        if (arrow != null)
        {
            StartCoroutine(Blink(arrow.gameObject));
        }
    }

    System.Collections.IEnumerator Blink(GameObject arrow)
    {
        arrow.SetActive(false);
        yield return new WaitForSeconds(arrowBlinkDuration);
        arrow.SetActive(true);
    }

    // Fun��o para alterar a cor da pele
    void ChangeSkinMaterial(bool next)
    {
        int skinMaterialCount = customizationLibrary.skinMaterials.Count;

        if (skinMaterialCount == 0)
        {
            // Se n�o houver materiais na lista, n�o faz nada
            return;
        }

        // Condicional para definir a navega��o circular (ir para o pr�ximo ou voltar para o anterior)
        if (next)
        {
            // Avan�a para o pr�ximo material, retornando ao �ndice 0 quando chegar ao final
            currentSkinColorIndex = (currentSkinColorIndex + 1) % skinMaterialCount;
        }
        else
        {
            // Retrocede para o material anterior, retornando ao �ltimo �ndice quando chegar ao in�cio
            currentSkinColorIndex = (currentSkinColorIndex - 1 + skinMaterialCount) % skinMaterialCount;
        }

        // Acessa o material da op��o selecionada da lista skinMaterials
        Material skinMaterial = customizationLibrary.skinMaterials[currentSkinColorIndex].material;

        // Aplica o material na pele usando as refer�ncias corretas
        headRenderer.material = skinMaterial;

    }
}