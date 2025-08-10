using UnityEngine;
using MoreMountains.Feedbacks;

public class FootStep : MonoBehaviour
{
    [SerializeField] private MMFeedbacks leftFootStepSfx, rightFootStepSfx;
    public RigidbodyController controller;

    public void LeftStep()
    {
        if (controller.isGrounded)
        {
            leftFootStepSfx.PlayFeedbacks();
        }
    }

    public void RightStep()
    {
        if (controller.isGrounded)
        {
            rightFootStepSfx.PlayFeedbacks();
        }
    }
}

