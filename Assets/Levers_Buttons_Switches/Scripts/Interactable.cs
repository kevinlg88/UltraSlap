using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Interactable : MonoBehaviour
{
    [SerializeField]
    Animator targetAnimator;

    [SerializeField]
    string animationTriggerName;

    public Animator GetAnimator()
    {
        return targetAnimator;
    }

    public string GetAnimationTriggerName()
    {
        return animationTriggerName;
    }
}
