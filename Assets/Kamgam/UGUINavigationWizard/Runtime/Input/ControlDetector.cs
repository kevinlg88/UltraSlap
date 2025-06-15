using System;
using System.Collections.Generic;
using System.Linq;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;
using UnityEngine.EventSystems;
#if ENABLE_INPUT_SYSTEM && KAMGAM_UGUINAVIGATION_INPUT_SYSTEM
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;
#endif

namespace Kamgam.UGUINavigationWizard
{
    public enum ControlHardware { Touch = 0, Keyboard = 1, Controller = 2 }
    public enum ControlMode { Touch = 0, Pointer = 1, Key = 2 }

    /// <summary>
    /// Detects what hardware is connected and stores the result it "Hardware".
    /// Controllers takes precedence over Touch or Keyboard. Touch takes precedence over Keyboard.
    /// <br /><br />
    /// Also abstracts the used method of control into three modes and stores it in "Mode". This
    /// information can be used in cases of ambiguity (Mouse or Keyboard?, Touch or Controller?).
    /// </summary>
    public class ControlDetector : MonoBehaviour
    {
        public static bool LogEnabled = false;

        #region MonoBehaviourSingleton
        private static bool s_Destroyed = false;
        private static ControlDetector s_Instance = null;
        public static ControlDetector Instance
        {
            get
            {
                if (s_Instance == null && s_Destroyed == false)
                {
#if UNITY_2023_1_OR_NEWER
                    s_Instance = GameObject.FindFirstObjectByType<ControlDetector>(FindObjectsInactive.Exclude);
#else
                    s_Instance = GameObject.FindObjectOfType<ControlDetector>();
#endif
                    if (s_Instance == null)
                    {
                        s_Instance = (new GameObject("[Temp] UGUINavigationWizard_ControlDetector")).AddComponent<ControlDetector>();
                        s_Instance.gameObject.hideFlags = HideFlags.DontSave | HideFlags.NotEditable;
#if UNITY_EDITOR
                        UnityEditor.EditorApplication.playModeStateChanged -= onEditorPlayModeChanged;
                        UnityEditor.EditorApplication.playModeStateChanged += onEditorPlayModeChanged;
#endif
                    }
                    s_Instance.lastMousePosition = Input.mousePosition;
                    s_Instance.Detect();
                }
                return s_Instance;
            }
        }

        // Support disabled domain reload
#if UNITY_EDITOR
        private static void onEditorPlayModeChanged(PlayModeStateChange change)
        {
            if (EditorSettings.enterPlayModeOptions.HasFlag(EnterPlayModeOptions.DisableDomainReload))
            {
                if (change == PlayModeStateChange.ExitingPlayMode)
                {
                    if (s_Instance != null)
                        Destroy(s_Instance.gameObject);
                }

                if (change == PlayModeStateChange.EnteredPlayMode)
                {
                    s_Destroyed = false;
                }
            }
        }
#endif

        void Awake()
        {
            DontDestroyOnLoad(transform.gameObject);
        }

        public void OnDestroy()
        {
            s_Destroyed = true;
            s_Instance = null;
        }
        #endregion

        public delegate void HardwareChangedEventHandler(List<ControlHardware> hardware, ControlHardware? added, ControlHardware? removed);
        public event HardwareChangedEventHandler OnHardwareChanged;

        public delegate void ModeChangedEventHandler(ControlMode from, ControlMode to);
        public event ModeChangedEventHandler OnModeChanged;

        /// <summary>
        /// What's the primary input hardware (touch first).
        /// </summary>
        [System.NonSerialized]
        public List<ControlHardware> Hardware = new List<ControlHardware>() {};

        /// <summary>
        /// The current input mode.
        /// </summary>
        [System.NonSerialized]
#if UNITY_ANDROID || UNITY_IOS
        public ControlMode Mode = ControlMode.Touch;
#else
        public ControlMode Mode = ControlMode.Key;
#endif

        [System.NonSerialized]
        public float DetectionIntervalInSec = 0.333f;
        
#if !ENABLE_INPUT_SYSTEM && !KAMGAM_UGUINAVIGATION_INPUT_SYSTEM
        /// <summary>
        /// A list of key codes which are also commonly used on touch devices.
        /// </summary>
        [System.NonSerialized]
        public static KeyCode[] TouchKeyCodes = new KeyCode[] { KeyCode.Escape, KeyCode.Space, KeyCode.Return };

