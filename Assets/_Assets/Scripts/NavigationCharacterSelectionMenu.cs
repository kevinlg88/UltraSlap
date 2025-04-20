using System.Diagnostics;
using UnityEngine;
using UnityEngine.InputSystem; // Importando o sistema de entrada do Unity

public class NavigationCharacterSelectionMenu : MonoBehaviour
{
    public GameObject[] menuItems;  // Lista de objetos de menu
    public int currentSelectionIndex = 0;

    // Referência ao InputAction do Player1 configurado no Unity Input System
    public InputActionAsset inputActions; // A referência ao InputActionAsset configurado no Unity (já configurado para o Player1)

    private InputAction moveActionPlayer1;  // Ação de movimento do Player1
    private InputAction selectActionPlayer1;  // Ação de seleção do Player1

    // Start é chamado antes do primeiro frame de atualização
    void Start()
    {
        // Obtendo as ações de Player1 a partir do InputActionAsset configurado
        moveActionPlayer1 = inputActions.FindActionMap("Player1").FindAction("Move");
        selectActionPlayer1 = inputActions.FindActionMap("Player1").FindAction("Select");

        // Certificando que as ações estão habilitadas
        moveActionPlayer1.Enable();
        selectActionPlayer1.Enable();

        // Vinculando as ações
        moveActionPlayer1.performed += ctx => MoveMenu(ctx.ReadValue<Vector2>());
        selectActionPlayer1.performed += ctx => SelectMenu();
    }

    void OnDisable()
    {
        // Desabilitar as ações ao sair da cena
        moveActionPlayer1.Disable();
        selectActionPlayer1.Disable();
    }

    // Função para mover a seleção do menu com base no input do Player1
    void MoveMenu(Vector2 direction)
    {
        if (direction.y > 0) // Para cima
        {
            currentSelectionIndex = Mathf.Max(0, currentSelectionIndex - 1);
        }
        else if (direction.y < 0) // Para baixo
        {
            currentSelectionIndex = Mathf.Min(menuItems.Length - 1, currentSelectionIndex + 1);
        }

        UpdateMenuSelection();
    }

    // Função para realizar a seleção do menu (quando pressionar o botão de seleção)
    void SelectMenu()
    {
        UnityEngine.Debug.Log("Selecionado: " + menuItems[currentSelectionIndex].name);
        // Adicione a lógica de seleção aqui, como carregar uma nova cena ou executar uma ação.
    }

    // Função para atualizar a seleção do menu
    void UpdateMenuSelection()
    {
        // Desabilitar todos os itens do menu
        foreach (var item in menuItems)
        {
            item.SetActive(false);
        }

        // Ativar o item selecionado
        menuItems[currentSelectionIndex].SetActive(true);
    }
}