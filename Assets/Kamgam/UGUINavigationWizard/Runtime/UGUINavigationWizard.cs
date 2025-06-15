using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using System;

namespace Kamgam.UGUINavigationWizard
{
    public enum NavigationStrategy
    {
        None, Overrides, Automatic, UnityNative
    }

    public enum SelectNullIfOccluded
    {
        InheritFromGlobalSettings, SelectNull, DontChange
    }

    public enum OcclusionDetection
    {
        InheritFromGlobalSettings, Enable, Disable
    }

    public enum OutOfScreenOcclusion
    {
        InheritFromGlobalSettings, Enable, Disable
    }

    [System.Serializable]
    public class NavigationAutomaticStrategyParameters
    {
        /// <summary>
        /// The maximum distance of the rect scan perpedicular to the edge.
        /// </summary>
        [Tooltip("The maximum distance of the rect scan perpedicular to the edge.")]
        public float ScanDistance = 2000f;

        /// <summary>
        /// The margin width that should be added to the scan rect size along both directions of the edge.
        /// </summary>
        [Tooltip("The margin width that should be added to the scan rect size along both directions of the edge.")]
        public float ScanMargin = 2000f;

        /// <summary>
        /// The higher the sensitivity the more directed (up, down, left, right) the navigation will be.
        /// </summary>
        [Range(0f, 100f)]
        [Tooltip("The higher the sensitivity the more directed (up, down, left, right) the navigation will be.")]
        public float AngleSensitivity = 20f;
    }

    /// <summary>
    /// Takes control of the "Navigation" property of the selectable.
    /// It sets the navigation to explicit and fills the four directions.
    /// <br /><br />
    /// NOTICE: This component requires settings access. 
    /// If you instantiate if via code then make sure to use
    /// to call SetSettings(..) on it.
    /// <br /><br />
    /// NOTICE: It takes constraints into account (based on
    /// global settings).
    /// </summary>
    [AddComponentMenu("UI/Navigation Wizard")]
    [DefaultExecutionOrder(-1)]
    public class UGUINavigationWizard : MonoBehaviour, ISelectHandler, IUpdateSelectedHandler
    {
        [SerializeField]
        protected UGUINavigationWizardSettings m_Settings;

        protected Selectable selectable;
        public Selectable Selectable
        {
            get
            {
                if (selectable == null)
                {
                    this.TryGetComponent<Selectable>(out selectable);
                }
                return selectable;
            }
        }

        public SelectNullIfOccluded SelectNullIfOccluded = SelectNullIfOccluded.InheritFromGlobalSettings;

        [Header("Blocking")]
        public bool UpBlocked = false;
        public bool DownBlocked = false;
        public bool LeftBlocked = false;
        public bool RightBlocked = false;
        [Space(5)]
        [Tooltip("If enabled then this element will not be considered as a valid auto-navigation targert. However, you will still be able to navigate to it explicitly.")]
        public bool BlockIncomingAutoNavigation = false;

        [Header("Direction Overrides")]
        public List<Selectable> UpOverrides;
        public bool UpOverridesAutoNav = false;

        public List<Selectable> DownOverrides;
        public bool DownOverridesAutoNav = false;

        public List<Selectable> LeftOverrides;
        public bool LeftOverridesAutoNav = false;

        public List<Selectable> RightOverrides;
        public bool RightOverridesAutoNav = false;

        [Header("Auto Navigation")]
        public bool UseAutoNavigation = true;
        public bool CustomizeAutomaticNavigationUp = false;
        public NavigationAutomaticStrategyParameters AutomaticParametersUp = null;
        public bool CustomizeAutomaticNavigationDown = false;
        public NavigationAutomaticStrategyParameters AutomaticParametersDown = null;
        public bool CustomizeAutomaticNavigationLeft = false;
        public NavigationAutomaticStrategyParameters AutomaticParametersLeft = null;
        public bool CustomizeAutomaticNavigationRight = false;
        public NavigationAutomaticStrategyParameters AutomaticParametersRight = null;

        public OcclusionDetection Occlusion = OcclusionDetection.InheritFromGlobalSettings;
        public bool IgnoreOcclusion
        {
            get
            {
                if (Occlusion == OcclusionDetection.InheritFromGlobalSettings)
                {
                    var settings = GetSettings();
                    if (settings == null)
                        return false;
                    else
                        return !settings.DefaultOcclusion;
                }
                else
                {
                    return Occlusion == OcclusionDetection.Disable;
                }
            }
        }

