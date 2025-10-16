using UnityEngine;

public class OneWayPlatform : MonoBehaviour
{
    [Header("References")]
    [Tooltip("Objeto que será movido pela plataforma.")]
    public Transform target;

    [Tooltip("Limite menor no eixo X.")]
    public Transform lowLimit;

    [Tooltip("Limite maior no eixo X.")]
    public Transform highLimit;

    [Header("Settings")]
    [Tooltip("Velocidade de movimento no eixo X.")]
    public float moveSpeed = 2f;

    [Tooltip("Direção inicial do movimento (true = positivo, false = negativo).")]
    public bool startMovingPositive = true;

    private enum MoveState { Stopped, MovingPositive, MovingNegative }
    private MoveState currentState;

    private void Start()
    {
        currentState = startMovingPositive ? MoveState.MovingPositive : MoveState.MovingNegative;
    }

    private void Update()
    {
        if (target == null || lowLimit == null || highLimit == null)
            return;

        switch (currentState)
        {
            case MoveState.MovingPositive:
                MovePositive();
                break;

            case MoveState.MovingNegative:
                MoveNegative();
                break;

            case MoveState.Stopped:
                // Nenhum movimento
                break;
        }
    }

    private void MovePositive()
    {
        target.position += Vector3.right * moveSpeed * Time.deltaTime;

        if (target.position.x >= highLimit.position.x)
        {
            // Teleporta para o limite A (lado esquerdo)
            Vector3 pos = target.position;
            pos.x = lowLimit.position.x;
            target.position = pos;
        }
    }

    private void MoveNegative()
    {
        target.position += Vector3.left * moveSpeed * Time.deltaTime;

        if (target.position.x <= lowLimit.position.x)
        {
            // Teleporta para o limite B (lado direito)
            Vector3 pos = target.position;
            pos.x = highLimit.position.x;
            target.position = pos;
        }
    }

    // --- Funções públicas de controle ---
    public void MovePositiveX() => currentState = MoveState.MovingPositive;
    public void MoveNegativeX() => currentState = MoveState.MovingNegative;
    public void Stop() => currentState = MoveState.Stopped;
}