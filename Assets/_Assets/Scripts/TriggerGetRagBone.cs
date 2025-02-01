using FIMSpace.FProceduralAnimation;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerGetRagBone : MonoBehaviour
{
    [SerializeField] float slapPower;
    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("trigger:" + other.name);
        
        //#### Detect Components ####

        //Component[] components = other.GetComponents<Component>();
        //foreach (Component component in components)
        //{
        //    Debug.Log($"Componente: {component.GetType().Name}");
        //}

        if(other.TryGetComponent(out Rigidbody rigidbody))
        {
            rigidbody.AddForce(this.gameObject.transform.forward * slapPower, ForceMode.Impulse);
            Debug.Log("aplicou força");
        }
    }
}