        public OutOfScreenOcclusion OutOfScreenOcclusion = OutOfScreenOcclusion.InheritFromGlobalSettings;
        public bool IgnoreOutOfScreenOcclusion
        {
            get
            {
                if (OutOfScreenOcclusion == OutOfScreenOcclusion.InheritFromGlobalSettings)
                {
                    var settings = GetSettings();
                    if (settings == null)
                        return false;
                    else
                        return !settings.DefaultOutOfScreenOcclusion;
                }
                else
                {
                    return OutOfScreenOcclusion == OutOfScreenOcclusion.Disable;
                }
            }
        }

        [Header("Unity Native Navigation")]
        public bool UseUnityNative = true;

        protected RectTransform m_rectTransform;
        public RectTransform RectTransform
        {
            get
            {
                if (m_rectTransform == null)
                {
                    m_rectTransform = transform as RectTransform;
                }
                return m_rectTransform;
            }
        }

        protected Canvas _canvas;
        public Canvas Canvas
        {
            get
            {
                if (_canvas == null)
                {
                    _canvas = this.gameObject.GetComponentInParent<Canvas>();
                }
                return _canvas;
            }
        }

        /// <summary>
        /// Increase this value to save on performance.
        /// </summary>
        public static int UpdateNavigationEveryNthFrame = 1;

        [System.NonSerialized]
        protected int m_SelectedFrameCount = 0;


        public void OnEnable()
        {
            // Don't remove this. We want this to get the enabled flag in the inspector.
            UGUINavigationWizardSettings.BindLoggerLevelToSetting(GetSettings());
            m_SelectedFrameCount = 0;

            if (!IsEnabled())
                return;

            Apply();
        }

        public UGUINavigationWizardSettings GetSettings()
        {
#if UNITY_EDITOR
            ensureSettingsExist();
#endif
            return m_Settings;
        }

        public void SetSettings(UGUINavigationWizardSettings settings)
        {
            m_Settings = settings;
        }

        public bool IsEnabled()
        {
            var settings = GetSettings();
            if (settings != null && !settings.UseOnTouchDevices)
            {
#if UNITY_EDITOR
                if (!UnityEditor.EditorApplication.isPlaying)
                    return enabled;
                else if (ControlDetector.Instance == null)
                    return false;
                else
                    return ControlDetector.Instance.Mode != ControlMode.Touch;
#else
                return ControlDetector.Instance.Mode != ControlMode.Touch;
#endif
            }
            else
            {
                return enabled;
            }
        }

        public void OnSelect(BaseEventData eventData)
        {
            if (!IsEnabled())
                return;

            Apply();
        }

        // This is executed every frame while the Selectable is selected.
        public void OnUpdateSelected(BaseEventData eventData)
        {
            if (!IsEnabled())
                return;

            if (UpdateNavigationEveryNthFrame == 1)
            {
                Apply();

                // If this can be occluded then check if it is occluded. If yes then try to find
                // another suitable selectable
                if (!IgnoreOcclusion && getResolvedSelectNullIfOccluded() && EventSystem.current != null)
                {
                    if (Selectable.IsInteractable() && !skipOcclusionCheckDueToConstraints(Selectable) && !GraphicOcclusionChecker.IsCompletelyVisible(RectTransform, IgnoreOutOfScreenOcclusion))
                    {
                        // Notice: If a NullResolver is in the scene then this will trigger it (in the next frame).
                        EventSystem.current.SetSelectedGameObject(null);
                    }
                }
            }
            else
            {
                m_SelectedFrameCount++;
                if (m_SelectedFrameCount % UpdateNavigationEveryNthFrame == 0)
                {
                    Apply();
                }
            }
        }

        protected bool skipOcclusionCheckDueToConstraints(Selectable seletable)
        {
            var settings = GetSettings();
            if (settings == null)
                return false;

            return settings.EnableConstraints && settings.ConstraintsIgnoreOcclusion && settings.IsAllowed(seletable);
        }

#if UNITY_EDITOR
        public void OnDrawGizmos()
        {
            if (!IsEnabled())
                return;

            if (isSelected())
            {
                ensureSettingsExist();

                var settings = GetSettings();
                if (settings == null)
                    return;

                Apply(drawGizmos: GetSettings().DebugVisualization);
            }
        }

        public void OnValidate()
        {
            if (isSelected())
            {
                ensureSettingsExist();
            }
        }

        protected bool isSelected()
        {
            return UnityEditor.Selection.activeGameObject == gameObject;
        }

        private void ensureSettingsExist()
        {
            if (m_Settings == null)
            {
                m_Settings = UGUINavigationWizardSettings.GetOrCreateSettings();
                UnityEditor.EditorUtility.SetDirty(this);
            }
        }
#endif

