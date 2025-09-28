using UnityEngine;
using TMPro;
using UnityEngine.EventSystems;

public class HighlightTextOnSelect : MonoBehaviour, ISelectHandler, IDeselectHandler
{
    [SerializeField] private TMP_Text targetText;
    [SerializeField] private Color highlightedColor = Color.yellow;
    private Color originalColor;

    void Start()
    {
        if (targetText != null)
            originalColor = targetText.color;
    }

    public void OnSelect(BaseEventData eventData)
    {
        if (targetText != null)
            targetText.color = highlightedColor;
    }

    public void OnDeselect(BaseEventData eventData)
    {
        if (targetText != null)
            targetText.color = originalColor;
    }
}