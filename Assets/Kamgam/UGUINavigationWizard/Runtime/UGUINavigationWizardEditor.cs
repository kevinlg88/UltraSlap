#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.UIElements;
using Kamgam.UGUINavigationWizard.EditorHelpers;
using UnityEngine;
using UnityEngine.UIElements;
using System;
using UnityEngine.UI;

namespace Kamgam.UGUINavigationWizard
{
    [CustomEditor(typeof(UGUINavigationWizard), true)]
    [CanEditMultipleObjects]
    public class UGUINavigationWizardEditor : UnityEditor.Editor
    {
        [System.NonSerialized]
        public static int NumberOfElemtUpdatesPerFrame = 10;
        static int s_elementUpdateIndex = 0;

        public StyleSheet m_StyleSheet;

        const string StyleSheedName = "UGUINavigationWizardEditorStyles";
        public StyleSheet GetStyleSheet()
        {
            if(m_StyleSheet == null)
            {
                var guids = AssetDatabase.FindAssets("t:StyleSheet " + StyleSheedName);
                if (guids.Length > 0)
                {
                    var path = AssetDatabase.GUIDToAssetPath(guids[0]);
                    m_StyleSheet = AssetDatabase.LoadAssetAtPath<StyleSheet>(path);
                }
            }

            return m_StyleSheet;
        }

        protected static Selectable[] s_AllSelectables = new Selectable[30];
        protected static Vector3[] s_rectFourCornersArray = new Vector3[4];

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

        UGUINavigationWizard wizard;

        UGUINavigationWizardSettings settings;
        SerializedObject settingsObj;
        SerializedProperty settingsAutomaticParamsProp;
        SerializedProperty settingsStrategiesProp;
        SerializedProperty settingsOcclusionProp;
        SerializedProperty settingsOutOfScreenOcclusionProp;
        SerializedProperty settingsSelectNullIfOccludedProp;
        SerializedProperty settingsUseOnTouchDevicesProp;
        SerializedProperty settingsEnableConstraintsProp;
        SerializedProperty settingsConstraintsIgnoreOcclusionProp;
        SerializedProperty settingsDebugVisualizationProp;
        SerializedProperty CustomizeAutomaticNavigationUpProp;
        SerializedProperty CustomizeAutomaticNavigationDownProp;
        SerializedProperty CustomizeAutomaticNavigationLeftProp;
        SerializedProperty CustomizeAutomaticNavigationRightProp;
        SerializedProperty upBlockedProp;
        SerializedProperty downBlockedProp;
        SerializedProperty leftBlockedProp;
        SerializedProperty rightBlockedProp;

        VisualElement root;

        public void OnEnable()
        {
            wizard = target as UGUINavigationWizard;

            settings = wizard.GetSettings();
            settingsObj = new SerializedObject(settings);
            if (settingsObj != null)
            {
                settingsAutomaticParamsProp = settingsObj.FindProperty("DefaultAutomaticParameters");
                settingsStrategiesProp = settingsObj.FindProperty("DefaultStrategies");
                settingsOcclusionProp = settingsObj.FindProperty("DefaultOcclusion");
                settingsOutOfScreenOcclusionProp = settingsObj.FindProperty("DefaultOutOfScreenOcclusion");
                settingsSelectNullIfOccludedProp = settingsObj.FindProperty("DefaultSelectNullIfOccluded");
                settingsUseOnTouchDevicesProp = settingsObj.FindProperty("UseOnTouchDevices");
                settingsEnableConstraintsProp = settingsObj.FindProperty("EnableConstraints");
                settingsConstraintsIgnoreOcclusionProp = settingsObj.FindProperty("ConstraintsIgnoreOcclusion");
                settingsDebugVisualizationProp = settingsObj.FindProperty("DebugVisualization");
            }

            CustomizeAutomaticNavigationUpProp = serializedObject.FindProperty("CustomizeAutomaticNavigationUp");
            CustomizeAutomaticNavigationDownProp = serializedObject.FindProperty("CustomizeAutomaticNavigationDown");
            CustomizeAutomaticNavigationLeftProp = serializedObject.FindProperty("CustomizeAutomaticNavigationLeft");
            CustomizeAutomaticNavigationRightProp = serializedObject.FindProperty("CustomizeAutomaticNavigationRight");

            EditorApplication.update += onEditorUpdate;

            wizard.Apply();
        }

