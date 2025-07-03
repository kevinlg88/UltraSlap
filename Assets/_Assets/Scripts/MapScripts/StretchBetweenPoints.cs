using UnityEngine;

[ExecuteAlways]
public class StretchBetweenPoints : MonoBehaviour
{
    public Transform pointA;
    public Transform pointB;

    public float lengthOffset = 0f;

    void Update()
    {
        if (pointA == null || pointB == null)
        {
            Debug.LogWarning("StretchBetweenPoints: Points not assigned.");
            return;
        }

        Vector3 direction = pointB.position - pointA.position;
        float distance = direction.magnitude;

        transform.position = pointA.position + direction * 0.5f;

        transform.rotation = Quaternion.LookRotation(direction);

        transform.localScale = new Vector3(
            transform.localScale.x,
            transform.localScale.y,
            distance + lengthOffset
        );
    }
}