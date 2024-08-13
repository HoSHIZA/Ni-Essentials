using System;
using System.Linq;
using System.Reflection;
using JetBrains.Annotations;

namespace NiGames.Essentials
{
    /// <summary>
    /// Class providing additional functionality for complex type conversion in different ways.
    /// </summary>
    [PublicAPI]
    public static class ComplexConvert
    {
        private static object[] _cachedConvertParameter;
        
        /// Applies complex type conversion.
        /// The first thing that happens is the type conversion,
        /// then an attempt at conversion via the <see cref="IConvertible"/> interface,
        /// and at the end there is an attempt to use explicit or implicit operation using reflection.
        /// If it fails, it returns an error.
        public static T ChangeType<T>(object obj)
        {
            switch (obj)
            {
                case null:          return default;
                case T objT:        return objT;
                case IConvertible:  return (T)Convert.ChangeType(obj, typeof(T));
            }
            
            var method = obj.GetType()
                .GetMethods(BindingFlags.Public | BindingFlags.Static)
                .FirstOrDefault(t => t.Name is "op_Implicit" or "op_Explicit" && t.ReturnType == typeof(T));
            
            if (method != null)
            {
                _cachedConvertParameter ??= new object[1];
                _cachedConvertParameter[0] = obj;
                
                return (T)method.Invoke(null, _cachedConvertParameter);
            }
            
            return default;
        }
    }
}