        public void OnDisable()
        {
            EditorApplication.update -= onEditorUpdate;
        }

        private void onEditorUpdate()
        {
            if (IsVisualizing())
            {
                wizard.Apply();
                ApplyToNextChunk();
            }
        }

        public static bool IsVisualizing()
        {
            // Key is from SelectableEditor.s_ShowNavigationKey, see:
            // https://github.com/Unity-Technologies/uGUI/blob/5ab4c0fee7cd5b3267672d877ec4051da525913c/UnityEditor.UI/UI/SelectableEditor.cs#L34
            return EditorPrefs.GetBool("SelectableEditor.ShowNavigation", false);
        }

        public override VisualElement CreateInspectorGUI()
        {
            root = new VisualElement();
            if (GetStyleSheet() != null)
                root.styleSheets.Add(GetStyleSheet());
            root.TrackSerializedObjectValue(serializedObject, onValueChanged);

            if(wizard.hideFlags == HideFlags.DontSave)
            {
                var label = new Label("This is a temporary Wizard added by the Automator. Any changes you make here will NOT persist (except for global settings)!")
                    .Bold().Red().Wrap().Padding(10).Margin(15f, 0f).Background(Color.red * 0.3f, 5);
                root.Insert(0, label);
            }

            var infoFoldout = new Foldout { text = "Info", value = false };
            infoFoldout.Add(new Label("Notice that if the Navigation Wizard is enabled then it will take charge of the 'Navigation' section of your selectable (button, slider, ..) and force it to 'explicit'.") { style = { whiteSpace = WhiteSpace.Normal } });
            root.Add(infoFoldout);

            // Default inspector
            InspectorElement.FillDefaultInspector(root, this.serializedObject, this);

            // Remove script (since it's not clickable in UI Toolkit)
            root.PlaceAfterProperty("m_Script", infoFoldout);
            root.RemovePropertyField("m_Script");

            // Global Settings
            var header = root.AddHeader("m_Settings", "Global Settings");
            header.tooltip = "Here you can edit the global settings directly from the current object.";
            var settingsFoldout = new Foldout { text = "Edit Global Settings", value = false };
            var settingsParamsElement = UIToolkitExtensions.CreatePropertyField(settingsObj, settingsAutomaticParamsProp);
            var settingsStrategiesElement = UIToolkitExtensions.CreatePropertyField(settingsObj, settingsStrategiesProp);
            var settingsOcclusionElement = UIToolkitExtensions.CreatePropertyField(settingsObj, settingsOcclusionProp);
            var settingsOutOfScreenOcclusionElement = UIToolkitExtensions.CreatePropertyField(settingsObj, settingsOutOfScreenOcclusionProp);
            var settingsSelectNullIfOccludedElement = UIToolkitExtensions.CreatePropertyField(settingsObj, settingsSelectNullIfOccludedProp);
            var settingsUseOnTouchDevicesElement = UIToolkitExtensions.CreatePropertyField(settingsObj, settingsUseOnTouchDevicesProp);
            var settingsEnableConstraintsElement = UIToolkitExtensions.CreatePropertyField(settingsObj, settingsEnableConstraintsProp);
            var settingsConstraintsIgnoreOcclusionElement = UIToolkitExtensions.CreatePropertyField(settingsObj, settingsConstraintsIgnoreOcclusionProp);
            var settingsDebugVisElement = UIToolkitExtensions.CreatePropertyField(settingsObj, settingsDebugVisualizationProp);
            settingsFoldout.Add(settingsParamsElement);
            settingsFoldout.Add(settingsStrategiesElement);
            settingsFoldout.Add(settingsOcclusionElement);
            settingsFoldout.Add(settingsOutOfScreenOcclusionElement);
            settingsFoldout.Add(settingsSelectNullIfOccludedElement);
            settingsFoldout.Add(settingsUseOnTouchDevicesElement);
            settingsOcclusionElement.tooltip = "If enabled then the navigation will check if the object is occluded or not.\n" +
                                               "INFO: Occlusiong is checked at the center of the selectable rect. If the center can be clicked then the whole object is considered selectable.";
            settingsOutOfScreenOcclusionElement.tooltip = "If enabled then objects outside the screen will not be selectable.\n" +
                                                "NOTICE: Disabling this will make any object outside the screen selectable. There is no normal occlusion detection performed outside the screen. Disable with care (useful for scroll views).";
            settingsFoldout.Add(settingsEnableConstraintsElement);
            settingsFoldout.AddHeader("EnableConstraints", "Constraints");
            settingsEnableConstraintsElement.tooltip = "If enabled then the 'AllowedObjects' list will be the only objects selectable.";
            settingsFoldout.Add(settingsConstraintsIgnoreOcclusionElement);
            settingsConstraintsIgnoreOcclusionElement.tooltip = "If enabled then the 'AllowedObjects' will never be occluded (they are always selectable). Useful for tutorial overlays.";
            settingsFoldout.Add(settingsDebugVisElement);
            settingsDebugVisElement.style.marginTop = 10;
            settingsFoldout.TrackSerializedObjectValue(settingsObj, onSettingsValueChanged);
            root.InsertAfterProperty(settingsFoldout, "m_Settings");

            // Object settings
            root.AddHeader("SelectNullIfOccluded", "Object Settings");

            // Blocking fields
            root.AddHeader("UpBlocked", "Block");
            root.SetPropertyTooltip("UpBlocked", "If enabled then the up navigation will be blocked completely.");
            root.SetPropertyTooltip("DownBlocked", "If enabled then the down navigation will be blocked completely.");
            root.SetPropertyTooltip("LeftBlocked", "If enabled then the left navigation will be blocked completely.");
            root.SetPropertyTooltip("RightBlocked", "If enabled then the right navigation will be blocked completely.");

            // Overrides
            root.AddHeader("UpOverrides", "Overrides");
            root.SetPropertyTooltip("UpOverrides",
                "Use overrides to specify selectable objects that will use preferred to automatic selection.\n" +
                "First elements will take precedence. Inactive elements will be ignored.");
            root.SetPropertyTooltip("UpOverridesAutoNav", "If enabled then the overrides are not ordered by the list but by the improved auto navigation (closer ones are prioritized).");
            root.SetPropertyMarginTop("DownOverrides", 10);
            root.SetPropertyMarginTop("LeftOverrides", 10);
            root.SetPropertyMarginTop("RightOverrides", 10);

            // Auto Navigation
            var autoNavigationHeader = root.AddHeader("UseAutoNavigation", "Auto Navigation");
            autoNavigationHeader.tooltip = "Based on the default strategies in the global settings the navigation will use automatic " +
                "selectable detection strategies as a fallback if the overrides did not return a valid navigation target " +
                "(i.e. all of them are inactive).\n\n" +
                "Here you can configure (or completely disable) the auto navigation fallback.";
            root.SetPropertyTooltip("UseAutoNavigation", "If disabled the no improved auto navigation will be used as fallback for overrides.");

            var settingsHeader = root.AddHeader("CustomizeAutomaticNavigationUp", "Auto Nav Settings", bold: false);
            settingsHeader.tooltip = "These are the per object settings. Use them to customize this objects' navigation behaviour.";
            root.SetPropertyTooltip("UseUnityNative", "If disabled the Unitys default navigation is not used as fallback for overrides.");

            var customizeAutomaticNavigationUpField = root.FindPropertyField("CustomizeAutomaticNavigationUp");
            customizeAutomaticNavigationUpField.TrackPropertyValue(CustomizeAutomaticNavigationUpProp, onCustomizeAutomaticNavigationUpChanged);

            var customizeAutomaticNavigationDownField = root.FindPropertyField("CustomizeAutomaticNavigationDown");
            customizeAutomaticNavigationDownField.TrackPropertyValue(CustomizeAutomaticNavigationDownProp, onCustomizeAutomaticNavigationDownChanged);

            var customizeAutomaticNavigationLeftField = root.FindPropertyField("CustomizeAutomaticNavigationLeft");
            customizeAutomaticNavigationLeftField.TrackPropertyValue(CustomizeAutomaticNavigationLeftProp, onCustomizeAutomaticNavigationLeftChanged);

            var customizeAutomaticNavigationRightField = root.FindPropertyField("CustomizeAutomaticNavigationRight");
            customizeAutomaticNavigationRightField.TrackPropertyValue(CustomizeAutomaticNavigationRightProp, onCustomizeAutomaticNavigationRightChanged);

            root.SetPropertyTooltip(
                "Occlusion",
                "If enabled then no occlusion checks will be performed on this object.\n\n" +
                "Notice that this object itself still performs occlusion checks on OTHER objects whether this is on or off."
                );

            root.SetPropertyTooltip(
                "OutOfScreenOcclusion",
                "If enabled then objects outside the camera view frustum will not be selectable.\n\n" +
                "NOTICE: Disabling this will make any object outside the screen selectable. There is no normal occlusion detection performed outside the screen. Disable with care (useful for scroll views)."
                );

            // Unity Native navigation
            var unityNativeHeader = root.AddHeader("UseUnityNative", "Unity Native Navigation");
            unityNativeHeader.tooltip = "Based on the default strategies in the global settings the navigation will use Unitys native navigation " +
                " as a fallback if the overrides and automatic navigation did not return a valid navigation target " +
                "(i.e. all of them are inactive).\n\n" +
                "Here you can configure (or completely disable) the Unity native fallback.";

            updateGUIEnabled();
            onCustomizeAutomaticNavigationUpChanged(CustomizeAutomaticNavigationUpProp);
            onCustomizeAutomaticNavigationDownChanged(CustomizeAutomaticNavigationDownProp);
            onCustomizeAutomaticNavigationLeftChanged(CustomizeAutomaticNavigationLeftProp);
            onCustomizeAutomaticNavigationRightChanged(CustomizeAutomaticNavigationRightProp);

            return root;
        }

