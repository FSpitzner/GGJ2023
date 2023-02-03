using UnityEngine;

namespace TGOM.Utility
{
    public enum HelpBoxMessageType { None, Info, Warning, Error }

    public class HelpBoxAttribute : PropertyAttribute
    {
        public string text;
        public HelpBoxMessageType messageType;
        public int fontSize;

        public HelpBoxAttribute(string text, HelpBoxMessageType messageType = HelpBoxMessageType.None, int fontSize = -1)
        {
            this.text = text;
            this.messageType = messageType;
            this.fontSize = fontSize;
        }
    }
}