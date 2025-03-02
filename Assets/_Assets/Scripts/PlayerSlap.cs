using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MoreMountains.Feedbacks;

public class PlayerSlap : MonoBehaviour
{
    [SerializeField] KeyCode keySlap;
    [SerializeField] private MMFeedbacks slapWhoosh;
    Animator animator;

    private void Start()
    {
        animator = GetComponent<Animator>();

    }
    void Update()
    {
        if (Input.GetKeyDown(keySlap)) 
        {
            animator.SetTrigger("triggerTapa");
            slapWhoosh.PlayFeedbacks();
        }
    }
}