        public void Apply(bool drawGizmos = false)
        {
            if (this == null || this.gameObject == null || !isActiveAndEnabled || RectTransform == null)
                return;

            if (Selectable == null)
                return;

            Navigation navigation = Selectable.navigation;

            // Change to explicit
            if (navigation.mode != Navigation.Mode.Explicit)
            {
                // Logger.Log("Changed navigation to explicit");
                navigation.mode = Navigation.Mode.Explicit;
            }

            // In the editor if we are in the editor and we are not playing and visualizing is ON,
            // then lock navigation if occluded (if occlusion is used). This makes all the preview
            // arrows go away.
#if UNITY_EDITOR
            if (UGUINavigationWizardEditor.IsVisualizing())
            {
                var settings = GetSettings();
                if (settings != null && settings.DefaultOcclusion && !IgnoreOcclusion && !skipOcclusionCheckDueToConstraints(Selectable))
                {
                    bool isOccluded = !GraphicOcclusionChecker.IsCompletelyVisible(RectTransform, IgnoreOutOfScreenOcclusion);
                    if (isOccluded)
                    {
                        navigation.mode = Navigation.Mode.Explicit;
                        navigation.selectOnUp = null;
                        navigation.selectOnDown = null;
                        navigation.selectOnLeft = null;
                        navigation.selectOnRight = null;
                        Selectable.navigation = navigation;
                        return;
                    }
                }
            }
#endif

            // Find selectables in all directions.
            navigation.selectOnUp = FindSelectable(Vector3.up, drawGizmos);
            navigation.selectOnDown = FindSelectable(Vector3.down, drawGizmos);
            navigation.selectOnLeft = FindSelectable(Vector3.left, drawGizmos);
            navigation.selectOnRight = FindSelectable(Vector3.right, drawGizmos);

            // Apply
            Selectable.navigation = navigation;
        }

        public bool HasActiveOverrides()
        {
            if (!UpBlocked)
            {
                foreach (var sel in UpOverrides)
                {
                    if (sel != null && sel.isActiveAndEnabled && sel.gameObject != null && sel.gameObject.activeInHierarchy)
                        return true;
                }
            }

            if (!DownBlocked)
            {
                foreach (var sel in DownOverrides)
                {
                    if (sel != null && sel.isActiveAndEnabled && sel.gameObject != null && sel.gameObject.activeInHierarchy)
                        return true;
                }
            }

            if (!LeftBlocked)
            {
                foreach (var sel in LeftOverrides)
                {
                    if (sel != null && sel.isActiveAndEnabled && sel.gameObject != null && sel.gameObject.activeInHierarchy)
                        return true;
                }
            }

            if (!RightBlocked)
            {
                foreach (var sel in RightOverrides)
                {
                    if (sel != null && sel.isActiveAndEnabled && sel.gameObject != null && sel.gameObject.activeInHierarchy)
                        return true;
                }
            }

            return false;
        }

        public Selectable FindSelectableOverrideOnUp(bool drawGizmos = false)
        {
            if (UpBlocked)
                return null;

            if (UpOverridesAutoNav)
                return FindSelectableImprovedAutomatic(Vector3.up, UpOverrides, drawGizmos: false, out _);

            return getFirstSelectable(UpOverrides, null);
        }

        public Selectable FindSelectableOverrideOnDown(bool drawGizmos = false)
        {
            if (DownBlocked)
                return null;

            if (DownOverridesAutoNav)
                return FindSelectableImprovedAutomatic(Vector3.down, UpOverrides, drawGizmos: false, out _);

            return getFirstSelectable(DownOverrides, null);
        }

        public Selectable FindSelectableOverrideOnLeft(bool drawGizmos = false)
        {
            if (LeftBlocked)
                return null;

            if (LeftOverridesAutoNav)
                return FindSelectableImprovedAutomatic(Vector3.left, UpOverrides, drawGizmos: false, out _);

            return getFirstSelectable(LeftOverrides, null);
        }

        public Selectable FindSelectableOverrideOnRight(bool drawGizmos = false)
        {
            if (RightBlocked)
                return null;

            if (RightOverridesAutoNav)
                return FindSelectableImprovedAutomatic(Vector3.right, UpOverrides, drawGizmos: false, out _);

            return getFirstSelectable(RightOverrides, null);
        }

