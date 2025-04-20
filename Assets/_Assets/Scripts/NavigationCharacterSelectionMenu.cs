using System.Diagnostics;
using UnityEngine;
using UnityEngine.InputSystem; // Importando o sistema de entrada do Unity

public class NavigationCharacterSelectionMenu : MonoBehaviour
{
    public GameObject[] menuItems;  // Lista de objetos de menu
    public int currentSelectionIndex = 0;

    // Refer�ncia ao InputAction do Player1 configurado no Unity Input System
    public InputActionAsset inputActions; // A refer�ncia ao InputActionAsset configurado no Unity (j� configurado para o Player1)

    private InputAction moveActionPlayer1;  // A��o de movimento do Player1
    private InputAction selectActionPlayer1;  // A��o de sele��o do Player1

    // Start � chamado antes do primeiro frame de atualiza��o
    void Start()
    {
        // Obtendo as a��es de Player1 a partir do InputActionAsset configurado
        moveActionPlayer1 = inputActions.FindActionMap("Player1").FindAction("Move");
        selectActionPlayer1 = inputActions.FindActionMap("Player1").FindAction("Select");

        // Certificando que as a��es est�o habilitadas
        moveActionPlayer1.Enable();
        selectActionPlayer1.Enable();

        // Vinculando as a��es
        moveActionPlayer1.performed += ctx => MoveMenu(ctx.ReadValue<Vector2>());
        selectActionPlayer1.performed += ctx => SelectMenu();
    }

    void OnDisable()
    {
        // Desabilitar as a��es ao sair da cena
        moveActionPlayer1.Disable();
        selectActionPlayer1.Disable();
    }

    // Fun��o para mover a sele��o do menu com base no input do Player1
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

    // Fun��o para realizar a sele��o do menu (quando pressionar o bot�o de sele��o)
    void SelectMenu()
    {
        UnityEngine.Debug.Log("Selecionado: " + menuItems[currentSelectionIndex].name);
        // Adicione a l�gica de sele��o aqui, como carregar uma nova cena ou executar uma a��o.
    }

    // Fun��o para atualizar a sele��o do menu
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