        /// <summary>
        /// List of controller key codes (used to detect if a controller key has been pressed).
        /// </summary>
        [System.NonSerialized]
        public static KeyCode[] ControllerKeyCodes = new KeyCode[] {
            KeyCode.JoystickButton0, KeyCode.JoystickButton1, KeyCode.JoystickButton2, KeyCode.JoystickButton3, KeyCode.JoystickButton4,
            KeyCode.JoystickButton5, KeyCode.JoystickButton6, KeyCode.JoystickButton7, KeyCode.JoystickButton8, KeyCode.JoystickButton9,
            KeyCode.JoystickButton10, KeyCode.JoystickButton11, KeyCode.JoystickButton12, KeyCode.JoystickButton13, KeyCode.JoystickButton14,
            KeyCode.JoystickButton15, KeyCode.JoystickButton16, KeyCode.JoystickButton17, KeyCode.JoystickButton18, KeyCode.JoystickButton19
        };
#endif

        protected float timeSinceLastDetection;
        protected Vector3 lastMousePosition;
        protected bool wasMouseMovedInLastInterval;
        protected bool wasKeyboardPressedInLastInterval;
        protected bool wasControllerKeyPressedInLastInterval;
        protected bool wasTouchedInLastInterval;

        public static string Log;

        public void LateUpdate()
        {
            if (wasKeyboardPressedInLastInterval == false && IsKeyboardKeyPressed())
            {
                wasKeyboardPressedInLastInterval = true;
            }

            if (wasControllerKeyPressedInLastInterval == false && IsControllerKeyPressed())
            {
                wasControllerKeyPressedInLastInterval = true;
            }

            if (wasTouchedInLastInterval == false && Input.touchCount > 0 && IsTouchSupported())
            {
                wasTouchedInLastInterval = true;
            }

            // ignore mouse input if touch was detected
            if (wasMouseMovedInLastInterval == false && lastMousePosition != Input.mousePosition && Input.touchCount == 0 && !wasTouchedInLastInterval)
            {
                wasMouseMovedInLastInterval = true;
            }

            timeSinceLastDetection -= Time.unscaledDeltaTime;
            if (timeSinceLastDetection < 0)
            {
                timeSinceLastDetection = DetectionIntervalInSec;

                Detect();

                wasKeyboardPressedInLastInterval = false;
                wasControllerKeyPressedInLastInterval = false;
                wasTouchedInLastInterval = false;
                wasMouseMovedInLastInterval = false;
                lastMousePosition = Input.mousePosition;
            }
            /*
            Log = $"CTD: wasKey[{wasKeyboardPressedInLastInterval}], isKey[{IsKeyboardKeyPressed()}]";
            Log += $" wasCtrl[{wasControllerKeyPressedInLastInterval}], isCtrl[{IsControllerKeyPressed()}]";
            Log += $" wasTch[{wasTouchedInLastInterval}], isTch[{Input.touchCount > 0 && IsTouchSupported()}]";
            Log += $" wasMouse[{wasMouseMovedInLastInterval}], isMouse[{lastMousePosition != Input.mousePosition && Input.touchCount == 0 && !wasTouchedInLastInterval}]";
            Log += $" mode[{Mode}]";
            foreach (var hdw in Hardware)
            {
                Log += $" hdw[{hdw}]";
            }*/
        }

        public void Detect()
        {
            detectHardware();
            detectMode();
        }

        /// <summary>
        /// An alias for "GuessUsedHardware() == hardware".
        /// Returns wheter or not the current hardware is used at the moment (can change during runtime).
        /// Takes control mode changes into account.
        /// </summary>
        /// <param name=""></param>
        /// <returns></returns>
        public bool IsHardwareUsed(ControlHardware hardware)
        {
            return GuessUsedHardware() == hardware;
        }

        /// <summary>
        /// Tries to make a reasonable guess of the currently used hardware (can change during runtime).
        /// Takes control mode changes into account.
        /// </summary>
        /// <returns></returns>
        public ControlHardware GuessUsedHardware()
        {
// #if UNITY_EDITOR
//             if (UtilsEditorPrefs.IsForceTouchPrefOn())
//             {
//                 return ControlHardware.Touch;
//             }
// #endif

            // Special case for Safari on iOS (it always reports mouse unless the screen is actually touched).
#if UNITY_WEBGL
            if (Application.isMobilePlatform || MobileDeviceDetector.IsMobileBrowser())
            {
                return ControlHardware.Touch;
            }
#endif

            if (Hardware.Contains(ControlHardware.Controller))
            {
                return ControlHardware.Controller;
            }
            else if ( (Mode == ControlMode.Pointer || Mode == ControlMode.Key) && Hardware.Contains(ControlHardware.Keyboard) )
            {
                return ControlHardware.Keyboard;
            }
            else if(Hardware.Contains(ControlHardware.Touch))
            {
                return ControlHardware.Touch;
            }
            else
            {
                return ControlHardware.Keyboard;
            }
        }

