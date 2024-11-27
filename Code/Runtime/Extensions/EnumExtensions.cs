using System;

namespace NiGames.Essentials
{
    public static class EnumExtensions
    {
        public static T SetFlag<T>(this T value, T flag) where T : struct, Enum
        {
            return (T)(object)((int)(object)value | (int)(object)flag);
        }
        
        public static T ClearFlag<T>(this T value, T flag) where T : struct, Enum
        {
            return (T)(object)((int)(object)value & ~(int)(object)flag);
        }
        
        public static T ToggleFlag<T>(this T value, T flag) where T : struct, Enum
        {
            return (T)(object)((int)(object)value ^ (int)(object)flag);
        }
        
        public static bool HasFlagFast<T>(this T value, T flag) where T : struct, Enum
        {
            return ((int)(object)value & (int)(object)flag) != 0;
        }
    }
}