        private void refreshInEditor()
        {
            // Ensure continuous Update calls.
            if (!Application.isPlaying)
            {
                UnityEditor.EditorApplication.QueuePlayerLoopUpdate();
                UnityEditor.SceneView.RepaintAll();
            }

            Canvas.ForceUpdateCanvases();
        }

        private void onSettingsValueChanged(SerializedObject obj)
        {
            ApplyAll();
            refreshInEditor();
            updateGUIEnabled();
        }

        public static void ApplyAll()
        {
            updateAllSelectablesList();
            UGUINavigationWizard wizard;
            foreach (var sel in s_AllSelectables)
            {
                if (sel != null && sel.isActiveAndEnabled && sel.TryGetComponent<UGUINavigationWizard>(out wizard))
                {
                    wizard.Apply();
                }
            }
        }

        /// <summary>
        /// Updates only a part of all wizards. We use this to avoid choking the Editor FPS if
        /// hundreds of selectables are visible.
        /// </summary>
        public static void ApplyToNextChunk()
        {
            updateAllSelectablesList();

            if (s_elementUpdateIndex > s_AllSelectables.Length)
            {
                s_elementUpdateIndex = 0;
            }

            UGUINavigationWizard wizard;
            int limit = Mathf.Min(s_elementUpdateIndex + NumberOfElemtUpdatesPerFrame, s_AllSelectables.Length);
            for (int i = s_elementUpdateIndex; i < limit; i++)
            {
                var sel = s_AllSelectables[i];
                if (sel != null && sel.isActiveAndEnabled && sel.TryGetComponent<UGUINavigationWizard>(out wizard))
                {
                    wizard.Apply();
                }
            }

            s_elementUpdateIndex += NumberOfElemtUpdatesPerFrame;
        }

