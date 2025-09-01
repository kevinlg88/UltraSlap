using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using MaskTransitions;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

public class UIManager : MonoBehaviour
{
    [Header("UI Score")]
    [SerializeField] private GameObject uiScoreMain;
    [SerializeField] private GameObject teams;
    [SerializeField] private GameObject teamPrefab;
    [SerializeField] private Button scoreContinueBttn;
    [Header("UI RoundTransition")]
    [SerializeField] private GameObject uiRoundTransitionMain;
    [SerializeField] private GameObject maskHandTransparent;
    [SerializeField] private GameObject handBlackScreen;

    [Header("UI RoundTransition Setup")]
    [SerializeField] private float delayStartTransition = 1f;
    [SerializeField] private Vector3 maskHandFinalScale;
    [SerializeField] private float animDuration;
    [SerializeField] Ease easeType;

    [Inject]
    public void Construct(PlayerManager playerManager,GameEvent gameEvent)
    {

    }


#region Slap Transition Anim
    private void StartMatchTransitionAnim()
    {
        uiRoundTransitionMain.SetActive(true);
        Invoke("HandBlackScreenTransitionAnim", delayStartTransition);
    }
    private void MaskScreenTransitionAnim()
    {
        maskHandTransparent.SetActive(true);
        maskHandTransparent.transform.DOScale(maskHandFinalScale, animDuration)
            .SetEase(easeType)
            .OnComplete(() =>
            {
                maskHandTransparent.transform.localScale = Vector3.zero;
                handBlackScreen.transform.localScale = Vector3.zero;

                maskHandTransparent.SetActive(false);
                uiRoundTransitionMain.SetActive(false);
            });
    }

    private void HandBlackScreenTransitionAnim()
    {
        handBlackScreen.transform.DOScale(maskHandFinalScale, animDuration)
        .SetEase(easeType)
        .OnComplete(() =>
        {
            MaskScreenTransitionAnim();
        });
    }
}
#endregion