        public bool IsMode(params ControlMode[] modes)
        {
            for (int i = 0; i < modes.Length; i++)
            {
                if (modes[i] == Mode)
                    return true;
            }
            return false;
        }

#if ENABLE_INPUT_SYSTEM && KAMGAM_UGUINAVIGATION_INPUT_SYSTEM
        protected void detectHardware()
        {
            if (InputDetector.LastInputDevice == InputDetector.InputDevice.Touch)
            {
                if (Hardware.Contains(ControlHardware.Touch) == false)
                {
                    Hardware.Add(ControlHardware.Touch);
                    OnHardwareChanged?.Invoke(Hardware, ControlHardware.Touch, null);
                }
            }
            else
            {
                if (Hardware.Contains(ControlHardware.Touch) == true)
                {
                    Hardware.Remove(ControlHardware.Touch);
                    OnHardwareChanged?.Invoke(Hardware, null, ControlHardware.Touch);
                }
            }

            if (InputDetector.LastInputDevice == InputDetector.InputDevice.Controller)
            {
                if (Hardware.Contains(ControlHardware.Controller) == false)
                {
                    Hardware.Add(ControlHardware.Controller);
                    OnHardwareChanged?.Invoke(Hardware, ControlHardware.Controller, null);
                }
            }
            else
            {
                if (Hardware.Contains(ControlHardware.Controller) == true)
                {
                    Hardware.Remove(ControlHardware.Controller);
                    OnHardwareChanged?.Invoke(Hardware, null, ControlHardware.Controller);
                }
            }

            if (InputDetector.LastInputDevice == InputDetector.InputDevice.Keyboard)
            {
                if (Hardware.Contains(ControlHardware.Keyboard) == false)
                {
                    Hardware.Add(ControlHardware.Keyboard);
                    OnHardwareChanged?.Invoke(Hardware, ControlHardware.Keyboard, null);
                }
            }
            else
            {
                if (Hardware.Contains(ControlHardware.Keyboard) == true)
                {
                    Hardware.Remove(ControlHardware.Keyboard);
                    OnHardwareChanged?.Invoke(Hardware, null, ControlHardware.Keyboard);
                }
            }
        }
#else
        protected void detectHardware()
        {
            if (IsTouchSupported())
            {
                if (Hardware.Contains(ControlHardware.Touch) == false)
                {
                    Hardware.Add(ControlHardware.Touch);
                    OnHardwareChanged?.Invoke(Hardware, ControlHardware.Touch, null);
                }
            }
            else
            {
                if (Hardware.Contains(ControlHardware.Touch) == true)
                {
                    Hardware.Remove(ControlHardware.Touch);
                    OnHardwareChanged?.Invoke(Hardware, null, ControlHardware.Touch);
                }
            }

            if (IsWellKnownControllerConnected() || wasControllerKeyPressedInLastInterval)
            {
                if (Hardware.Contains(ControlHardware.Controller) == false)
                {
                    Hardware.Add(ControlHardware.Controller);
                    OnHardwareChanged?.Invoke(Hardware, ControlHardware.Controller, null);
                }
            }
            else
            {
                if (Hardware.Contains(ControlHardware.Controller) == true)
                {
                    Hardware.Remove(ControlHardware.Controller);
                    OnHardwareChanged?.Invoke(Hardware, null, ControlHardware.Controller);
                }
            }

            if (IsKeyboardConnected() || wasKeyboardPressedInLastInterval)
            {
                if (Hardware.Contains(ControlHardware.Keyboard) == false)
                {
                    Hardware.Add(ControlHardware.Keyboard);
                    OnHardwareChanged?.Invoke(Hardware, ControlHardware.Keyboard, null);
                }
            }
            else
            {
                if (Hardware.Contains(ControlHardware.Keyboard) == true)
                {
                    Hardware.Remove(ControlHardware.Keyboard);
                    OnHardwareChanged?.Invoke(Hardware, null, ControlHardware.Keyboard);
                }
            }
        }
#endif

