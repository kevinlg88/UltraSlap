using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Kamgam.UGUINavigationWizard
{
    /// <summary>
    /// A component the resolves null selections at runtime.
    /// It remembers the last selected viewport position and 
    /// if the selection becomes NULL then it will try to find 
    /// and select the closest selectable to that position.
    /// <br /><br />
    /// NOTICE: This component requires settings access. 
    /// If you instantiate if via code then make sure to use
    /// to call SetSettings(..) on it.
    /// <br /><br />
    /// NOTICE: It takes constraints into account (that's why
    /// it needs access to the settings object).
    /// </summary>
    [AddComponentMenu("UI/Navigation Wizard Helpers/Navigation Wizard Null Resolver")]
    [DefaultExecutionOrder(-1)]
    public class UGUINavigationWizardNullResolver : MonoBehaviour
    {
        protected static Selectable[] s_AllSelectables = new Selectable[30];

        /// <summary>
        /// List of all currently active null resolvers (global and local ones).
        /// </summary>
        public static List<UGUINavigationWizardNullResolver> NullResolvers = new List<UGUINavigationWizardNullResolver>();

        /// <summary>
        /// The default position that is used for searching selectables in case no previous position is known.
        /// </summary>
        public static Vector2 DefaultPosition = new Vector2(0.5f, 0.5f);

        /// <summary>
        /// The instance of the null resolver that was enabled (or awoken) most recently.
        /// </summary>
        public static UGUINavigationWizardNullResolver Instance
        {
            get
            {
                if (NullResolvers.Count > 0)
                {
                    // Return the most recently added active GLOBAL resolver
                    for (int i = NullResolvers.Count - 1; i >= 0; i--)
                    {
                        if (NullResolvers[i].isActiveAndEnabled && NullResolvers[i].IsGlobal)
                            return NullResolvers[i];
                    }

                    // Return the most recently added active resolver
                    for (int i = NullResolvers.Count-1; i >= 0; i--)
                    {
                        if (NullResolvers[i].isActiveAndEnabled)
                            return NullResolvers[i];
                    }
                }

                return null;
            }
        }

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

        public static void SetAllPaused(bool pause)
        {
            foreach (var resolver in NullResolvers)
            {
                resolver.Pause = pause;
            }
        }

        public static void SelectNull()
        {
            if (EventSystem.current != null && !EventSystem.current.alreadySelecting)
                EventSystem.current.SetSelectedGameObject(null);
        }

        [SerializeField]
        protected UGUINavigationWizardSettings m_Settings;

        [Tooltip("If enabled then it will search in all selectables for candidates.\n\n" +
            "If disabled then it will only search in its own children for selectables.")]
        public bool IsGlobal = false;

        [Tooltip("Useful to prohibit this from being unloaded if using a global null resolver.")]
        public bool DontDestroy = false;

        [System.NonSerialized]
        /// <summary>
        /// The position based on which a new selectable will be chosen if none is currently selected.
        /// </summary>
        public Vector2 LastSelectionViewportPos = DefaultPosition;

        /// <summary>
        /// If paused then null selections will not be resolved.
        /// </summary>
        public bool Pause = false;

        public void Awake()
        {
            if (!NullResolvers.Contains(this))
                NullResolvers.Add(this);

            if (DontDestroy)
                DontDestroyOnLoad(this.gameObject);

#if UNITY_EDITOR
            EditorEnsureSettingsExist();
#endif
        }

        public void OnEnable()
        {
            if(!NullResolvers.Contains(this))
                NullResolvers.Add(this);
        }

        public void OnDisable()
        {
            NullResolvers.Remove(this);
        }

        public void Update()
        {
            if (Pause)
                return;

            if (EventSystem.current == null)
                return;

            var sel = EventSystem.current.currentSelectedGameObject;
            if (sel == null || !sel.gameObject.activeInHierarchy)
            {
                var settings = GetSettings();
                Resolve(LastSelectionViewportPos, this, settings.EnableConstraints, settings.AllowedObjects, settings.ConstraintsIgnoreOcclusion);
            }
            else
            {
                // Memorize last selected viewport position.
                var selRect = sel.transform as RectTransform;
                if (selRect)
                {
                    LastSelectionViewportPos = Utils.RectPosToViewportPos(selRect, DefaultPosition);
                }
            }
        }

        public static void Resolve(
            RectTransform lastKnownRectTransform,
            UGUINavigationWizardNullResolver resolver = null,
            bool useAllowedObjectsConstraint = false,
            List<Selectable> allowedObjects = null,
            bool allowedObjectsIgnoreOcclusion = false)
        {
            Resolve(
                Utils.RectPosToViewportPos(lastKnownRectTransform, DefaultPosition),
                resolver,
                useAllowedObjectsConstraint, allowedObjects, allowedObjectsIgnoreOcclusion
                );
        }

        public static void Resolve(
            Vector2 lastKnownViewportPosition,
            UGUINavigationWizardNullResolver resolver = null,
            bool useAllowedObjectsConstraint = false,
            List<Selectable> allowedObjects = null,
            bool allowedObjectsIgnoreOcclusion = false)
        {
            updateAllSelectablesList();

            bool isGlobal = resolver == null || resolver.IsGlobal;
            Selectable bestPick = null;
            float minDistance = float.MaxValue;
            foreach (var sel in s_AllSelectables)
            {
                if (sel != null 
                    && sel.IsInteractable() 
                    && sel.gameObject != null 
                    && sel.gameObject.activeInHierarchy
                    && (isGlobal || resolver == null || sel.transform.IsChildOf(resolver.transform))
                    && (!useAllowedObjectsConstraint || UGUINavigationWizardSettings.CheckIfIsAllowed(sel, allowedObjects))
                    )
                {
                    var selRect = sel.transform as RectTransform;
                    if (selRect != null)
                    {
                        // 0/0 as default pos because we want it to be BIG if selRect is not valid.
                        var viewportPos = Utils.RectPosToViewportPos(selRect, new Vector2(0f, 0f));
                        float distance = (viewportPos - lastKnownViewportPosition).sqrMagnitude;
                        if (distance < minDistance)
                        {
                            // If we find a wizard then take occlusion into account.
                            bool skipOcclusionDueToAllowedObjects = useAllowedObjectsConstraint && allowedObjectsIgnoreOcclusion && UGUINavigationWizardSettings.CheckIfIsAllowed(sel, allowedObjects);
                            if (!skipOcclusionDueToAllowedObjects && sel.TryGetComponent<UGUINavigationWizard>(out var wizard) && !wizard.IgnoreOcclusion)
                            {
                                if (!GraphicOcclusionChecker.IsCompletelyVisible(selRect, wizard.IgnoreOutOfScreenOcclusion))
                                    continue;
                            }

                            minDistance = distance;
                            bestPick = sel;
                        }
                    }
                }
            }

            if (bestPick != null && !EventSystem.current.alreadySelecting)
            {
                EventSystem.current.SetSelectedGameObject(bestPick.gameObject);
            }
        }

        public void OnDestroy()
        {
            NullResolvers.Remove(this);
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
    [UnityEditor.CustomEditor(typeof(UGUINavigationWizardNullResolver))]
    public class UGUINavigationWizardNullResolverEditor : UnityEditor.Editor
    {
        UGUINavigationWizardNullResolver obj;

        public void OnEnable()
        {
            obj = target as UGUINavigationWizardNullResolver;
        }

        public override void OnInspectorGUI()
        {
            obj.EditorEnsureSettingsExist();
            base.OnInspectorGUI();
        }
    }
#endif
}
