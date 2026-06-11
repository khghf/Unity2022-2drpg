#if UNITY_EDITOR
using System;
using UnityEditor;
using UnityEngine;
namespace GFW.Editor.Drawer
{
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false)]
    public class LabelAttribute : PropertyAttribute
    {
        public string Label;
        public LabelAttribute(string label) { Label=label; }
    }

    [CustomPropertyDrawer(typeof(LabelAttribute))]
    public class LabelDrawer:PropertyDrawer
    {
        LabelAttribute Attr=>attribute as LabelAttribute;
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);
           
            Rect labelRect = position;
            labelRect.height = EditorGUIUtility.singleLineHeight; 
            EditorGUI.LabelField(labelRect, Attr.Label, EditorStyles.boldLabel);
           
            position.y += EditorGUIUtility.singleLineHeight + 2; 
            label.text=property.displayName;
            EditorGUI.PropertyField(position, property, label, true);

            EditorGUI.EndProperty();
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return EditorGUI.GetPropertyHeight(property, label, true)+ EditorGUIUtility.singleLineHeight + 2;
        }
    }
}
#endif
