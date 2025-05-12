using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CustomParentConstraints : MonoBehaviour
{
    [SerializeField] Transform target;

    void LateUpdate()
    {
        if (target != null)
        {
            transform.position = target.position;
            transform.rotation = target.rotation;
        }
    }
}
