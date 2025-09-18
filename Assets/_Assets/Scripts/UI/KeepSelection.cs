using UnityEngine;
using UnityEngine.EventSystems;

public class KeepSelection : MonoBehaviour
{
    private GameObject lastSelected;

    void Update()
    {
        // Se não tiver nada selecionado, restaura o último selecionado
        if (EventSystem.current.currentSelectedGameObject == null && lastSelected != null)
        {
            EventSystem.current.SetSelectedGameObject(lastSelected);
        }
        else
        {
            // Atualiza o último selecionado se houver algum
            lastSelected = EventSystem.current.currentSelectedGameObject;
        }
    }
}