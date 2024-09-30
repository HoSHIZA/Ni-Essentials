using UnityEditor;
using UnityEngine;

namespace NiGames.Essentials.Editor
{
    [CustomPropertyDrawer(typeof(TitleAttribute))]
    public sealed class TitleDrawer : DecoratorDrawer
    {
        public const int SPACE_BEFORE_TITLE = 9;
        public const int SPACE_BEFORE_LINE = 2;
        public const int SPACE_AFTER_LINE = 2;
        
        public override void OnGUI(Rect position)
        {
            var attr = (TitleAttribute)attribute;
            
            var titleRect = new Rect(position)
            {
                y = position.y + SPACE_BEFORE_TITLE,
                height = EditorGUIUtility.singleLineHeight,
            };

            var lineRect = new Rect(position)
            {
                y = titleRect.yMax + SPACE_BEFORE_LINE,
                height = attr.LineHeight,
            };
            
            var style = new GUIStyle(EditorStyles.label)
            {
                richText = true,
                fontStyle = FontStyle.Bold,
            };
            
            var label = new GUIContent(attr.Header);
            
            var originalColor = GUI.color;
            ColorUtility.TryParseHtmlString(attr.LabelColor, out var newColor);
            newColor = newColor == Color.clear ? Color.white : newColor;
            
            GUI.color = newColor;
            GUI.Label(titleRect, label, style);
            GUI.color = originalColor;
            
            if (attr.LineHeight > 0)
            {
                ColorUtility.TryParseHtmlString(attr.LineColor, out var lineColor);
                
                lineColor = lineColor == Color.clear
                    ? EditorGUIUtility.isProSkin
                        ? new Color(.3f, .3f, .3f, 1)
                        : new Color(.7f, .7f, .7f, 1f)
                    : lineColor; 
                
                EditorGUI.DrawRect(lineRect, lineColor);
            }
            
        }
        
        public override float GetHeight()
        {
            var attr = (TitleAttribute)attribute;

            return SPACE_BEFORE_TITLE + EditorGUIUtility.singleLineHeight + (attr.LineHeight > 0
                ? SPACE_BEFORE_LINE + attr.LineHeight + SPACE_AFTER_LINE
                : SPACE_AFTER_LINE);
        }
    }
}