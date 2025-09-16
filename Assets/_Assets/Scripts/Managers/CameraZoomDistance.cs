using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class CameraZoom : MonoBehaviour
{
    [SerializeField] private Transform cameraTransform;

    [SerializeField] private List<PlayerController> playersOnCamera = new List<PlayerController>();
    private GameEvent _gameEvent;

    [Header("Zoom Control")]
    [SerializeField] private float minZoom = 5f;
    [SerializeField] private float maxZoom = 20f;
    [SerializeField] private float zoomFactor = 1.2f;
    [SerializeField] private float zoomSpeed = 5f;
    [SerializeField] private Vector3 offset = new(0, 5, -10);

    [Header("Dynamic Limit Factors")]
    [SerializeField] private float yLimitFactor = 0.5f;   // quanto maior o zoom, mais sobe o mínimo Y
    [SerializeField] private float xLimitFactor = 0.8f;   // quanto maior o zoom, mais restringe X

    [Inject]
    public void Construct(GameEvent gameEvent)
    {
        Debug.Log($"Instalou game event na camera");
        _gameEvent = gameEvent;
        _gameEvent.onPlayersJoined.AddListener(OnPlayersJoined);
        _gameEvent.onPlayerDeath.AddListener(OnPlayerDeath);

    }

    void Update()
    {
        if (playersOnCamera.Count == 0) return;

        Vector3 targetPosition;
        float targetZoom;

        if (playersOnCamera.Count == 1)
        {
            var playerPos = playersOnCamera[0].transform.position;
            targetZoom = minZoom;
            targetPosition = playerPos + offset.normalized * targetZoom;
        }
        else
        {
            Bounds bounds = new Bounds(playersOnCamera[0].transform.position, Vector3.zero);
            foreach (var player in playersOnCamera)
                bounds.Encapsulate(player.transform.position);

            Vector3 midPoint = bounds.center;
            float greatestSize = Mathf.Max(bounds.size.x, bounds.size.y, bounds.size.z);

            targetZoom = Mathf.Clamp(greatestSize * zoomFactor, minZoom, maxZoom);

            targetPosition = midPoint + offset.normalized * targetZoom;
        }

        // Calcula limites dinâmicos com base no zoom
        float dynamicMinY = targetZoom * yLimitFactor;
        float dynamicMinX = -targetZoom * xLimitFactor;
        float dynamicMaxX = targetZoom * xLimitFactor;

        // Aplica os limites dinâmicos
        targetPosition.y = Mathf.Max(targetPosition.y, dynamicMinY);
        targetPosition.x = Mathf.Clamp(targetPosition.x, dynamicMinX, dynamicMaxX);

        cameraTransform.position = Vector3.Lerp(
            cameraTransform.position,
            targetPosition,
            Time.deltaTime * zoomSpeed
        );
    }

    private void OnPlayerDeath() => playersOnCamera.RemoveAll(p => p.IsDead);
    private void OnPlayersJoined(List<PlayerController> players) => playersOnCamera = new List<PlayerController>(players);
}
