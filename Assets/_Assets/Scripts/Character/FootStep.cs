using UnityEngine;
using MoreMountains.Feedbacks;

public class FootStep : MonoBehaviour
{
    [SerializeField] private MMFeedbacks footStepSfx;

    private void OnTriggerEnter(Collider other)
    {
        //Debug.Log("trigger:" + other.name);

        if (other.gameObject.layer == LayerMask.NameToLayer("Floor"))
        {
            footStepSfx.PlayFeedbacks();
            UnityEngine.Debug.Log("pisou no chão");
        }

    }

}

