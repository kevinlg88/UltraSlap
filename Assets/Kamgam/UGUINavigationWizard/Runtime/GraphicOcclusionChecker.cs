using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace Kamgam.UGUINavigationWizard
{
    public static class GraphicOcclusionChecker
    {
        static readonly List<RaycastResult> s_RaycastResults = new List<RaycastResult>(30);
        static PointerEventData s_PointerEventData;
        static EventSystem s_PointerEventDataEventSystem;

        /// <summary>
        /// Returns whether or not that rectTransform is clickable.
        /// </summary>
        /// <returns>Returns TRUE if in case of doubt or error.</returns>
        public static bool IsCompletelyVisible(RectTransform rectTransform, bool allowOutOfBounds = false, GraphicRaycaster graphicRaycaster = null, Canvas canvas = null)
        {
            if (rectTransform == null)
                return true;

            if (graphicRaycaster == null)
            {
                graphicRaycaster = rectTransform.GetComponentInParent<GraphicRaycaster>();
            }
            if (graphicRaycaster == null)
            {
#if UNITY_EDITOR
                Logger.LogError("Canvas does not have a GraphicsRaycaster component.");
#endif
                return true;
            }

            if (canvas == null)
            {
                graphicRaycaster.gameObject.TryGetComponent<Canvas>(out canvas);
            }

            if (rectTransform == null || canvas == null)
                return true;

            // Get center
            Vector3 center = rectTransform.TransformPoint(rectTransform.rect.center);

            var eventSystem = EventSystem.current;

            // Create a pointer event data object
            if (s_PointerEventData == null
                || s_PointerEventDataEventSystem == null
                || s_PointerEventDataEventSystem != eventSystem)
            {
                s_PointerEventDataEventSystem = eventSystem;
                s_PointerEventData = new PointerEventData(eventSystem);
            }
            
            // Set the pointer event data position
            s_PointerEventData.position = RectTransformUtility.WorldToScreenPoint(canvas.renderMode == RenderMode.ScreenSpaceOverlay ? null : canvas.worldCamera, center);

            var cam = Utils.GetCamera();
            if (cam == null)
                return false;

            // Check if outside of camera frustum.
            if (!allowOutOfBounds && (
                   s_PointerEventData.position.x < 0
                || s_PointerEventData.position.x > cam.pixelWidth
                || s_PointerEventData.position.y < 0
                || s_PointerEventData.position.y > cam.pixelHeight)
                )
            {
                return false;
            }

            // Raycast and check if the rectTransform is hit
            s_RaycastResults.Clear();

            // NOTICE: Raycasts will ALWAYS return an empty list if the target position is outside of the canvas.
            //         This needs to be taken into account if allowOutOfBounds is true.
            // BIG TODO !!!
            if (eventSystem != null)
                eventSystem.RaycastAll(s_PointerEventData, s_RaycastResults);
            else
                graphicRaycaster.Raycast(s_PointerEventData, s_RaycastResults);

            // Sometimes in edit mode the raycasts return nothing (the eventSystem is always null during edit mode).
            // Since the raycast is always made on the current selectable this is a faulty state (thus we abort with TRUE).
            if (s_RaycastResults.Count == 0)
                return true;

            bool hit = false;
            // If all elements before the selectable are children of the selectable then it is visible.
            // Otherwise it's occluded at the position of this raycast.
            foreach (var result in s_RaycastResults)
            {
                // Check if we have reached the selectable (or a child of it).
                if (result.gameObject.transform.IsChildOf(rectTransform))
                {
                    hit = true;
                    break;
                }

                // Check if we hit an element that is not a child of the selectable.
                // If yes, then this means the selectable is occluded and we abort with FALSE as a result.
                if (!result.gameObject.transform.IsChildOf(rectTransform))
                {
                    hit = false;
                    break;
                }
            }

            return hit;
        }
    }
}