        protected Selectable getFirstSelectable(List<Selectable> list, Selectable defaultExplicit)
        {
            if (list == null)
                return null;

            for (int i = 0; i < list.Count; i++)
            {
                if (list[i] != null
                    && list[i].isActiveAndEnabled
                    && list[i].IsInteractable()
                    && list[i].gameObject != null
                    && list[i].gameObject.activeInHierarchy)
                {
                    return list[i];
                }
            }

            return defaultExplicit;
        }

        public void AddUpSelectable(Selectable selectable, bool prioritize = false)
        {
            addSelectable(UpOverrides, selectable, prioritize);
        }

        public void AddDownSelectable(Selectable selectable, bool prioritize = false)
        {
            addSelectable(DownOverrides, selectable, prioritize);
        }

        public void AddLeftSelectable(Selectable selectable, bool prioritize = false)
        {
            addSelectable(LeftOverrides, selectable, prioritize);
        }

        public void AddRightSelectable(Selectable selectable, bool prioritize = false)
        {
            addSelectable(RightOverrides, selectable, prioritize);
        }

        protected void addSelectable(List<Selectable> list, Selectable selectable, bool prioritize)
        {
            if (selectable == null)
                return;

            if (list == null)
            {
                list = new List<Selectable>();
            }

            if (list.Contains(selectable) == false)
            {
                if (prioritize)
                {
                    list.Insert(0, selectable);
                }
                else
                {
                    list.Add(selectable);
                }
            }
        }

        public void Clear()
        {
            UpOverrides?.Clear();
            DownOverrides?.Clear();
            LeftOverrides?.Clear();
            RightOverrides?.Clear();
        }

        protected static Vector3[] s_rectFourCornersArray = new Vector3[4];

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

        protected List<NavigationStrategy> getResolvedStrategies(bool up, bool down, bool left, bool right)
        {
            List<NavigationStrategy> strategies = null;
            var settings = GetSettings();
            if (settings == null)
            {
#if UNITY_EDITOR
                Logger.LogWarning("Settings missing on " + this.gameObject.name, this.gameObject);
#endif
                strategies = UGUINavigationWizardSettings.DefaultStrategiesTemplate;
            }
            else
            {
                strategies = settings.DefaultStrategies;
            }

            return strategies;
        }

        protected NavigationAutomaticStrategyParameters getResolvedImprovedAutomaticParameters(bool up, bool down, bool left, bool right)
        {
            NavigationAutomaticStrategyParameters parameters = null;
            var settings = GetSettings();
            if (settings == null)
            {
#if UNITY_EDITOR
                Logger.LogWarning("Settings missing on " + this.gameObject.name, this.gameObject);
#endif
                parameters = UGUINavigationWizardSettings.DefaultAutomaticParametersTemplate;
            }
            else
            {
                parameters = settings.DefaultAutomaticParameters;
            }

            if (CustomizeAutomaticNavigationUp && up && AutomaticParametersUp != null)
            {
                parameters = AutomaticParametersUp;
            }
            else if (CustomizeAutomaticNavigationDown && down && AutomaticParametersDown != null)
            {
                parameters = AutomaticParametersDown;
            }
            else if (CustomizeAutomaticNavigationLeft && left && AutomaticParametersLeft != null)
            {
                parameters = AutomaticParametersLeft;
            }
            else if (CustomizeAutomaticNavigationRight && right && AutomaticParametersRight != null)
            {
                parameters = AutomaticParametersRight;
            }

            return parameters;
        }

        protected bool getResolvedSelectNullIfOccluded()
        {
            var settings = GetSettings();
            if (settings == null)
            {
                return true;
            }
            else if (SelectNullIfOccluded == SelectNullIfOccluded.InheritFromGlobalSettings)
            {
                return settings.DefaultSelectNullIfOccluded;
            }
            else
            {
                return SelectNullIfOccluded == SelectNullIfOccluded.SelectNull;
            }
        }

        public Selectable FindSelectable(Vector3 dir, bool drawGizmos = false)
        {
            Selectable selectable = null;

            getDirection(dir, out bool up, out bool down, out bool left, out bool right);

            if (UpBlocked && up)
                return null;
            else if (DownBlocked && down)
                return null;
            else if (LeftBlocked && left)
                return null;
            else if (RightBlocked && right)
                return null;

            var strategies = getResolvedStrategies(up, down, left, right);
            if (!strategies.IsNullOrEmpty())
            {
                foreach (var strat in strategies)
                {
                    if (selectable == null)
                    {
                        if (strat == NavigationStrategy.Automatic && !UseAutoNavigation)
                            continue;

                        if (strat == NavigationStrategy.UnityNative && !UseUnityNative)
                            continue;

                        bool wasOccluded;
                        selectable = FindSelectable(strat, dir, drawGizmos, out wasOccluded);

                        // Stop using more strategies if it stopped due to occlusion (avoids native selection fallback to kick in).
                        if (selectable == null && wasOccluded)
                            break;
                    }
                    else
                    {
                        break;
                    }
                }
            }

            return selectable;
        }

