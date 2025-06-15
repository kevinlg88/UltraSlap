using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using System;

namespace Kamgam.UGUINavigationWizard
{
    /// <summary>
    /// Adds to or removes selectables from constraints.
    /// <br /><br />
    /// You should only ever have one of these enabled that the same time.
    /// <br /><br />
    /// NOTICE: This component requires settings access. 
    /// If you instantiate if via code then make sure to use
    /// to call SetSettings(..) on it.
    /// </summary>
    [AddComponentMenu("UI/Navigation Wizard Helpers/Navigation Wizard Constraint")]
    [DefaultExecutionOrder(-3)]
    public class UGUINavigationWizardConstraint : MonoBehaviour
    {
        /// <summary>
        /// List of all currently active constraints.
        /// </summary>
        public static List<UGUINavigationWizardConstraint> Constraints = new List<UGUINavigationWizardConstraint>();

        [SerializeField]
        protected UGUINavigationWizardSettings m_Settings;

        [Tooltip("If enabled the the list of Objects will be added to the global list of allowed objects in OnEnable() and removed in OnDisable().\n\n" +
            "NOTICE: The constraint will only take effect if 'EnableConstraints' is enabled in the global settings.")]
        public bool ExecuteOnEnable = true;

        [Tooltip("Should constraints be updated in Update(). Useful if you have dynamic objects popping in and out during a tutorial.")]
        public bool ExecuteOnUpdate = true;

        [Tooltip("The list objects that will be added to the list of allowed selections.")]
        public List<Selectable> AllowedObjects = new List<Selectable>();

        [Tooltip("If enabled then the 'interactable' property of all selectables NOT in the 'AllowedObjects' will be set/unset in OnEnable() and OnDisable()")]
        public bool SetInteractability = false;

        protected static Selectable[] s_AllSelectables = new Selectable[30];

        protected static void clearAllSelectables()
        {
            for (int i = s_AllSelectables.Length - 1; i >= 0; i--)
            {
                s_AllSelectables[i] = null;
            }
        }

        protected static void updateAllSelectablesList()
        {
            if (s_AllSelectables.Length < Selectable.allSelectableCount)
            {
                s_AllSelectables = new Selectable[Selectable.allSelectableCount + 10];
            }
            Selectable.AllSelectablesNoAlloc(s_AllSelectables);
        }

        public void Awake()
        {
#if UNITY_EDITOR
            EditorEnsureSettingsExist();
#endif

            if (!Constraints.Contains(this))
                Constraints.Add(this);
        }

        public void OnEnable()
        {
            if (ExecuteOnEnable)
            {
                if (!Constraints.Contains(this))
                    Constraints.Add(this);

                Constrain(SetInteractability);
            }
        }

        public void Update()
        {
            if (ExecuteOnUpdate)
                Constrain(SetInteractability);
        }

        public void OnDisable()
        {
            Constraints.Remove(this);

            if (ExecuteOnEnable)
                LiftConstraint(SetInteractability);

            clearAllSelectables();
        }

        /// <summary>
        /// Checks if the currently selected object is allowed and changes selection if not.
        /// </summary>
        /// <param name="setInteractability">Sets selectables that are active but not allowed to non-interactable. Reverts this in OnDisable().</param>
        public void Constrain(bool setInteractability)
        {
            var settings = GetSettings();

            // Update global allowed list in settings
            foreach (var sel in AllowedObjects)
            {
                if (!settings.AllowedObjects.Contains(sel))
                {
                    settings.AllowedObjects.Add(sel);
                }
            }

            // remove current selection if not allowed
            if (EventSystem.current != null && EventSystem.current.currentSelectedGameObject != null)
            {
                if(EventSystem.current.currentSelectedGameObject.TryGetComponent<Selectable>(out var sel) && !settings.AllowedObjects.Contains(sel))
                {
                    EventSystem.current.SetSelectedGameObject(null);
                    UGUINavigationWizardNullResolver.Resolve(sel.GetComponent<RectTransform>());
                }
            }

            if (SetInteractability)
            {
                updateAllSelectablesList();
                for (int i = 0; i < s_AllSelectables.Length; i++)
                {
                    var sel = s_AllSelectables[i];

                    if (sel == null)
                        continue;

                    if (sel.interactable && !settings.AllowedObjects.Contains(sel))
                    {
                        sel.interactable = false;
                    }
                }
            }
        }

        public void LiftConstraint(bool setInteractability)
        {
            var settings = GetSettings();

            // Update global allowed list in settings
            foreach (var sel in AllowedObjects)
            {
                if (settings.AllowedObjects.Contains(sel))
                {
                    settings.AllowedObjects.Remove(sel);
                }
            }

            if (SetInteractability)
            {
                updateAllSelectablesList();
                for (int i = 0; i < s_AllSelectables.Length; i++)
                {
                    var sel = s_AllSelectables[i];
                    
                    if (sel == null)
                        continue;

                    if (!sel.interactable && !settings.AllowedObjects.Contains(sel))
                    {
                        sel.interactable = true;
                    }
                }
            }
        }

        public UGUINavigationWizardSettings GetSettings()
        {
#if UNITY_EDITOR
            EditorEnsureSettingsExist();
#endif
            return m_Settings;
        }

        public void SetSettings(UGUINavigationWizardSettings settings)
        {
            m_Settings = settings;
        }

#if UNITY_EDITOR
        public void OnValidate()
        {
            editorCheckSettings();
        }

        public void Reset()
        {
            editorCheckSettings();
        }

        private void editorCheckSettings()
        {
            if (!isActiveAndEnabled)
                return;

            if (isSelected())
            {
                EditorEnsureSettingsExist();
            }
        }

        protected bool isSelected()
        {
            return UnityEditor.Selection.activeGameObject == gameObject;
        }

        public void EditorEnsureSettingsExist()
        {
            if (m_Settings == null)
            {
                m_Settings = UGUINavigationWizardSettings.GetOrCreateSettings();
                UnityEditor.EditorUtility.SetDirty(this);
            }
        }
#endif
    }

#if UNITY_EDITOR
    [UnityEditor.CustomEditor(typeof(UGUINavigationWizardConstraint))]
    public class UGUINavigationWizardConstraintEditor : UnityEditor.Editor
    {
        UGUINavigationWizardConstraint obj;

        public void OnEnable()
        {
            obj = target as UGUINavigationWizardConstraint;
        }

        public override void OnInspectorGUI()
        {
            obj.EditorEnsureSettingsExist();
            base.OnInspectorGUI();
        }
    }
#endif
}
