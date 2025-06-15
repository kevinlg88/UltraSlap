#if UNITY_EDITOR
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace Kamgam.UGUINavigationWizard.EditorHelpers
{
    public static class UIToolkitExtensions
    {
        public static VisualElement CreatePropertyField(SerializedObject serializedObject, SerializedProperty serializedProperty, string label = null)
        {
            var prop = new PropertyField(serializedProperty, label);
            prop.Bind(serializedObject);
            return prop;
        }

        public static VisualElement AddPropertyField(this VisualElement container, SerializedObject serializedObject, SerializedProperty serializedProperty, string label = null)
        {
            var prop = new PropertyField(serializedProperty, label);
            prop.Bind(serializedObject);
            container.Add(prop);
            return prop;
        }

        public static TextElement AddHeader(this VisualElement container, string propertyPath, string text, float marginTop = 10, bool bold = true)
        {
            var label = new Label(text);
            if (bold)
                label = label.Bold();
            var propField = FindPropertyField(container, propertyPath);
            InsertBefore(label, propField);

            label.style.marginTop = marginTop;
            label.style.marginBottom = Mathf.RoundToInt(marginTop * 0.3f);
            return label;
        }

        public static void HideProperty(this VisualElement container, string propertyPath)
        {
            var propField = FindPropertyField(container, propertyPath);
            propField.style.display = DisplayStyle.None;
        }

        public static void ShowProperty(this VisualElement container, string propertyPath)
        {
            var propField = FindPropertyField(container, propertyPath);
            propField.style.display = DisplayStyle.Flex;
        }

        public static void SetPropertyDisplay(this VisualElement container, string propertyPath, bool display)
        {
            var propField = FindPropertyField(container, propertyPath);
            propField.style.display = display ? DisplayStyle.Flex : DisplayStyle.None;
        }

        public static void SetPropertyEnabled(this VisualElement container, string propertyPath, bool enabled)
        {
            var propField = FindPropertyField(container, propertyPath);
            propField.SetEnabled(enabled);
        }

        public static void TrackPropertyChange(this VisualElement container, SerializedObject serializedObject, string propertyPath, System.Action<SerializedProperty> callback = null)
        {
            var prop = serializedObject.FindProperty(propertyPath);
            var field = container.FindPropertyField(propertyPath);
            field.TrackPropertyValue(prop, callback);
        }

        public static void SetPropertyMarginTop(this VisualElement container, string propertyPath, float marginTop)
        {
            var propField = FindPropertyField(container, propertyPath);
            propField.style.marginTop = marginTop;
        }

        public static void SetPropertyTooltip(this VisualElement container, string propertyPath, string tooltip)
        {
            var propField = FindPropertyField(container, propertyPath);
            propField.tooltip = tooltip;
        }

        public static T Bold<T>(this T te) where T : TextElement
        {
            te.style.unityFontStyleAndWeight = FontStyle.Bold;
            return te;
        }

        public static T Color<T>(this T te, Color color) where T : TextElement
        {
            te.style.color = color;
            return te;
        }

        public static T Red<T>(this T te) where T : TextElement
        {
            te.style.color = UnityEngine.Color.red;
            return te;
        }

        public static T Green<T>(this T te) where T : TextElement
        {
            te.style.color = UnityEngine.Color.green;
            return te;
        }

        public static T Blue<T>(this T te) where T : TextElement
        {
            te.style.color = UnityEngine.Color.blue;
            return te;
        }

        public static T Wrap<T>(this T te) where T : TextElement
        {
            te.style.whiteSpace = WhiteSpace.Normal;
            return te;
        }

        public static T Padding<T>(this T te, float padding) where T : TextElement
        {
            te.style.paddingTop = padding;
            te.style.paddingRight = padding;
            te.style.paddingBottom = padding;
            te.style.paddingLeft = padding;
            return te;
        }

        public static T Margin<T>(this T te, float margin) where T : TextElement
        {
            te.style.marginTop = margin;
            te.style.marginRight = margin;
            te.style.marginBottom = margin;
            te.style.marginLeft = margin;
            return te;
        }

        public static T Margin<T>(this T te, float marginsVertical, float marginsHorizontal) where T : TextElement
        {
            te.style.marginTop = marginsVertical;
            te.style.marginRight = marginsHorizontal;
            te.style.marginBottom = marginsVertical;
            te.style.marginLeft = marginsHorizontal;
            return te;
        }

        public static T Background<T>(this T te, Color color, float roundCornerWidth = 0f) where T : TextElement
        {
            te.style.backgroundColor = color;
            if (roundCornerWidth != 0f)
            {
                te.style.borderTopRightRadius = roundCornerWidth;
                te.style.borderTopLeftRadius = roundCornerWidth;
                te.style.borderBottomLeftRadius = roundCornerWidth;
                te.style.borderBottomRightRadius = roundCornerWidth;
            }
            return te;
        }

        public static VisualElement FindPropertyField(this VisualElement ve, string propertyPath)
        {
            return ve.Query<PropertyField>().Where(v => v.bindingPath == propertyPath).First();   
        }

        public static void RemovePropertyField(this VisualElement ve, string propertyPath)
        {
            var field = ve.Query<PropertyField>().Where(v => v.bindingPath == propertyPath).First();
            if(field != null)
                ve.Remove(field);
        }

        public static void InsertBefore(VisualElement newElement, VisualElement anchorElement)
        {
            InsertBefore(anchorElement.parent, newElement, anchorElement);
        }

        public static void InsertBefore(this VisualElement container, VisualElement newElement, VisualElement anchorElement)
        {
            container.Insert(container.IndexOf(anchorElement), newElement);
        }

        public static void InsertAfter(VisualElement newElement, VisualElement anchorElement)
        {
            InsertAfter(anchorElement.parent, newElement, anchorElement);
        }

        public static void InsertAfter(this VisualElement container, VisualElement newElement, VisualElement anchorElement)
        {
            container.Insert(container.IndexOf(anchorElement) + 1, newElement);
        }

        public static VisualElement InsertBeforeProperty(this VisualElement container, VisualElement newElement, string anchorPropertyPath)
        {
            return InsertRelativeToProperty(container, newElement, anchorPropertyPath, 0);
        }

        public static VisualElement InsertAfterProperty(this VisualElement container, VisualElement newElement, string AnchorPropertyPath)
        {
            return InsertRelativeToProperty(container, newElement, AnchorPropertyPath, 1);
        }

        public static VisualElement InsertRelativeToProperty(this VisualElement container, VisualElement newElement, string anchorPropertyPath, int indexDelta)
        {
            var sibling = container.FindPropertyField(anchorPropertyPath);
            if (sibling == null)
                container.Add(newElement);
            else
                container.Insert(container.IndexOf(sibling) + indexDelta, newElement);

            return newElement;
        }

        public static VisualElement PlaceBeforeProperty(this VisualElement container, string propertyPath, VisualElement element)
        {
            return PlaceRelativeToProperty(container, propertyPath, element, -1);
        }

        public static VisualElement PlaceAfterProperty(this VisualElement container, string propertyPath, VisualElement element)
        {
            return PlaceRelativeToProperty(container, propertyPath, element, 0);
        }

        public static VisualElement PlaceRelativeToProperty(this VisualElement container, string propertyPath, VisualElement element, int indexDelta)
        {
            var sibling = container.FindPropertyField(propertyPath);
            if (sibling == null)
                container.Add(element);
            else
                container.Insert(container.IndexOf(sibling) + indexDelta, element);

            return element;
        }
    }
}
#endif