        /// <summary>
        /// Executes the given strategy for finding selectable.<br />
        /// NOTICE: It does NOT take blocking into account.
        /// </summary>
        /// <param name="strategy"></param>
        /// <param name="dir"></param>
        /// <param name="drawGizmos"></param>
        /// <param name="wasOccluded">Is true if a valid selection would have been found but was rejected due to occlusion.</param>
        /// <returns></returns>
        public Selectable FindSelectable(NavigationStrategy strategy, Vector3 dir, bool drawGizmos, out bool wasOccluded)
        {
            Selectable selectable = null;
            wasOccluded = false;

            switch (strategy)
            {
                case NavigationStrategy.Overrides:
                    getDirection(dir, out bool up, out bool down, out bool left, out bool right);
                    if (up) selectable = FindSelectableOverrideOnUp(drawGizmos);
                    else if (down) selectable = FindSelectableOverrideOnDown(drawGizmos);
                    else if (left) selectable = FindSelectableOverrideOnLeft(drawGizmos);
                    else if (right) selectable = FindSelectableOverrideOnRight(drawGizmos);
                    break;
                case NavigationStrategy.Automatic:
                    updateAllSelectablesList();
                    selectable = FindSelectableImprovedAutomatic(dir, s_AllSelectables, drawGizmos, out wasOccluded);
                    break;
                case NavigationStrategy.UnityNative:
                    selectable = FindSelectableUnityNative(dir);
                    break;
                case NavigationStrategy.None:
                default:
                    break;
            }

            return selectable;
        }

        protected void getDirection(Vector3 dir, out bool up, out bool down, out bool left, out bool right)
        {
            var cam = Utils.GetCamera();

            if (cam == null || isScreenSpaceOverlay())
            {
                up = Vector3.Dot(Vector3.up, dir) > 0f;
                down = Vector3.Dot(Vector3.down, dir) > 0f;
                left = Vector3.Dot(Vector3.left, dir) > 0f;
                right = Vector3.Dot(Vector3.right, dir) > 0f;
                return;
            }

            dir = cam.transform.TransformVector(dir); // from camera space to world space
            dir = dir.normalized;

            // Directions of camera in world space
            var upDir = cam.transform.TransformDirection(Vector3.up);
            var downDir = cam.transform.TransformDirection(Vector3.down);
            var leftDir = cam.transform.TransformDirection(Vector3.left);
            var rightDir = cam.transform.TransformDirection(Vector3.right);

            // Determine dir relative to camera space.
            up = Vector3.Dot(dir, upDir) > 0f;
            down = Vector3.Dot(dir, downDir) > 0f;
            left = Vector3.Dot(dir, leftDir) > 0f;
            right = Vector3.Dot(dir, rightDir) > 0f;
        }

        static List<int> s_ignoredCandidateIndices = new List<int>();

        protected bool isScreenSpaceOverlay()
        {
            if (Canvas.renderMode == RenderMode.ScreenSpaceOverlay)
                return true;

            if (Canvas.renderMode == RenderMode.ScreenSpaceCamera && Canvas.worldCamera == null)
                return true;

            return false;
        }

