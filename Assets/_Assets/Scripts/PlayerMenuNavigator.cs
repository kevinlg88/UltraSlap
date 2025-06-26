using UnityEngine;
using UnityEngine.InputSystem;
using MoreMountains.Feedbacks;
using Rewired.Integration.UnityUI;
using Rewired;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class PlayerMenuNavigator : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private RewiredStandaloneInputModule inputModule;
    [SerializeField] private RewiredEventSystem rewiredEventSystem;
    [SerializeField] GameObject UIReady;

    [Header("Feedbacks")]
    [SerializeField] private MMF_Player feedback_SoundScrolling;
    [SerializeField] private MMF_Player feedback_SoundSwitchingArrows;

    private Player player;
    private Button leftButton;
    private Button rightButton;
    private int playerId = 0; // Default player ID, can
    private bool axisInUse = false;

    private GameObject _currentSelection;
    private GameObject CurrentSelection
    {
        get { return _currentSelection; }
        set
        {
            if (value != _currentSelection)
            {
                feedback_SoundScrolling.PlayFeedbacks(transform.position);
                _currentSelection = value;
            }
        }
    }
    private Button CurrentLeftButton
    {
        get { return leftButton; }
        set
        {
            if (value != leftButton)
            {
                if (leftButton) leftButton.image.color = new Color(0, 0, 0, 0);
                value.image.color = Color.white;
                leftButton = value;
            }
        }
    }

    private Button CurrentRightButton
    {
        get { return rightButton; }
        set
        {
            if (value != rightButton)
            {
                if (rightButton) rightButton.image.color = new Color(0, 0, 0, 0);
                value.image.color = Color.white;
                rightButton = value;
            }
        }
    }

    void Awake()
    {
        inputModule.RewiredInputManager = FindObjectOfType<InputManager>();
        player = ReInput.players.GetPlayer(playerId);
    }

    void Update()
    {
        CurrentSelection = rewiredEventSystem.currentSelectedGameObject;
        SelectHorizontalButtonField(CurrentSelection);
    }

    private void SelectHorizontalButtonField(GameObject currentSelection)
    {
        if (currentSelection != null)
        {
            CurrentLeftButton = currentSelection.transform.GetChild(0).GetComponent<Button>();
            CurrentRightButton = currentSelection.transform.GetChild(1).GetComponent<Button>();
            if (CurrentLeftButton != null && CurrentRightButton != null)
            {
                float horizontalInput = player.GetAxis("Move Horizontal");
                if (horizontalInput < -0.5f && !axisInUse)
                {
                    feedback_SoundSwitchingArrows.PlayFeedbacks(transform.position);
                    ExecuteEvents.Execute(CurrentLeftButton.gameObject,
                        new PointerEventData(rewiredEventSystem),
                        ExecuteEvents.submitHandler);

                    axisInUse = true;
                }

                else if (horizontalInput > 0.5f && !axisInUse)
                {
                    feedback_SoundSwitchingArrows.PlayFeedbacks(transform.position);
                    ExecuteEvents.Execute(CurrentRightButton.gameObject,
                        new PointerEventData(rewiredEventSystem),
                        ExecuteEvents.submitHandler);

                    axisInUse = true;
                }

                if (Mathf.Abs(horizontalInput) < 0.2f && axisInUse)
                {
                    axisInUse = false;
                }
            }
        }
    }

    public void SetPlayerId(int id)
    {
        playerId = id;
        player = ReInput.players.GetPlayer(playerId);
        inputModule.RewiredPlayerIds = new int[] { playerId };
    }

    public void SetReady(bool value)
    {
        UIReady.SetActive(value);
    }
    public void ButtonPressed(string msg)
    {
        Debug.Log("Button pressed: " + msg);
    }

}