using UnityEngine;
using UnityEditor;

namespace TGOM.Utility
{
    [CustomPropertyDrawer(typeof(HelpBoxAttribute))]
    public class HelpBoxAttributeDrawer : DecoratorDrawer
    {
        public override float GetHeight()
        {
            HelpBoxAttribute helpBoxAttribute = attribute as HelpBoxAttribute;
            if (helpBoxAttribute == null) return base.GetHeight();

            int fontSize = EditorStyles.helpBox.fontSize;
            
            if(helpBoxAttribute.fontSize > 1)
                EditorStyles.helpBox.fontSize = helpBoxAttribute.fontSize;

            // TODO: This Size Calculation sometimes cuts away the last column of the Text. Find a workaround and apply it here
            float height = Mathf.Max(40f, EditorStyles.helpBox.CalcHeight(new GUIContent(helpBoxAttribute.text), EditorGUIUtility.currentViewWidth) + 4);

            EditorStyles.helpBox.fontSize = fontSize;

            return height;
        }

        public override void OnGUI(Rect position)
        {
            HelpBoxAttribute helpBoxAttribute = attribute as HelpBoxAttribute;
            if (helpBoxAttribute == null) return;

            int fontSize = EditorStyles.helpBox.fontSize;

            if(helpBoxAttribute.fontSize > 1)
                EditorStyles.helpBox.fontSize = helpBoxAttribute.fontSize;

            EditorGUI.HelpBox(position, helpBoxAttribute.text, GetMessageType(helpBoxAttribute.messageType));

            EditorStyles.helpBox.fontSize = fontSize;
        }

        private MessageType GetMessageType(HelpBoxMessageType helpBoxMessageType)
        {
            switch (helpBoxMessageType)
            {
                default:
                case HelpBoxMessageType.None: return MessageType.None;
                case HelpBoxMessageType.Info: return MessageType.Info;
                case HelpBoxMessageType.Warning: return MessageType.Warning;
                case HelpBoxMessageType.Error: return MessageType.Error;
            }
        }
    }
}