using UnityEngine;
using UnityEngine.UI;
using System.Collections;

[RequireComponent(typeof(RectTransform))]
public class UIImageReveal : MonoBehaviour
{
    [Header("RectTransform do container que atua como máscara (deve ter RectMask2D)")]
    public RectTransform maskRect; // geralmente é o mesmo objeto que tem este script
    [Header("A imagem que será revelada (filha direta do maskRect)")]
    public RectTransform targetImage;
    [Header("Velocidade em pixels por segundo")]
    public float speed = 800f;

    float maxWidth;
    bool initialized = false;

    void Awake()
    {
        if (maskRect == null) maskRect = (RectTransform)transform;
    }

    void Start()
    {
        // garante que o maskRect está configurado para pivot/anchor corretos (crescer da esquerda p/ dir)
        maskRect.pivot = new Vector2(0f, 0.5f);
        // fixa anchors X para a esquerda (anchorMin.x = anchorMax.x = 0)
        maskRect.anchorMin = new Vector2(0f, maskRect.anchorMin.y);
        maskRect.anchorMax = new Vector2(0f, maskRect.anchorMax.y);

        // Começa com largura zero
        Vector2 sd = maskRect.sizeDelta;
        sd.x = 0f;
        maskRect.sizeDelta = sd;

        // espera um frame para o layout rodar corretamente e pega a largura alvo
        StartCoroutine(InitNextFrame());
    }

    IEnumerator InitNextFrame()
    {
        yield return null; // espera 1 frame (layout já aplicado)
        if (targetImage == null)
        {
            Debug.LogWarning("UIImageReveal: targetImage não atribuído. Coloque a imagem que será revelada.");
            yield break;
        }

        // A largura alvo é a largura do targetImage em pixels neste layout atual
        maxWidth = targetImage.rect.width;
        initialized = true;
        // opcional debug
        Debug.Log($"UIImageReveal inicializado: maxWidth = {maxWidth}");
    }

    void Update()
    {
        if (!initialized) return;

        float curWidth = maskRect.sizeDelta.x;
        if (curWidth >= maxWidth) return;

        float newWidth = curWidth + speed * Time.unscaledDeltaTime;
        newWidth = Mathf.Min(newWidth, maxWidth);

        Vector2 sd = maskRect.sizeDelta;
        sd.x = newWidth;
        maskRect.sizeDelta = sd;
    }
}