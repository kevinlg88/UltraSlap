using UnityEngine;

namespace Kamgam.UGUINavigationWizard
{
    public static class Utils
    {
#if UNITY_EDITOR
        [UnityEditor.MenuItem("Tools/" + Installer.AssetName + "/Create Automator", priority = 0)]
#endif
        public static UGUINavigationWizardAutomator GetOrCreateAutomator()
        {
            if (UGUINavigationWizardAutomator.Instance != null && UGUINavigationWizardAutomator.Instance.gameObject != null)
            {
                return UGUINavigationWizardAutomator.Instance;
            }
            else
            {
                return GetOrCreateBehaviour<UGUINavigationWizardAutomator>("UGUINavigationWizardAutomator");
            }
        }

#if UNITY_EDITOR
        [UnityEditor.MenuItem("Tools/" + Installer.AssetName + "/Create Null Resolver", priority = 1)]
#endif
        public static UGUINavigationWizardNullResolver GetOrCreateNullResolver()
        {
            if (UGUINavigationWizardNullResolver.Instance != null && UGUINavigationWizardNullResolver.Instance.gameObject != null)
            {
                return UGUINavigationWizardNullResolver.Instance;
            }
            else
            {
                return GetOrCreateBehaviour<UGUINavigationWizardNullResolver>("UGUINavigationWizardNullResolver");
            }
        }

        public static T GetOrCreateBehaviour<T>(string prefabName) where T : MonoBehaviour
        {
#if UNITY_EDITOR
            // Try to find one in the scene
            var automator = GameObject.FindAnyObjectByType<T>(FindObjectsInactive.Include);
            if (automator != null)
            {
                UnityEditor.EditorGUIUtility.PingObject(automator);
                UnityEditor.Selection.objects = new GameObject[] { automator.gameObject };
                return automator;
            }

            var prefabGUIDs = UnityEditor.AssetDatabase.FindAssets(prefabName);
            var prefabPath = UnityEditor.AssetDatabase.GUIDToAssetPath(prefabGUIDs[0]);
            var prefab = UnityEditor.AssetDatabase.LoadAssetAtPath<GameObject>(prefabPath);
            var go = UnityEditor.PrefabUtility.InstantiatePrefab(prefab) as GameObject;
            UnityEditor.EditorGUIUtility.PingObject(go);
            UnityEditor.Selection.objects = new GameObject[] { go };
            UnityEditor.EditorUtility.SetDirty(go);
#else
            var go = new GameObject(typeof(T).Name, typeof(T));
#endif
            return go.GetComponent<T>();
        }

        static Camera[] s_TmpCameras = new Camera[5];

        public static Camera GetCamera()
        {
            if (Camera.main != null)
            {
                return Camera.main;
            }

            if (Camera.allCamerasCount > s_TmpCameras.Length)
                s_TmpCameras = new Camera[Camera.allCamerasCount];

            Camera.GetAllCameras(s_TmpCameras);

            for (int i = 0; i < Camera.allCamerasCount; i++)
            {
                if (s_TmpCameras[i] == null || !s_TmpCameras[i].isActiveAndEnabled)
                    continue;

                if (s_TmpCameras[i].cameraType == CameraType.Game)
                {
                    return s_TmpCameras[i];
                }
            }

            return Camera.current;
        }

        public static Vector2 RectPosToViewportPos(RectTransform rectTransform, Vector2 defaultValue)
        {
            if (rectTransform == null)
                return defaultValue;

            var worldPos = rectTransform.localToWorldMatrix.MultiplyPoint(rectTransform.rect.center);
            var canvas = rectTransform.GetComponentInParent<Canvas>();
            if (canvas != null)
            {
                var cam = canvas.renderMode == RenderMode.ScreenSpaceOverlay ? null : canvas.worldCamera;
                if (cam == null)
                {
                    var screenPoint = new Vector2(worldPos.x, worldPos.y);
                    cam = Utils.GetCamera();
                    var viewportPos = new Vector3(
                        screenPoint.x / cam.pixelWidth,
                        screenPoint.y / cam.pixelHeight
                    );
                    return viewportPos;
                }
                else
                {
                    return (Vector2) cam.WorldToViewportPoint(worldPos);
                }
            }
            else
            {
                return defaultValue;
            }
        }
    }
}