        /// <summary>
        /// Does a strict scan along the four sides of the rect meaning anything that is positioned diagonally will not be ignored.
        /// </summary>
        /// <param name="dir"></param>
        /// <param name="candidates"></param>
        /// <param name="drawGizmos"></param>
        /// <param name="wasOccluded">Out value that indicates that the search failed due to candidates being occluded.</param>
        /// <returns></returns>
        public Selectable FindSelectableImprovedAutomatic(Vector3 dir, IList<Selectable> candidates, bool drawGizmos, out bool wasOccluded)
        {
            wasOccluded = false;

            if (candidates == null)
                return null;

            var settings = GetSettings();
            var cam = Utils.GetCamera();

            if (!isScreenSpaceOverlay())
            {
                dir = cam.transform.TransformVector(dir); // from camera space to world space
            }
            dir = dir.normalized;

            var transform = Selectable.transform;
            var rectTransform = Selectable.transform as RectTransform;

            Vector3 thisCenter = rectTransform != null ? transform.TransformPoint((Vector3)rectTransform.rect.center) : transform.position;
            thisCenter.z = transform.position.z;
            var thisBounds = getCameraAlignedBoundingBox(cam, rectTransform, transform.position);

            Vector3 localDir = Quaternion.Inverse(transform.rotation) * dir;
            Vector3 thisLocalPointOnRectEdge = GetPointOnRectEdge(transform as RectTransform, localDir);
            Vector3 thisWorldPosOnRectEdge = transform.TransformPoint(thisLocalPointOnRectEdge);

            // Directions of camera in world space
            var upDir = isScreenSpaceOverlay() ? Vector3.up : cam.transform.TransformDirection(Vector3.up);
            var downDir = isScreenSpaceOverlay() ? Vector3.down : cam.transform.TransformDirection(Vector3.down);
            var leftDir = isScreenSpaceOverlay() ? Vector3.left : cam.transform.TransformDirection(Vector3.left);
            var rightDir = isScreenSpaceOverlay() ? Vector3.right : cam.transform.TransformDirection(Vector3.right);

            // Determine dir relative to this in camera space.
            bool up = Vector3.Dot(dir, upDir) > 0f;
            bool down = Vector3.Dot(dir, downDir) > 0f;
            bool left = Vector3.Dot(dir, leftDir) > 0f;
            bool right = Vector3.Dot(dir, rightDir) > 0f;

            // Calculate scan bounds
            var rectParams = getResolvedImprovedAutomaticParameters(up, down, left, right);
            var scanDistance = rectParams.ScanDistance;
            var scanMargin = rectParams.ScanMargin;
            Bounds scanBounds = new Bounds(thisBounds.center, thisBounds.size);
            if (up)
            {
                // First modify min then max, order is important here!
                // Do not overlap self
                scanBounds.min = new Vector3(scanBounds.min.x - scanMargin, scanBounds.max.y, scanBounds.min.z);
                // Extend outwards
                scanBounds.max = new Vector3(scanBounds.max.x + scanMargin, scanBounds.max.y + scanDistance, scanBounds.max.z);
            }
            else if (down)
            {
                // First modify max then min, order is inportant here!
                scanBounds.max = new Vector3(scanBounds.max.x + scanMargin, scanBounds.min.y, scanBounds.max.z);
                scanBounds.min = new Vector3(scanBounds.min.x - scanMargin, scanBounds.min.y - scanDistance, scanBounds.min.z);
            }
            else if (left)
            {
                // First modify max then min, order is inportant here!
                scanBounds.max = new Vector3(scanBounds.min.x, scanBounds.max.y + scanMargin, scanBounds.max.z);
                scanBounds.min = new Vector3(scanBounds.min.x - scanDistance, scanBounds.min.y - scanMargin, scanBounds.min.z);
            }
            else if (right)
            {
                // First modify min then max, order is inportant here!
                scanBounds.min = new Vector3(scanBounds.max.x, scanBounds.min.y - scanMargin, scanBounds.min.z);
                scanBounds.max = new Vector3(scanBounds.max.x + scanDistance, scanBounds.max.y + scanMargin, scanBounds.max.z);
            }

            // Add depth to scanBounds (for world canvases)
            float depth = scanBounds.size.magnitude;
            scanBounds.min = new Vector3(scanBounds.min.x, scanBounds.min.y, scanBounds.min.z - depth);
            scanBounds.max = new Vector3(scanBounds.max.x, scanBounds.max.y, scanBounds.max.z + depth);

            // We use indices here instead of Selectable because the profiler showed that
            // the equality compare of selectables is quite slow.
            s_ignoredCandidateIndices.Clear();

            Selectable bestPick = null;
            int bestPickIndex = 0;
            int candidatesCount = candidates.Count;

            // We do run the "find the best for loop" multiple times until be have either exhausted the
            // candidates or we have found one that is not occlusded. Why not check for occlusion within
            // shouldSkip()? If we do the graphics raycast in shouldSkip then it is called on every selectable.
            // That's a lot of raycasts (and they are slow). By doing the occlusion check only on the valid candidates
            // we can get away with doing much less raycasts (ideally just one per direction). Sadly this makes to code
            // a bit messy (while loop).
            int attempts = candidatesCount;
            bool occlusionHappened = true;
            bool endedWithOcclusion = false;
            while (occlusionHappened && attempts > 0 && bestPick == null)
            {
                occlusionHappened = false;
                attempts--;
                float minDistance = float.MaxValue;
                for (int i = 0; i < candidatesCount; i++)
                {
                    Selectable sel = candidates[i];

                    // Scene validation check primarily for Editor (sometimes the object lingers). TODO: investigate.
                    if (sel == null || sel.gameObject == null || !sel.gameObject.scene.IsValid())
                        continue;

                    if (s_ignoredCandidateIndices.Contains(i))
                        continue;

                    if (shouldSkip(cam, sel, dir, thisWorldPosOnRectEdge, out _))
                    {
                        s_ignoredCandidateIndices.Add(i);
                        continue;
                    }

                    if (sel.gameObject.TryGetComponent<UGUINavigationWizard>(out var wizard))
                    {
                        if (wizard.BlockIncomingAutoNavigation)
                            continue;
                    }

                    var selRect = sel.transform as RectTransform;
                    Vector3 selCenter = selRect != null ? sel.transform.TransformPoint((Vector3)selRect.rect.center) : sel.transform.position;
                    selCenter.z = sel.transform.position.z;
                    var selBounds = getCameraAlignedBoundingBox(cam, selRect, selCenter);
                    if (selBounds.Intersects(scanBounds))
                    {
                        var camPosOnRectEdge = isScreenSpaceOverlay() ? thisWorldPosOnRectEdge : cam.transform.InverseTransformPoint(thisWorldPosOnRectEdge);
                        var point = selBounds.ClosestPoint(camPosOnRectEdge);
                        var pointInWorldSpace = isScreenSpaceOverlay() ? point : cam.transform.TransformPoint(point);
                        Vector3 dirToSel = (pointInWorldSpace - thisWorldPosOnRectEdge).normalized;

                        // Project onto direction (if <= 0 then sel is behind the currently selected object).
                        float dot = Vector3.Dot(dir, dirToSel);

                        // Debug draw closest points
                        if (drawGizmos)
                        {
                            var col = Gizmos.color;
                            // Gizmos.matrix = cam.transform.localToWorldMatrix;
                            Gizmos.color = Color.blue;
                            Gizmos.DrawSphere(thisWorldPosOnRectEdge, 2.0f);
                            Gizmos.color = Color.blue;
                            Gizmos.DrawSphere(pointInWorldSpace, 2.0f);
                            Gizmos.color = col;
                        }

                        // Calc distance by touch points, not by center distance (like Unity does).
                        // Weight by angle by artificially increasing the distance for big angle differences.
                        float scaleFactor = 1f + (1f - dot) * rectParams.AngleSensitivity;
                        // Debug.Log(sel.name + ", dot: " + dot + ", scale: " + scaleFactor);

                        float distance = (pointInWorldSpace - thisWorldPosOnRectEdge).sqrMagnitude * scaleFactor;

                        if (distance < minDistance)
                        {
                            minDistance = distance;
                            bestPick = sel;
                            bestPickIndex = i;
                        }
                    }
                }

                // Occlusion check 
                if (settings != null && settings.DefaultOcclusion && bestPick != null)
                {
                    if (!skipOcclusionCheckDueToConstraints(bestPick))
                    {
                        bestPick.TryGetComponent<UGUINavigationWizard>(out var bestPickWizard);
                        if (bestPickWizard == null || !bestPickWizard.IgnoreOcclusion)
                        {
                            s_ignoredCandidateIndices.Add(bestPickIndex);
                            bool allowOutOfBounds = bestPickWizard != null && bestPickWizard.IgnoreOutOfScreenOcclusion;
                            if (!GraphicOcclusionChecker.IsCompletelyVisible(bestPick.transform as RectTransform, allowOutOfBounds))
                            {
                                bestPick = null;
                                occlusionHappened = true;
                                endedWithOcclusion = true;
                            }
                        }
                    }
                }

                if (bestPick != null || settings == null || !settings.DefaultOcclusion)
                {
                    endedWithOcclusion = false;
                }
            }

            wasOccluded = endedWithOcclusion;

#if UNITY_EDITOR
            if (drawGizmos)
            {
                // draw gizmos
                var col = Gizmos.color;
                if (!isScreenSpaceOverlay())
                    Gizmos.matrix = cam.transform.localToWorldMatrix;
                Gizmos.color = new Color(0f, 1f, 1f, 0.2f);
                Gizmos.DrawCube(scanBounds.center, scanBounds.size);
                Gizmos.color = col;
            }
#endif

            clearAllSelectables();

            return bestPick;
        }

