#if UNITY_EDITOR
using UnityEditor;
#endif
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Kamgam.UGUINavigationWizard
{
    // Create a new type of Settings Asset.
    public class UGUINavigationWizardSettings : ScriptableObject
    {
        [SerializeField, Tooltip(_logLevelTooltip)]
        public Logger.LogLevel LogLevel;
        public const string _logLevelTooltip = "Any log above this log level will not be shown. To turn off all logs choose 'NoLogs'";

        [System.NonSerialized]
        public static List<NavigationStrategy> DefaultStrategiesTemplate = new List<NavigationStrategy>()
        {
            NavigationStrategy.Overrides,
            NavigationStrategy.Automatic,
            NavigationStrategy.UnityNative
        };

        public List<NavigationStrategy> DefaultStrategies = DefaultStrategiesTemplate;

        [System.NonSerialized]
        public static NavigationAutomaticStrategyParameters DefaultAutomaticParametersTemplate = new NavigationAutomaticStrategyParameters();
        public NavigationAutomaticStrategyParameters DefaultAutomaticParameters = DefaultAutomaticParametersTemplate;

        [Tooltip("If enabled then the navigation will check if the object is occluded or not.\n" +
            "INFO: Occlusiong is checked at the center of the selectable rect. If the center can be clicked then the whole object is considered selectable.")]
        public bool DefaultOcclusion = true;

        [Tooltip("If enabled then objects outside the screen will not be selectable.\n" +
            "NOTICE: Disabling this will make any object outside the screen selectable. There is no normal occlusion detection performed outside the screen. Disable with care (useful for scroll views).")]
        public bool DefaultOutOfScreenOcclusion = true;

        public bool DefaultSelectNullIfOccluded = true;

        public bool UseOnTouchDevices = true;

        [Header("Constraints")]
        [Tooltip("If enabled then the 'AllowedObjects' list will be the only objects selectable.")]
        public bool EnableConstraints = true;

        [Tooltip("If enabled then the 'AllowedObjects' will never be occluded (they are always selectable). Useful for tutorial overlays.")]
        public bool ConstraintsIgnoreOcclusion = false;

        /// <summary>
        /// The allowed objects to take into account if 'EnableConstraints' is true.
        /// </summary>
        [System.NonSerialized]
        public List<Selectable> AllowedObjects = new List<Selectable>();

        [Header("Debug")]
        public bool DebugVisualization = false;


        static bool _loggerBound = false;

#if UNITY_EDITOR
        [RuntimeInitializeOnLoadMethod]
        public static void BindLoggerLevelToSetting()
        {
            BindLoggerLevelToSetting(null);
        }
#endif

        public static void BindLoggerLevelToSetting(UGUINavigationWizardSettings settings)
        {
            if (_loggerBound)
                return;

#if UNITY_EDITOR
            if (settings == null)
            {
                settings = GetOrCreateSettings();
            }
#endif
            // Notice: This does not yet create a setting instance!
            Logger.OnGetLogLevel = () => settings.LogLevel;
            _loggerBound = true;
        }

        public static bool CheckIfIsAllowed(Selectable selectable, List<Selectable> allowedObjects)
        {
            // We interpret an empty list as alll elements being allowed.
            if (allowedObjects == null || allowedObjects.Count == 0)
                return true;

            return allowedObjects.Contains(selectable);
        }

        public bool IsAllowed(Selectable sel)
        {
            return CheckIfIsAllowed(sel, AllowedObjects);
        }

        public bool IsAllowed(Transform transform)
        {
            if (transform.TryGetComponent<Selectable>(out var sel))
            {
                return IsAllowed(sel);
            }
            else
            {
                return false;
            }
        }

#if UNITY_EDITOR
        public const string SettingsFilePath = "Assets/UGUINavigationWizardSettings.asset";

        [System.NonSerialized]
        static UGUINavigationWizardSettings cachedSettings;

        public static UGUINavigationWizardSettings GetOrCreateSettings()
        {
            if (cachedSettings == null)
            {
                cachedSettings = AssetDatabase.LoadAssetAtPath<UGUINavigationWizardSettings>(SettingsFilePath);

                // Still not found? Then search for it.
                if (cachedSettings == null)
                {
                    string typeName = typeof(UGUINavigationWizardSettings).Name;
                    string[] results = AssetDatabase.FindAssets("t:" + typeName);
                    if (results.Length > 0)
                    {
                        string path = AssetDatabase.GUIDToAssetPath(results[0]);
                        cachedSettings = AssetDatabase.LoadAssetAtPath<UGUINavigationWizardSettings>(path);
                    }
                }

                // Still not found? Then create settings.
                if (cachedSettings == null)
                {
                    cachedSettings = ScriptableObject.CreateInstance<UGUINavigationWizardSettings>();
                    cachedSettings.LogLevel = Logger.LogLevel.Warning;
                    cachedSettings.DefaultStrategies = DefaultStrategiesTemplate;
                    cachedSettings.DefaultAutomaticParameters = DefaultAutomaticParametersTemplate;
                    cachedSettings.DefaultSelectNullIfOccluded = true;
                    cachedSettings.UseOnTouchDevices = true;
                    cachedSettings.DefaultOcclusion = true;
                    cachedSettings.EnableConstraints = true;
                    cachedSettings.ConstraintsIgnoreOcclusion = false;
                    cachedSettings.AllowedObjects = new List<Selectable>();
                    cachedSettings.DebugVisualization = false;

                    AssetDatabase.CreateAsset(cachedSettings, SettingsFilePath);
                    AssetDatabase.SaveAssets();

                    Logger.OnGetLogLevel = () => cachedSettings.LogLevel;
                }
            }

            return cachedSettings;
        }

        internal static SerializedObject GetSerializedSettings()
        {
            return new SerializedObject(GetOrCreateSettings());
        }

        [MenuItem("Tools/UGUI Navigation Wizard/Settings", priority = 100)]
        public static void OpenSettings()
        {
            var settings = UGUINavigationWizardSettings.GetOrCreateSettings();
            if (settings != null)
            {
                Selection.activeObject = settings;
                EditorGUIUtility.PingObject(settings);
            }
            else
            {
                EditorUtility.DisplayDialog("Error", "UGUI Navigation Wizard Settings could not be found or created.", "Ok");
            }
        }
#endif
    }


#if UNITY_EDITOR
    [CustomEditor(typeof(UGUINavigationWizardSettings))]
    public class UGUINavigationWizardSettingsEditor : Editor
    {
        public UGUINavigationWizardSettings settings;

        public void OnEnable()
        {
            settings = target as UGUINavigationWizardSettings;
        }

        public override void OnInspectorGUI()
        {
            EditorGUILayout.LabelField("Version: " + Installer.Version);
            base.OnInspectorGUI();
        }
    }

    static class UGUINavigationWizardSettingsProvider
    {
        [SettingsProvider]
        public static UnityEditor.SettingsProvider CreateUGUINavigationWizardSettingsProvider()
        {
            var provider = new UnityEditor.SettingsProvider("Project/UGUI Navigation Wizard", SettingsScope.Project)
            {
                label = "UGUI Navigation Wizard",
                guiHandler = (searchContext) =>
                {
                    var settings = UGUINavigationWizardSettings.GetSerializedSettings();

                    var style = new GUIStyle(GUI.skin.label);
                    style.wordWrap = true;

                    EditorGUILayout.LabelField("Version: " + Installer.Version);
                    if (drawButton(" Open Manual ", icon: "_Help"))
                    {
                        Installer.OpenManual();
                    }

                    drawField("LogLevel", "Log Level", UGUINavigationWizardSettings._logLevelTooltip, settings, style);

                    settings.ApplyModifiedProperties();
                },

                // Populate the search keywords to enable smart search filtering and label highlighting.
                keywords = new System.Collections.Generic.HashSet<string>(new[] { "ui", "ugui", "navigation", "wizard" })
            };

            return provider;
        }

        static void drawField(string propertyName, string label, string tooltip, SerializedObject settings, GUIStyle style)
        {
            EditorGUILayout.PropertyField(settings.FindProperty(propertyName), new GUIContent(label));
            if (!string.IsNullOrEmpty(tooltip))
            {
                GUILayout.BeginVertical(EditorStyles.helpBox);
                GUILayout.Label(tooltip, style);
                GUILayout.EndVertical();
            }
            GUILayout.Space(10);
        }

        static bool drawButton(string text, string tooltip = null, string icon = null, params GUILayoutOption[] options)
        {
            GUIContent content;

            // icon
            if (!string.IsNullOrEmpty(icon))
                content = EditorGUIUtility.IconContent(icon);
            else
                content = new GUIContent();

            // text
            content.text = text;

            // tooltip
            if (!string.IsNullOrEmpty(tooltip))
                content.tooltip = tooltip;

            return GUILayout.Button(content, options);
        }
    }
#endif
}