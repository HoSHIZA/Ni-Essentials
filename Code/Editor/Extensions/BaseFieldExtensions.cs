using UnityEngine.UIElements;

namespace NiGames.Essentials.Editor
{
    public static class BaseFieldExtensions
    {
        public static void SetInspectorAligned<T>(this BaseField<T> field, bool aligned)
        {
            field.EnableInClassList(BaseField<T>.alignedFieldUssClassName, aligned);
        }
    }
}