        /// <summary>
        /// Mirrors the Unity native auto navigation behaviour.
        /// </summary>
        /// <param name="dir">Direction in camera space.</param>
        /// <param name="strategy"></param>
        /// <returns></returns>
        public Selectable FindSelectableUnityNative(Vector3 dir, bool drawGizmos = false)
        {
            if (Camera.current == null)
                return null;

            var cam = Camera.current;
            dir = cam.transform.TransformVector(dir); // from camera space to world space
            dir = dir.normalized;

            var transform = Selectable.transform;
            var rectTransform = Selectable.transform as RectTransform;

            // Vars for Unity native selection.
            Vector3 localDir = Quaternion.Inverse(transform.rotation) * dir;
            Vector3 thisPosOnRectEdge = transform.TransformPoint(GetPointOnRectEdge(transform as RectTransform, localDir));
            float maxScore = Mathf.NegativeInfinity;

            Selectable bestPick = null;
            updateAllSelectablesList();
            for (int i = 0; i < Selectable.allSelectableCount; i++)
            {
                Selectable sel = s_AllSelectables[i];

                if (shouldSkip(cam, sel, dir, thisPosOnRectEdge, out float dot))
                    continue;

                var selRect = sel.transform as RectTransform;
                Vector3 selCenter = selRect != null ? (Vector3)selRect.rect.center : Vector3.zero;
                Vector3 thisToSel = (sel.transform.TransformPoint(selCenter) - thisPosOnRectEdge).normalized;
                // Weight by angle.
                float score = dot / thisToSel.sqrMagnitude;

                if (score > maxScore)
                {
                    maxScore = score;
                    bestPick = sel;
                }
            }

            clearAllSelectables();

            return bestPick;
        }

