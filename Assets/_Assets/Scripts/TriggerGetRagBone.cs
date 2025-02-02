using FIMSpace.FProceduralAnimation;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerGetRagBone : MonoBehaviour
{
    [SerializeField] GameObject effectPrefab;
    [SerializeField] float slapPower;
    bool isSlapping = false;
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
            if (isSlapping) return;
            isSlapping = true;
            rigidbody.AddForce(this.gameObject.transform.forward * slapPower, ForceMode.Impulse);
            GameObject go = Instantiate(effectPrefab,this.transform.position, Quaternion.identity);
            Destroy(go,1f);
            Debug.Log("aplicou força");
        }
    }

    private void OnDisable()
    {
        isSlapping = false;
    }
}
