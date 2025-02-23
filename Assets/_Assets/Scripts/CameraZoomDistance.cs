using UnityEngine;

public class CameraZoom : MonoBehaviour
{
    public Transform character1; // Primeiro personagem
    public Transform character2; // Segundo personagem
    public Transform cameraTransform; // Transform da câmera

    public float minZoom = 5f; // Distância mínima da câmera
    public float maxZoom = 20f; // Distância máxima da câmera
    public float zoomFactor = 1.2f; // Influencia o ritmo de afastamento/aproximação da câmera quando os personagens se distanciam ou se aproximam
    public float zoomSpeed = 5f; // Velocidade de ajuste do zoom
    public Vector3 offset = new Vector3(0, 5, -10); // Offset da câmera
    public float heightThreshold = -5f; // Altura mínima para considerar um personagem ativo

    void Update()
    {
        if (character1 == null || character2 == null || cameraTransform == null)
            return;

        bool isCharacter1AboveThreshold = character1.position.y >= heightThreshold;
        bool isCharacter2AboveThreshold = character2.position.y >= heightThreshold;

        Vector3 targetPosition = cameraTransform.position; // Mantém a posição atual se ambos estiverem abaixo do limite
        float targetZoom = (maxZoom + minZoom) / 2; // Zoom médio padrão se nenhum personagem estiver acima do limite

        if (isCharacter1AboveThreshold && isCharacter2AboveThreshold)
        {
            // Ambos estão acima do limite, calcular normalmente
            float distance = Vector3.Distance(character1.position, character2.position);
            targetZoom = Mathf.Clamp(distance*zoomFactor, minZoom, maxZoom);
            Vector3 midPoint = (character1.position + character2.position) / 2;
            targetPosition = midPoint + offset.normalized * targetZoom;
        }
        else if (isCharacter1AboveThreshold)
        {
            // Apenas o personagem 1 está acima do limite, focar nele
            targetPosition = character1.position + offset.normalized * minZoom;
        }
        else if (isCharacter2AboveThreshold)
        {
            // Apenas o personagem 2 está acima do limite, focar nele
            targetPosition = character2.position + offset.normalized * minZoom;
        }

        // Aplicar interpolação para suavizar o movimento da câmera
        cameraTransform.position = Vector3.Lerp(cameraTransform.position, targetPosition, Time.deltaTime * zoomSpeed);
    }
}