        /// <summary>
        /// Mode priorityies: touch > controller > keyboard > mouse
        /// </summary>
        protected void detectMode()
        {
            // Special case for Safari on iOS (it always reports mouse unless the screen is actually touched).
            bool isTouch = false;
#if UNITY_WEBGL
            if (Application.isMobilePlatform || MobileDeviceDetector.IsMobileBrowser())
            {
                isTouch = true;
            }
#endif

            if (wasTouchedInLastInterval || isTouch)
            {
                if (Mode != ControlMode.Touch)
                {
                    ControlMode was = Mode;
                    Mode = ControlMode.Touch;
                    onModeChanged(was, Mode);
                }
            }
            else if (wasControllerKeyPressedInLastInterval)
            {
                if (Mode != ControlMode.Key)
                {
                    ControlMode was = Mode;
                    Mode = ControlMode.Key;
                    onModeChanged(was, Mode);
                }
            }
            else if (wasKeyboardPressedInLastInterval)
            {
                if (Mode != ControlMode.Key)
                {
                    ControlMode was = Mode;
                    Mode = ControlMode.Key;
                    onModeChanged(was, Mode);
                }
            }
            else if (wasMouseMovedInLastInterval)
            {
//#if UNITY_EDITOR
//                if (UtilsEditorPrefs.IsForceTouchPrefOn())
//                {
//                    if (Mode != ControlMode.Touch)
//                    {
//                        ControlMode was = Mode;
//                        Mode = ControlMode.Touch;
//                        onModeChanged(was, Mode);
//                    }
//                    return;
//                }
//#endif
                if (Mode != ControlMode.Pointer)
                {
                    ControlMode was = Mode;
                    Mode = ControlMode.Pointer;
                    onModeChanged(was, Mode);
                }
            }
            
        }

        protected void onModeChanged(ControlMode was, ControlMode isNow)
        {
            OnModeChanged?.Invoke(was, isNow);

            if (LogEnabled)
            {
                Debug.Log("ControlDetector: Mode change from " + was + " to " + isNow + ", frame: " + Time.frameCount + ", i: " + this.GetInstanceID());
                Debug.Log("ControlDetector: current selected: " + EventSystem.current.currentSelectedGameObject + ", frame: " + Time.frameCount + ", i: " + this.GetInstanceID());
            }
        }

        public override string ToString()
        {
            string str = "Mode: " + Mode.ToString() + ", Hardw.: ";
            foreach (var h in Hardware)
            {
                str += h.ToString() + ", ";
            }
            var joystickNames = Input.GetJoystickNames();
            for (int i = 0; i < joystickNames.Length; i++)
            {
                if (string.IsNullOrEmpty(joystickNames[i]) == false)
                {
                    str += ", " + joystickNames[i];
                }
            }
            return str;
        }

#region StaticAPI

        /// <summary>
        /// Is a controller (joystick) key pressed?
        /// </summary>
        /// <returns></returns>
        public static bool IsControllerKeyPressed()
        {
#if ENABLE_INPUT_SYSTEM && KAMGAM_UGUINAVIGATION_INPUT_SYSTEM
            foreach (var gamepad in Gamepad.all)
            {
                if (gamepad.allControls.Any(control => control is ButtonControl button && button.wasPressedThisFrame))
                {
                    return true;
                }
            }

            return false;
#else
            if (Input.anyKeyDown)
            {
                for (int i = 0; i < ControllerKeyCodes.Length; i++)
                {
                    if (Input.GetKeyDown(ControllerKeyCodes[i]))
                        return true;
                }
            }

            // InControl Support (Rewired would be similar)
//            if (InControl.InputManager.ActiveDevice.DeviceClass == InControl.InputDeviceClass.Controller)
//            {
//                if (   InControl.InputManager.ActiveDevice.AnyButtonWasPressed
//                    || InControl.InputManager.ActiveDevice.DPad.WasPressed
//                    || InControl.InputManager.ActiveDevice.LeftTrigger.WasPressed
//                    || InControl.InputManager.ActiveDevice.LeftStick.WasPressed
//                    || InControl.InputManager.ActiveDevice.RightTrigger.WasPressed
//                    || InControl.InputManager.ActiveDevice.RightStick.WasPressed)
//                {
//                    return true;
//                }
//            }

            return false;
#endif
        }

