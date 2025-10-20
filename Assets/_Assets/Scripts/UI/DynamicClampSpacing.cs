using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(HorizontalLayoutGroup))]
public class DynamicClampSpacing : MonoBehaviour
{
    public float minSpacing = -1000f; // Espaçamento mínimo (poucos elementos)
    public float maxSpacing = 10f;    // Espaçamento máximo (muitos elementos)
    public bool useParentWidth = true;

    private HorizontalLayoutGroup layoutGroup;
    private RectTransform rectTransform;

    void Awake()
    {
        layoutGroup = GetComponent<HorizontalLayoutGroup>();
        rectTransform = GetComponent<RectTransform>();
        UpdateSpacing();
    }

    void OnTransformChildrenChanged()
    {
        UpdateSpacing();
    }

    void OnEnable()
    {
        UpdateSpacing(); 
    }

    void UpdateSpacing()
    {
        int activeChildren = 0;
        foreach (Transform child in transform)
        {
            if (child.gameObject.activeSelf)
                activeChildren++;
        }

        if (activeChildren <= 1)
        {
            layoutGroup.spacing = minSpacing;
            return;
        }

        // Lógica invertida: quanto mais filhos, mais perto do maxSpacing
        float t = Mathf.InverseLerp(2, 8, activeChildren); // 2 = poucos, 8 = muitos (ajuste se quiser)
        float spacing = Mathf.Lerp(minSpacing, maxSpacing, t);

        layoutGroup.spacing = spacing;
    }
}