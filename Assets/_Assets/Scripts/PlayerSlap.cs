using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSlap : MonoBehaviour
{
    [SerializeField] KeyCode keySlap;
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
        }
    }
}
