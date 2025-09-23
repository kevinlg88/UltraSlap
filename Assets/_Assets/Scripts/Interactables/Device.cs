using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MoreMountains.Feedbacks;

public class Device : MonoBehaviour
{
    [Header("State")]
    [SerializeField] private bool isActive = false;       // indica se o device está ativo ou não
    [SerializeField] private bool isAtPositionB = false;  // indica se o device está na posição A (false) ou B (true)

    [Header("MMFeedbacks")]
    [SerializeField] private MMFeedbacks moveToPositionAFeedbacks; // Feedbacks para mover para A
    [SerializeField] private MMFeedbacks moveToPositionBFeedbacks; // Feedbacks para mover para B

    // Função para ativar o device
    public void Activate()
    {
        // Se já está ativo, não faz nada
        if (isActive) return;

        // Marca como ativo
        isActive = true;

        // Se está na posição A, manda ir para B
        if (!isAtPositionB)
        {
            MoveToPositionB();
        }
        else // Se está na posição B, manda ir para A
        {
            MoveToPositionA();
        }

    }

    // Função para mover até a posição A
    public void MoveToPositionA()
    {
        if (moveToPositionAFeedbacks != null)
        {
            moveToPositionAFeedbacks.PlayFeedbacks();
        }

        // Atualiza estado
        isAtPositionB = false;
    }

    // Função para mover até a posição B
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