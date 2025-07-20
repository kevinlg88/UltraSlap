using UnityEngine;
using MoreMountains.Feedbacks;

public class FootStep : MonoBehaviour
{
    [SerializeField] private MMFeedbacks leftFootStepSfx, rightFootStepSfx;

    public void LeftStep()
    {
        leftFootStepSfx.PlayFeedbacks();
    }

    public void RightStep()
    {
        rightFootStepSfx.PlayFeedbacks();
    }
}

