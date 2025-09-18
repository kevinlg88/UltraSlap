using UnityEngine;
using UnityEngine.EventSystems;

public class KeepSelection : MonoBehaviour
{
    private GameObject lastSelected;

    void Update()
    {
        // Se n�o tiver nada selecionado, restaura o �ltimo selecionado
        if (EventSystem.current.currentSelectedGameObject == null && lastSelected != null)
        {
            EventSystem.current.SetSelectedGameObject(lastSelected);
        }
        else
        {
            // Atualiza o �ltimo selecionado se houver algum
            lastSelected = EventSystem.current.currentSelectedGameObject;
        }
    }
}