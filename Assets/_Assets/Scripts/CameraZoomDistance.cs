using UnityEngine;

public class CameraZoom : MonoBehaviour
{
    public Transform character1; // Primeiro personagem
    public Transform character2; // Segundo personagem
    public Transform cameraTransform; // Transform da c�mera

    public float minZoom = 5f; // Dist�ncia m�nima da c�mera
    public float maxZoom = 20f; // Dist�ncia m�xima da c�mera
    public float zoomSpeed = 5f; // Velocidade de ajuste do zoom
    public Vector3 offset = new Vector3(0, 5, -10); // Offset da c�mera

    void Update()
    {
        if (character1 == null || character2 == null || cameraTransform == null)
            return;

        float distance = Vector3.Distance(character1.position, character2.position);
        float targetZoom = Mathf.Clamp(distance, minZoom, maxZoom);

        Vector3 midPoint = (character1.position + character2.position) / 2;
        Vector3 targetPosition = midPoint + offset.normalized * targetZoom;

        cameraTransform.position = Vector3.Lerp(cameraTransform.position, targetPosition, Time.deltaTime * zoomSpeed);
    }
}