        private void onValueChanged(SerializedObject obj)
        {
            updateGUIEnabled();
            onCustomizeAutomaticNavigationUpChanged(CustomizeAutomaticNavigationUpProp);
        }

        private void onCustomizeAutomaticNavigationUpChanged(SerializedProperty useCustomSettings)
        {
            root.SetPropertyDisplay("AutomaticParametersUp", useCustomSettings.boolValue);
        }

        private void onCustomizeAutomaticNavigationDownChanged(SerializedProperty useCustomSettings)
        {
            root.SetPropertyDisplay("AutomaticParametersDown", useCustomSettings.boolValue);
        }

        private void onCustomizeAutomaticNavigationLeftChanged(SerializedProperty useCustomSettings)
        {
            root.SetPropertyDisplay("AutomaticParametersLeft", useCustomSettings.boolValue);
        }

        private void onCustomizeAutomaticNavigationRightChanged(SerializedProperty useCustomSettings)
        {
            root.SetPropertyDisplay("AutomaticParametersRight", useCustomSettings.boolValue);
        }

        private void updateGUIEnabled()
        {
            var settings = wizard.GetSettings();

            bool usesOverrides = settings != null && settings.DefaultStrategies.Contains(NavigationStrategy.Overrides);

            root.SetPropertyEnabled("UpOverrides", !wizard.UpBlocked && usesOverrides);
            root.SetPropertyEnabled("UpOverridesAutoNav", !wizard.UpBlocked && usesOverrides);

            root.SetPropertyEnabled("DownOverrides", !wizard.DownBlocked && usesOverrides);
            root.SetPropertyEnabled("DownOverridesAutoNav", !wizard.DownBlocked && usesOverrides);

            root.SetPropertyEnabled("LeftOverrides", !wizard.LeftBlocked && usesOverrides);
            root.SetPropertyEnabled("LeftOverridesAutoNav", !wizard.LeftBlocked && usesOverrides);

            root.SetPropertyEnabled("RightOverrides", !wizard.RightBlocked && usesOverrides);
            root.SetPropertyEnabled("RightOverridesAutoNav", !wizard.RightBlocked && usesOverrides);

            bool usesUnityNative = settings != null && settings.DefaultStrategies.Contains(NavigationStrategy.UnityNative);
            root.SetPropertyEnabled("UseUnityNative", usesUnityNative);

            bool usesAutomatic = settings != null && settings.DefaultStrategies.Contains(NavigationStrategy.Automatic);
            root.SetPropertyEnabled("UseAutoNavigation", usesAutomatic);
            root.SetPropertyEnabled("CustomizeAutomaticNavigationUp", !wizard.UpBlocked && usesAutomatic && wizard.UseAutoNavigation);
            root.SetPropertyEnabled("CustomizeAutomaticNavigationDown", !wizard.DownBlocked && usesAutomatic && wizard.UseAutoNavigation);
            root.SetPropertyEnabled("CustomizeAutomaticNavigationLeft", !wizard.LeftBlocked && usesAutomatic && wizard.UseAutoNavigation);
            root.SetPropertyEnabled("CustomizeAutomaticNavigationRight", !wizard.RightBlocked && usesAutomatic && wizard.UseAutoNavigation);
            root.SetPropertyEnabled("AutomaticParametersUp", !wizard.UpBlocked && usesAutomatic && wizard.UseAutoNavigation);
            root.SetPropertyEnabled("AutomaticParametersDown", !wizard.DownBlocked && usesAutomatic && wizard.UseAutoNavigation);
            root.SetPropertyEnabled("AutomaticParametersLeft", !wizard.LeftBlocked && usesAutomatic && wizard.UseAutoNavigation);
            root.SetPropertyEnabled("AutomaticParametersRight", !wizard.RightBlocked && usesAutomatic && wizard.UseAutoNavigation);
            root.SetPropertyEnabled("Occlusion", usesAutomatic && wizard.UseAutoNavigation);
            root.SetPropertyEnabled("OutOfScreenOcclusion", usesAutomatic && wizard.UseAutoNavigation);
        }
    }
}
#endif