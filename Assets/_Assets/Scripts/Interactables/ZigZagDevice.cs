using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MoreMountains.Feedbacks;

public class ZigZagDevice : MonoBehaviour
{
    [Header("State")]
    [SerializeField] private bool isActive = false;       // indica se o device est� ativo ou n�o
    [SerializeField] private bool isAtPositionB = false;  // indica se o device est� na posi��o A (false) ou B (true)

    [Header("MMFeedbacks")]
    [SerializeField] private MMFeedbacks moveToPositionAFeedbacks; // Feedbacks para mover para A
    [SerializeField] private MMFeedbacks moveToPositionBFeedbacks; // Feedbacks para mover para B

    // Fun��o para ativar o device
    public void Activate()
    {
        // Se j� est� ativo, n�o faz nada
        if (isActive) return;

        // Marca como ativo
        isActive = true;

        // Se est� na posi��o A, manda ir para B
        if (!isAtPositionB)
        {
            MoveToPositionB();
        }
        else // Se est� na posi��o B, manda ir para A
        {
            MoveToPositionA();
        }
    }

    // Fun��o para mover at� a posi��o A
    public void MoveToPositionA()
    {
        if (moveToPositionAFeedbacks != null)
        {
            moveToPositionAFeedbacks.PlayFeedbacks();
        }

        // Atualiza estado
        isAtPositionB = false;
    }

    // Chama apenas o movimento para A se estiver no ponto B e n�o estiver ativo
    public void CallToPositionA()
    {
        // S� funciona se o device n�o estiver ativo e estiver em B
        if (!isActive && isAtPositionB)
        {
            Activate(); // usa a l�gica existente do Activate()
        }
    }

    // Chama apenas o movimento para B se estiver no ponto A e n�o estiver ativo
    public void CallToPositionB()
    {
        // S� funciona se o device n�o estiver ativo e estiver em B
        if (!isActive && !isAtPositionB)
        {
            Activate(); // usa a l�gica existente do Activate()
        }
    }

    // Fun��o para mover at� a posi��o B
    public void MoveToPositionB()
    {
        if (moveToPositionBFeedbacks != null)
        {
            moveToPositionBFeedbacks.PlayFeedbacks();
        }

        // Atualiza estado
        isAtPositionB = true;
    }

    public void Deactivate()
    {
        isActive = false;
    }
}