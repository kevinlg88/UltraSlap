using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using System;

namespace Kamgam.UGUINavigationWizard
{
    /// <summary>
    /// This component is nothing but a marker that tells the navigation wizard system to ignore
    /// this object in automic wizard mode (i.e. no "UGUINavigationWizard" will be added).
    /// <br /><br />
    /// It is recommended to use this only for highly dynamic UIs where adding the wizard to the scene objects
    /// is not viable.
    /// </summary>
    [AddComponentMenu("UI/Navigation Wizard Helpers/Navigation Wizard Automator")]
    [DefaultExecutionOrder(-2)]
    public class UGUINavigationWizardAutomator : MonoBehaviour
    {
        /// <summary>
        /// List of all currently active automators (global and local ones).
        /// </summary>
        public static List<UGUINavigationWizardAutomator> Automators = new List<UGUINavigationWizardAutomator>();

        [SerializeField]
        protected UGUINavigationWizardSettings m_Settings;

        [Tooltip("If you want to save on performance then you can configure it to only run every n-th frame.")]
        public int UpdaterEveryNthFrame = 1;

        [Tooltip("If enabled then it will add wizards to all selectables.\n\n" +
                  "If disabled then it will only add wizards to selectables within its own children.")]
        public bool IsGlobal = false;

        [Tooltip("Usually there should be only ONE automator in your project. Disable if you want to manually manage automator and use multiple.")]
        public bool DontDestroy = false;

        /// <summary>
        /// The instance of the null resolver that was enabled (or awoken) most recently.
        /// </summary>
        public static UGUINavigationWizardAutomator Instance
        {
            get
            {
                if (Automators.Count > 0)
                {
                    // Return the most recently added active GLOBAL instances
                    for (int i = Automators.Count - 1; i >= 0; i--)
                    {
                        if (Automators[i].isActiveAndEnabled && Automators[i].IsGlobal)
                            return Automators[i];
                    }

                    // Return the most recently added active instance
                    for (int i = Automators.Count - 1; i >= 0; i--)
                    {
                        if (Automators[i].isActiveAndEnabled)
                            return Automators[i];
                    }
                }

                return null;
            }
        }

        [System.NonSerialized]
        protected static List<Selectable> s_HandledSelectables = new List<Selectable>(30);

        [System.NonSerialized]
        protected static Selectable[] s_AllSelectables = new Selectable[30];

        protected static int updateAllSelectablesList()
        {
            int count = Selectable.allSelectableCount;
            
            if (s_AllSelectables.Length < count)
            {
                s_AllSelectables = new Selectable[count + 10];
            }
            Selectable.AllSelectablesNoAlloc(s_AllSelectables);

            return count;
        }

        public void Awake()
        {
            if (!Automators.Contains(this))
                Automators.Add(this);

            if (DontDestroy)
                DontDestroyOnLoad(this);
        }

        public void OnEnable()
        {
            if (!Automators.Contains(this))
                Automators.Add(this);
        }

        public void OnDisable()
        {
            Automators.Remove(this);
        }

        public void OnDestroy()
        {
            Automators.Remove(this);
        }

        public void LateUpdate()
        {
            if (Time.frameCount % UpdaterEveryNthFrame == 0)
            {
                CreateWizards(permanent: false);
            }
        }

        /// <summary>
        /// Adds a wizard component to any selectable that it can find
        /// (except if that has the UGUIIgnoreNavigationWizardAutomation)
        /// component added to it.
        /// </summary>
        public void CreateWizards(bool permanent, IList<UGUINavigationWizard> createdWizards = null)
        {
            var settings = GetSettings();

            int numOfSelectables = updateAllSelectablesList();
            for (int i = 0; i < numOfSelectables; i++)
            {
                var sel = s_AllSelectables[i];
                if (sel != null
                    && !s_HandledSelectables.Contains(sel)
                    && (IsGlobal || sel.transform.IsChildOf(transform))
                    && !sel.TryGetComponent<UGUINavigationWizard>(out var wizard)
                    && !sel.TryGetComponent<UGUINavigationWizardIgnoreAutomation>(out _))
                {
                    wizard = sel.gameObject.AddComponent<UGUINavigationWizard>();
                    wizard.SetSettings(settings);
                    if (!permanent)
                    {
                        wizard.hideFlags = HideFlags.DontSave;
                    }

                    s_HandledSelectables.Add(sel);

                    if (createdWizards != null)
                        createdWizards.Add(wizard);
                }
            }
        }

        public void DestroyWizards()
        {
            var settings = GetSettings();

            int numOfSelectables = updateAllSelectablesList();
            for (int i = 0; i < numOfSelectables; i++) 
            {
                var sel = s_AllSelectables[i];
                if (sel != null
                    && (IsGlobal || sel.transform.IsChildOf(transform))
                    && sel.TryGetComponent<UGUINavigationWizard>(out var wizard)
                    )
                {
                    s_HandledSelectables.Remove(sel);
#if UNITY_EDITOR
                    var go = wizard.gameObject;
                    if (UnityEditor.EditorApplication.isPlaying) {
                        GameObject.Destroy(wizard);
                    } else {
                        GameObject.DestroyImmediate(wizard);
                    }
                    UnityEditor.EditorUtility.SetDirty(go);
#else
                    GameObject.Destroy(wizard);
#endif

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

        // Ensure the settings are hooked up or created.
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
    [UnityEditor.CustomEditor(typeof(UGUINavigationWizardAutomator))]
    public class UGUINavigationWizardAutomatorEditor : UnityEditor.Editor
    {
        UGUINavigationWizardAutomator obj;

        public void OnEnable()
        {
            obj = target as UGUINavigationWizardAutomator;
        }

        public override void OnInspectorGUI()
        {
            obj.EditorEnsureSettingsExist();
            base.OnInspectorGUI();

            if (GUILayout.Button("Add Wizard Components"))
            {
                var wizards = new List<UGUINavigationWizard>();
                obj.CreateWizards(permanent: !UnityEditor.EditorApplication.isPlaying, wizards);
                foreach (var wizard in wizards)
                {
                    UnityEditor.EditorGUIUtility.PingObject(wizard.gameObject);
                    UnityEditor.EditorUtility.SetDirty(wizard.gameObject);

                    Logger.LogMessage("Added wizard to: " + wizard.gameObject, wizard.gameObject);
                }
            }

            if (GUILayout.Button("Remove Wizard Components"))
            {
                var wizards = new List<UGUINavigationWizard>();
                obj.DestroyWizards();
            }
        }
    }
#endif
}
