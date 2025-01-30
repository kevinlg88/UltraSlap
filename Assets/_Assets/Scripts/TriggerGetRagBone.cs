using FIMSpace.FProceduralAnimation;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerGetRagBone : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("trigger");
        if(other.TryGetComponent(out RagdollAnimator2BoneIndicator boneIndicator))
        {
            Debug.Log("bone name: " + boneIndicator.name + "\n" +
                "parent handler: " + boneIndicator.ParentHandler + "\n" +
                "parent chain: " + boneIndicator.ParentChain.ChainName);
        }
    }
}