        /// <summary>
        /// Is a keyboard key pressed?
        /// Controller keys (joystick keys) are ignored.
        /// Notice that some keys like "Return, Escape" are ignore on touch devices because those are also used by those devices.
        /// </summary>
        /// <returns></returns>
        public static bool IsKeyboardKeyPressed()
        {
#if ENABLE_INPUT_SYSTEM && KAMGAM_UGUINAVIGATION_INPUT_SYSTEM
            return Keyboard.current != null && Keyboard.current.anyKey.wasPressedThisFrame;
#else
            if (Input.anyKeyDown)
            {
                // No keyboard if mouse keys are pressed
                if (Input.GetMouseButtonDown(0) || Input.GetMouseButtonDown(1) || Input.GetMouseButtonDown(2))
                {
                    return false;
                }

                // Assume no keyboard if touch device and touch key
                if (Input.touchSupported)
                {
                    for (int i = 0; i < TouchKeyCodes.Length; i++)
                    {
                        if (Input.GetKeyDown(TouchKeyCodes[i]))
                            return false;
                    }
                }

                // No keyboard if controller
                for (int i = 0; i < ControllerKeyCodes.Length; i++)
                {
                    if (Input.GetKeyDown(ControllerKeyCodes[i]))
                        return false;
                }

                return true;
            }

            return false;
#endif
        }

        /// <summary>
        /// If it is not a touch device or if it has a mouse then we assume there is a keyboard.
        /// </summary>
        /// <returns></returns>
        public static bool IsKeyboardConnected()
        {
#if ENABLE_INPUT_SYSTEM && KAMGAM_UGUINAVIGATION_INPUT_SYSTEM
            return Keyboard.current != null;
#else
            bool hasKeyboard;
            if (Input.touchSupported == false)
            {
                // No touch support, most likely a desktop, console or key-only mobile device,
                // thus we assume a keyboard is connected.
                hasKeyboard = true;
            }
            else
            {
                // Touch is available but what about devices with attachable keyboards?
                // Let´s assume that if a mouse is present we usually also have a keyboard.
                hasKeyboard = Input.mousePresent;
            }
            return hasKeyboard;
#endif
        }

        /// <summary>
        /// Detects if an xbox or playstation controller is connected.
        /// </summary>
        /// <returns>False if an unknown controller is connected.</returns>
        public static bool IsWellKnownControllerConnected()
        {
#if ENABLE_INPUT_SYSTEM && KAMGAM_UGUINAVIGATION_INPUT_SYSTEM
            return Gamepad.all.Count > 0;
#else
            var joystickNames = Input.GetJoystickNames();
            for (int i = 0; i < joystickNames.Length; i++)
            {
                if (string.IsNullOrEmpty(joystickNames[i]) == false)
                {
                    string lowerName = joystickNames[i].ToLower();
                    // XBox and PS Controller
                    // Thanks to: https://github.com/pbhogan/InControl/tree/master/Assets/InControl/Source/Unity/DeviceProfiles
                    if (
                        // XBOX
                           lowerName.Contains("xbox")
                        || lowerName.Contains("x-box")
                        // PS
                        || lowerName.Contains("playstation")
                        || lowerName.Contains("play-station")
                        // SWITCH
                        || lowerName.Contains("npad")
                        || lowerName.Contains("joy-con")
                        || lowerName.Contains("switch")
                        || lowerName.Contains("pro")
                        // Generic (probably Switch)
                        || lowerName.Contains("gamepad")
                        )
                    {
                        return true;
                    }
                }
            }
            return false;
#endif
        }

#pragma warning disable CS0162
        public static bool IsTouchSupported()
        {
//#if UNITY_EDITOR
//            if (UtilsEditorPrefs.IsForceTouchPrefOn())
//            {
//                return true;
//            }
//#endif

            // Special case for Safari on iOS (it always reports mouse unless the screen is actually touched).
#if UNITY_WEBGL
            if (Application.isMobilePlatform || MobileDeviceDetector.IsMobileBrowser())
            {
                return true;
            }
#endif

#if ENABLE_INPUT_SYSTEM && KAMGAM_UGUINAVIGATION_INPUT_SYSTEM
            return Touchscreen.current != null;
#else
            return Input.touchSupported;
#endif
        }
#pragma warning restore CS0162

        #endregion
    }
}