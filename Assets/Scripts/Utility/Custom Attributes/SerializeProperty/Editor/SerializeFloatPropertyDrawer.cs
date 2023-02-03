using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace TGOM.Utility
{
    [CustomPropertyDrawer(typeof(SerializeFloatPropertyAttribute))]
    public class SerializeFloatPropertyDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.FloatField(position, label, property.floatValue);
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return EditorGUIUtility.singleLineHeight;
        }
    }
}