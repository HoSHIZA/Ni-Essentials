using UnityEngine.UIElements;

namespace NiGames.Essentials.Editor
{
    /// <summary>
    /// Duplicate class from runtime for editor. Necessary if the runtime assembly is not available.
    /// </summary>
    public static class StyleExtensions
    {
        public static void SetFlex(this IStyle style, StyleFloat grow, StyleFloat shrink)
        {
            style.flexGrow = grow;
            style.flexShrink = shrink;
        }
        
        public static void SetFlex(this IStyle style, StyleFloat grow, StyleFloat shrink, StyleLength basis)
        {
            style.flexGrow = grow;
            style.flexShrink = shrink;
            style.flexBasis = basis;
        }
        
        public static void SetFlex(this IStyle style, StyleFloat grow, StyleFloat shrink, StyleLength basis, FlexDirection direction)
        {
            style.flexGrow = grow;
            style.flexShrink = shrink;
            style.flexBasis = basis;
            style.flexDirection = direction;
        }
        
        public static void SetMargin(this IStyle style, StyleLength top, StyleLength right, StyleLength bottom, StyleLength left)
        {
            style.marginTop = top;
            style.marginRight = right;
            style.marginBottom = bottom;
            style.marginLeft = left;
        }
        
        public static void SetMargin(this IStyle style, StyleLength margin)
        {
            style.marginTop = margin;
            style.marginRight = margin;
            style.marginBottom = margin;
            style.marginLeft = margin;
        }
        
        public static void SetPadding(this IStyle style, StyleLength top, StyleLength right, StyleLength bottom, StyleLength left)
        {
            style.paddingTop = top;
            style.paddingRight = right;
            style.paddingBottom = bottom;
            style.paddingLeft = left;
        }
        
        public static void SetPadding(this IStyle style, StyleLength padding)
        {
            style.paddingTop = padding;
            style.paddingRight = padding;
            style.paddingBottom = padding;
            style.paddingLeft = padding;
        }

        public static void SetBorderWidth(this IStyle style, StyleFloat top, StyleFloat right, StyleFloat bottom, StyleFloat left)
        {
            style.borderTopWidth = top;
            style.borderRightWidth = right;
            style.borderBottomWidth = bottom;
            style.borderLeftWidth = left;
        }

        public static void SetBorderWidth(this IStyle style, StyleFloat width)
        {
            style.borderTopWidth = width;
            style.borderRightWidth = width;
            style.borderBottomWidth = width;
            style.borderLeftWidth = width;
        }
        
        public static void SetBorderRadius(this IStyle style, StyleLength topLeft, StyleLength topRight, StyleLength bottomRight, 
            StyleLength bottomLeft)
        {
            style.borderTopLeftRadius = topLeft;
            style.borderTopRightRadius = topRight;
            style.borderBottomRightRadius = bottomRight;
            style.borderBottomLeftRadius = bottomLeft;
        }
        
        public static void SetBorderRadius(this IStyle style, StyleLength radius)
        {
            style.borderTopLeftRadius = radius;
            style.borderTopRightRadius = radius;
            style.borderBottomRightRadius = radius;
            style.borderBottomLeftRadius = radius;
        }
    }
}