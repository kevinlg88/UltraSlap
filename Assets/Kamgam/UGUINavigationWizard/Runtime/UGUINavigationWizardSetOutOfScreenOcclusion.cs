using UnityEngine;
using System.Collections.Generic;

namespace Kamgam.UGUINavigationWizard
{
    /// <summary>
    /// Helper component to set the OutOfScreenOcclusion on all children of this transform.<br />
    /// Useful for scrollviews (add this to the viewport).
    /// </summary>
    [AddComponentMenu("UI/Navigation Wizard Helpers/Navigation Wizard Set OutOfScreen Occlusion")]
    [DefaultExecutionOrder(-4)]
    public class UGUINavigationWizardSetOutOfScreenOcclusion : MonoBehaviour
    {
        static List<UGUINavigationWizard> s_TmpWizards = new List<UGUINavigationWizard>();

        public OutOfScreenOcclusion OutOfScreenOcclusion = OutOfScreenOcclusion.InheritFromGlobalSettings;
        public bool IncludeInactive = true;

        public void OnEnable()
        {
            s_TmpWizards.Clear();
            transform.GetComponentsInChildren<UGUINavigationWizard>(IncludeInactive, s_TmpWizards);
            foreach (var wizard in s_TmpWizards)
            {
                wizard.OutOfScreenOcclusion = OutOfScreenOcclusion;
            }
            s_TmpWizards.Clear();
        }
    }
}
