using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace NiGames.Essentials.Editor
{
    public static class BindingExtensions
    {
        /// <summary>
        /// Sets the absolute <see cref="IBindable.bindingPath"/> using the relative of <see cref="SerializedProperty"/>.
        /// </summary>
        public static void BindPropertyRelative(this IBindable bindable, SerializedProperty property, string relativePath)
        {
            bindable.bindingPath = property.FindPropertyRelative(relativePath).propertyPath;
            
            // if (bindable is VisualElement ve)
            // {
            //     ve.Bind(property.serializedObject);
            // }
        }
    }
}