        private bool shouldSkip(Camera cam, Selectable sel, Vector3 dir, Vector3 posOnRectEdge, out float dot)
        {
            if (sel == Selectable || sel == null)
            {
                dot = 0f;
                return true;
            }

            if (!sel.IsInteractable() || sel.navigation.mode == Navigation.Mode.None)
            {
                dot = 0f;
                return true;
            }

#if UNITY_EDITOR
            // In the editor skip selectables that are not rendered.
            if (cam != null && !UnityEditor.SceneManagement.StageUtility.IsGameObjectRenderedByCamera(sel.gameObject, cam))
            {
                dot = 0f;
                return true;
            }
#endif

            var selRect = sel.transform as RectTransform;
            Vector3 selCenter = selRect != null ? (Vector3)selRect.rect.center : Vector3.zero;
            Vector3 thisToSel = (sel.transform.TransformPoint(selCenter) - posOnRectEdge).normalized;

            // Project onto direction (if <= 0 then sel is behind the currently selected object).
            dot = Vector3.Dot(dir, thisToSel);
            if (dot <= 0)
                return true;

            if (sel.gameObject.TryGetComponent<UGUINavigationWizard>(out var wizard))
            {
                if (wizard.BlockIncomingAutoNavigation)
                    return true;
            }

            var settings = GetSettings();
            if (settings != null && settings.EnableConstraints && !settings.IsAllowed(sel))
            {
                return true;
            }

            return false;
        }

        Bounds getCameraAlignedBoundingBox(Camera cam, RectTransform rectTransform, Vector3 worldPos)
        {
            var boundingBox = new Bounds();

            if (rectTransform == null)
            {
                boundingBox.center = isScreenSpaceOverlay() ? worldPos : cam.transform.InverseTransformPoint(worldPos);
                boundingBox.extents = new Vector3(0.01f, 0.01f, 0.01f);
                return boundingBox;
            }

            rectTransform.GetWorldCorners(s_rectFourCornersArray);
            if (isScreenSpaceOverlay())
            {
                boundingBox.center = s_rectFourCornersArray[0] + (s_rectFourCornersArray[2] - s_rectFourCornersArray[0]) * 0.5f;
            }
            else
            {
                boundingBox.center = cam.transform.InverseTransformPoint(s_rectFourCornersArray[0]) +
                                     (cam.transform.InverseTransformPoint(s_rectFourCornersArray[2]) -
                                       cam.transform.InverseTransformPoint(s_rectFourCornersArray[0])) * 0.5f;
            }
            for (int i = 0; i < 4; i++)
            {
                var localPos = isScreenSpaceOverlay() ? s_rectFourCornersArray[i] : cam.transform.InverseTransformPoint(s_rectFourCornersArray[i]);
                boundingBox.Encapsulate(localPos);
            }

            return boundingBox;
        }

        // Utils

        public static Vector3 GetPointOnRectEdge(RectTransform rect, Vector2 dir)
        {
            if (rect == null)
                return Vector3.zero;

            // Scale so either x or y is exactly 1.
            if (dir != Vector2.zero)
                dir /= Mathf.Max(Mathf.Abs(dir.x), Mathf.Abs(dir.y));

            dir = rect.rect.center + Vector2.Scale(rect.rect.size, dir * 0.5f);
            return dir;
        }
    }
}
