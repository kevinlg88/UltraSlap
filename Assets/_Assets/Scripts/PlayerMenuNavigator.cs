using UnityEngine;
using UnityEngine.InputSystem;
using MoreMountains.Feedbacks;

public class PlayerMenuNavigator : MonoBehaviour
{
    public GameObject[] customizationOptions; // Lista de GameObjects vazios representando partes customiz�veis
    public int currentOptionIndex = 0;

    public InputActionAsset inputActions; // Refer�ncia ao InputActionAsset configurado no Unity (Player1)

    private InputAction moveAction; // Para mover entre op��es

    [SerializeField] private MMFeedbacks scrollCategory;
    [SerializeField] private MMFeedbacks switchOption;

    [SerializeField] private float arrowBlinkDuration = 0.2f;

    // Refer�ncia para a CustomizationLibrary
    public CustomizationLibrary customizationLibrary; // A biblioteca de customiza��o do personagem
    public int currentSkinColorIndex = 0; // �ndice para a lista de skinMaterials

    // Refer�ncias para as partes do corpo do personagem
    public Renderer headRenderer;
    public Renderer leftEarRenderer;
    public Renderer rightEarRenderer;

    void Start()
    {
        // Busca a a��o de navega��o dentro do Action Map chamado "UI"
        moveAction = inputActions.FindActionMap("UI").FindAction("Navigation");

        moveAction.Enable();
        moveAction.performed += ctx => Navigate(ctx.ReadValue<Vector2>());

        UpdateCustomizationSelection(); // Inicializa visibilidade correta
    }

    void OnDisable()
    {
        moveAction.Disable();
    }

    void Navigate(Vector2 direction)
    {
        scrollCategory.PlayFeedbacks();

        // Detecta a movimenta��o para cima ou para baixo (op��es de navega��o de menu)
        if (direction.y > 0.5f)
        {
            // Movimento para cima: Descer o �ndice
            currentOptionIndex = Mathf.Max(0, currentOptionIndex - 1);
            UpdateCustomizationSelection();
        }
        else if (direction.y < -0.5f)
        {
            // Movimento para baixo: Subir o �ndice
            currentOptionIndex = Mathf.Min(customizationOptions.Length - 1, currentOptionIndex + 1);
            UpdateCustomizationSelection();
        }
        else if (direction.x < -0.5f)
        {
            // Movimento para a esquerda (alterar a op��o)
            switchOption.PlayFeedbacks();
            BlinkArrow("RightArrow");

            // Caso esteja na categoria de SkinColor, altera o material
            if (currentOptionIndex == 1) // Corrigido para currentSkinColorIndex
            {
                ChangeSkinMaterial(true); // Passar para o pr�ximo material
            }
        }
        else if (direction.x > 0.5f)
        {
            // Movimento para a direita (alterar a op��o)
            switchOption.PlayFeedbacks();
            BlinkArrow("LeftArrow");

            // Caso esteja na categoria de SkinColor, altera o material
            if (currentOptionIndex == 1) // Corrigido para currentSkinColorIndex
            {
                ChangeSkinMaterial(false); // Passar para o material anterior
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

        // Aplica o material nas partes do corpo usando as refer�ncias corretas
        headRenderer.material = skinMaterial;
        leftEarRenderer.material = skinMaterial;
        rightEarRenderer.material = skinMaterial;
    }
}