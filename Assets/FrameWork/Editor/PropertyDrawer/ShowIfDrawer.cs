#if UNITY_EDITOR
using System;
using System.Reflection;
using UnityEditor;
using UnityEngine;
namespace GFW.Editor.Drawer
{
    public enum CompareMode
    {
        Equal,
        Unequal
    }
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false)]
    public class ShowIfAttribute : PropertyAttribute
    {
        public string TargetFieldName;
        public object ValueForCompare;
        public CompareMode CompareMode = CompareMode.Equal;
        // 用于 bool
        public ShowIfAttribute(string fieldName)
        {
            TargetFieldName = fieldName;
        }
        // 用于枚举 == 值
        public ShowIfAttribute(string fieldName, object value, CompareMode compareMode = CompareMode.Equal)
        {
            TargetFieldName = fieldName;
            ValueForCompare = value;
            CompareMode=compareMode;
        }
    }



    [CustomPropertyDrawer(typeof(ShowIfAttribute))]
    public class ShowIfDrawer : PropertyDrawer
    {
        ShowIfAttribute Attr => attribute as ShowIfAttribute;
        FieldInfo FieldInfoForCompare = null;
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return ShouldShow(in property) ? EditorGUI.GetPropertyHeight(property, label, true) : 0;
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if (ShouldShow(in property)) EditorGUI.PropertyField(position, property, label, true);
        }

        bool ShouldShow(in SerializedProperty property)
        {
            if (FieldInfoForCompare==null)
            {
                var target = property.serializedObject.targetObject;
                var type = target.GetType();
                FieldInfoForCompare = type.GetField(Attr.TargetFieldName, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
            }

            if (FieldInfoForCompare==null)
            {
                Debug.LogWarning("[ShowIfDrawer]没有找到对应的字段/属性，请输入正确的字段或属性名");
                return true;
            }


            var targetObject = property.serializedObject.targetObject;
            var value = FieldInfoForCompare.GetValue(targetObject);
            if(Attr.ValueForCompare!=null)
            {
                bool isEqual=value.Equals(Attr.ValueForCompare);

                if(Attr.CompareMode==CompareMode.Equal)return isEqual;

                return !isEqual;
            }
            return (bool)value;
        }
    }
}

#endif

