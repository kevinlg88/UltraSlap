using UnityEngine;

public class Vibration : MonoBehaviour
{
    [Header("Shake Settings")]
    public float duration = 1f;        
    public float magnitude = 0.1f;       
    public float frequency = 25f;         

    private Vector3 originalPosition;
    private float elapsed = 0f;
    private bool isShaking = false;

    void Start()
    {
        originalPosition = transform.localPosition;
    }

    void Update()
    {
        if (isShaking)
        {
            if (elapsed < duration)
            {
                elapsed += Time.deltaTime;

                float x = Mathf.PerlinNoise(Time.time * frequency, 0f) * 2f - 1f;
                float y = Mathf.PerlinNoise(0f, Time.time * frequency) * 2f - 1f;
                transform.localPosition = originalPosition + new Vector3(x, y, 0f) * magnitude;
            }
            else
            {
                StopShake();
            }
        }
    }

    public void StartShake()
    {
        if (!isShaking)
        {
            originalPosition = transform.localPosition;
            elapsed = 0f;
            isShaking = true;
        }
    }

    public void StopShake()
    {
        isShaking = false;
        transform.localPosition = originalPosition;
    }
}