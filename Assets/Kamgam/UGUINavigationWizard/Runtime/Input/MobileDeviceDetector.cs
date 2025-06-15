using System.Runtime.InteropServices;
using UnityEngine;

namespace Kamgam.UGUINavigationWizard
{
    /// <summary>
    /// Helper for reliably detecting mobile touch devices.
    /// Especially iPads are tricky and we need some JavaScript for them.
    /// </summary>
    public static class MobileDeviceDetector
    {
#if UNITY_WEBGL
        [DllImport("__Internal")]
        public static extern bool KamgamIsMobilePlatformUGUINavigationWizard();
#endif

        public static bool IsMobileBrowser()
        {
#if UNITY_EDITOR
            return false; // value to return in Play Mode (in the editor)
#elif UNITY_WEBGL
            return KamgamIsMobilePlatformUGUINavigationWizard(); // value based on the current browser
#else
            return Application.isMobilePlatform;
#endif
        }
    }
}