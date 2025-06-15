using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Kamgam.UGUINavigationWizard.Examples
{
    public class FrameRate : MonoBehaviour
    {
        public int FramesPerSecond = 60;

        void Start()
        {
            Application.targetFrameRate = FramesPerSecond;
        }
    }
}
