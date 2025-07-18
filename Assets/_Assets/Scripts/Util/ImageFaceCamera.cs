using UnityEngine;

public class ImageFaceCamera : MonoBehaviour
{
    public float offsetTowardsCamera;

    void Start()
    {
        if (Camera.main == null) return;

        RectTransform rectTransform = GetComponent<RectTransform>();
        if (rectTransform == null) return;

        Vector3 directionAway = rectTransform.position - Camera.main.transform.position;
        rectTransform.rotation = Quaternion.LookRotation(directionAway);

        Vector3 toCamera = (Camera.main.transform.position - rectTransform.position).normalized;
        rectTransform.position += toCamera * offsetTowardsCamera;
    }
}