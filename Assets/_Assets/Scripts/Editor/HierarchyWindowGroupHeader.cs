using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

/// <summary>
/// Hierarchy Window Group Header
/// http://diegogiacomelli.com.br/unitytips-hierarchy-window-group-header
/// </summary>
[InitializeOnLoad]
public static class HierarchyWindowGroupHeader
{
    static HierarchyWindowGroupHeader()
    {
        EditorApplication.hierarchyWindowItemOnGUI += HierarchyWindowItemOnGUI;
    }

    static void HierarchyWindowItemOnGUI(int instanceID, Rect selectionRect)
    {
        var gameObject = EditorUtility.InstanceIDToObject(instanceID) as GameObject;

        if (gameObject != null && gameObject.name.StartsWith("---", System.StringComparison.Ordinal))
        {
            EditorGUI.DrawRect(selectionRect, new Color(0.1f, 0.1f, 0.1f));
            EditorGUI.DropShadowLabel(selectionRect, gameObject.name.Replace("-", "").ToUpperInvariant());
        }

        if (gameObject != null && gameObject.name.StartsWith("###", System.StringComparison.Ordinal))
        {
            EditorGUI.DrawRect(selectionRect, new Color(0.5f, 0f, 0f));
            EditorGUI.DropShadowLabel(selectionRect, gameObject.name.Replace("#", "").ToUpperInvariant());
        }
    }
}