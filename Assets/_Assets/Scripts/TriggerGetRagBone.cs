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
            RagdollAnimator2 ragdoll = GetRagdoll(rigidbody.gameObject);
            ragdoll.User_SwitchFallState();
            //ragdoll.RA2Event_AddHeadImpact(this.gameObject.transform.forward * slapPower);
            //ragdoll.
            //ragdoll.GetComponent<Rigidbody>().AddForce(this.gameObject.transform.forward * slapPower, ForceMode.Impulse);
            //ragdoll.User_AddAllBonesImpact(this.gameObject.transform.forward * slapPower, 0.0f, ForceMode.Impulse);
            //ragdoll.User_AddRigidbodyImpact(rigidbody,this.gameObject.transform.forward * slapPower, 0.0f, ForceMode.Impulse);
            //ragdoll.RA2Event_AddHeadImpact(this.gameObject.transform.forward * slapPower,0.0f, ForceMode.Impulse);
            rigidbody.AddForce(this.gameObject.transform.forward * slapPower, ForceMode.Impulse);
            GameObject go = Instantiate(effectPrefab,this.transform.position, Quaternion.identity);
            Destroy(go,1f);
            Debug.Log("aplicou for�a");
        }
    }

    private RagdollAnimator2 GetRagdoll(GameObject go)
    {
        GameObject parent = go.transform.parent.gameObject;
        if(!parent) return null;
        return parent.GetComponent<RagdollAnimatorDummyReference>().ParentComponent as RagdollAnimator2;
    }

    private void OnDisable()
    {
        isSlapping = false;
    }
}
