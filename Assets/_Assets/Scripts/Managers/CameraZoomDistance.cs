using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Zenject;

public class CameraZoom : MonoBehaviour
{
    public static CameraZoom Instance;

    public Transform[] characters;
    public Transform cameraTransform;

    public float minZoom = 5f;
    public float maxZoom = 20f;
    public float zoomFactor = 1.2f;
    public float zoomSpeed = 5f;
    public Vector3 offset = new Vector3(0, 5, -10);
    public float heightThreshold = -4f;

    [SerializeField]private List<PlayerController> playersOnCamera = new List<PlayerController>();
    private GameEvent _gameEvent;

    [Inject]
    public void Construct(GameEvent gameEvent)
    {
        Debug.Log($"Instalou game event na camera");
        _gameEvent = gameEvent;
        _gameEvent.onPlayerDeath.AddListener(OnPlayerDeath);

    }

    private void Start()
    {
        FindPlayersInScene();
    }

    public void FindPlayersInScene()
    {
        playersOnCamera = FindObjectsOfType<PlayerController>()
            .Where(p => !p.IsDead)
            .ToList();
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

        cameraTransform.position = Vector3.Lerp(
            cameraTransform.position,
            targetPosition,
            Time.deltaTime * zoomSpeed
        );
    }


    private void OnPlayerDeath()
    {
        playersOnCamera.RemoveAll(p => p.IsDead);
        FindPlayersInScene();
    }
}
