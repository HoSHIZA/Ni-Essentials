using System.Reflection;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace NiGames.Essentials
{
    public abstract class ScriptableObjectSingleton<T> : ScriptableObject 
        where T : ScriptableObjectSingleton<T>
    {
        #region Static
        
        private static T _instance;
        
        public static T Instance
        {
            get
            {
                if (_instance) return _instance;
                
                var attr = GetAttribute();
                
                if (attr == null)
                {
                    _instance = CreateInstance<T>();
                    
                    return _instance;
                }

                if (!string.IsNullOrEmpty(attr.ResourcesPath))
                {
                    _instance = Resources.Load<T>(attr.ResourcesPath);
                }

                if (_instance) return _instance;
                
                _instance = CreateInstance<T>();
                
                return _instance;
            }
        }
        
        public static bool HasInstance => _instance;
        
        public static void SetInstance(T instance, bool force = false)
        {
            if (!force && _instance) return;
            
            _instance = instance;
        }
        
        [MethodImpl(256)]
        private static ScriptableObjectSingletonAttribute GetAttribute()
        {
            return typeof(T).GetCustomAttribute<ScriptableObjectSingletonAttribute>();
        }

        #endregion

        private void OnEnable()
        {
            if (!HasInstance) return;
            
            DestroyImmediate(_instance);
                
            Debug.LogWarning($"Only one instance of a `{GetType().Name}` is allowed.");
        }
    }
}