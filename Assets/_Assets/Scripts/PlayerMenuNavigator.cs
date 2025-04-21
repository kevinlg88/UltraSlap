using UnityEngine;
using UnityEngine.InputSystem;
using MoreMountains.Feedbacks;
using System.Diagnostics;

public class PlayerMenuNavigator : MonoBehaviour
{
    public GameObject[] customizationOptions; // Lista de GameObjects vazios representando partes customizáveis
    public int currentOptionIndex = 0;

    public InputActionAsset inputActions; // Referência ao InputActionAsset configurado no Unity (Player1)

    private InputAction moveAction; //Para mover entre opções
    private InputAction confirmAction; //Para confirmar escolhas
    private InputAction backAction; //Para desfazer escolhas

    [SerializeField] private MMFeedbacks scrollCategory;
    [SerializeField] private MMFeedbacks switchOption;

    [SerializeField] private float arrowBlinkDuration = 0.2f;

    // Referência para a CustomizationLibrary
    public CustomizationLibrary customizationLibrary; // A biblioteca de customização do personagem
    public int currentSkinColorIndex = 0; // Índice para a lista de skinMaterials
    [SerializeField] private TeamMaterialLibrary teamMaterialLibrary; //Library de team materials

    // Referências para as partes do corpo do personagem
    public Renderer headRenderer;
    public Renderer leftEarRenderer;
    public Renderer rightEarRenderer;

    [SerializeField] private GameObject playerModel;

    void Start()
    {
        // Busca a ações de navegação, Confirm e Back, dentro do Action Map chamado "UI"
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

        playerModel.GetComponent<PlayerManager>().ApplyTeamMaterial(teamMaterialLibrary.teamMaterials[playerModel.GetComponent<PlayerManager>().team].material);


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
    }

    void OnBack()
    {
        if (playerModel.activeSelf)
            playerModel.SetActive(false);

        currentOptionIndex = 0;
        UpdateCustomizationSelection();
    }

    void Navigate(Vector2 direction)
    {
        if (!playerModel.activeSelf)
        {
            return;
        }


        // Detecta a movimentação para cima ou para baixo (opções de navegação de menu)
        if (direction.y > 0.5f)
        {
            // Movimento para cima: Descer o índice
            currentOptionIndex = Mathf.Max(0, currentOptionIndex - 1);
            UpdateCustomizationSelection();

            scrollCategory.PlayFeedbacks();

        }
        else if (direction.y < -0.5f)
        {
            // Movimento para baixo: Subir o índice
            currentOptionIndex = Mathf.Min(customizationOptions.Length - 1, currentOptionIndex + 1);
            UpdateCustomizationSelection();

            scrollCategory.PlayFeedbacks();
        }
        else if (direction.x < -0.5f)
        {
            // Movimento para a esquerda (alterar a opção)
            switchOption.PlayFeedbacks();
            BlinkArrow("LeftArrow");

            // Caso esteja na categoria de SkinColor, altera o material
            if (currentOptionIndex == 1) // Corrigido para currentSkinColorIndex
            {
                ChangeSkinMaterial(false); // Passar para o próximo material
            }
        }
        else if (direction.x > 0.5f)
        {
            // Movimento para a direita (alterar a opção)
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

    // Função para alterar a cor da pele
    void ChangeSkinMaterial(bool next)
    {
        int skinMaterialCount = customizationLibrary.skinMaterials.Count;

        if (skinMaterialCount == 0)
        {
            // Se não houver materiais na lista, não faz nada
            return;
        }

        // Condicional para definir a navegação circular (ir para o próximo ou voltar para o anterior)
        if (next)
        {
            // Avança para o próximo material, retornando ao índice 0 quando chegar ao final
            currentSkinColorIndex = (currentSkinColorIndex + 1) % skinMaterialCount;
        }
        else
        {
            // Retrocede para o material anterior, retornando ao último índice quando chegar ao início
            currentSkinColorIndex = (currentSkinColorIndex - 1 + skinMaterialCount) % skinMaterialCount;
        }

        // Acessa o material da opção selecionada da lista skinMaterials
        Material skinMaterial = customizationLibrary.skinMaterials[currentSkinColorIndex].material;

        // Aplica o material nas partes do corpo usando as referências corretas
        headRenderer.material = skinMaterial;
        leftEarRenderer.material = skinMaterial;
        rightEarRenderer.material = skinMaterial;
    }
}