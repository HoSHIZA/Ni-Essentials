using System;
using System.Reflection;
using UnityEngine;

namespace NiGames.Essentials
{
    public abstract class MonoBehaviourSingleton<T> : MonoBehaviour
        where T : MonoBehaviourSingleton<T>
    {
        #region Static

        private static T _instance;
        
        public static T Instance
        {
            get
            {
                if (_instance && !_instance.gameObject)
                {
                    _instance = null;
                }
                else
                {
                    return _instance;
                }

                if (_instance) return _instance;

                var instances = FindObjectsOfType<T>();
                foreach (var instance in instances)
                {
                    if (_instance == null)
                    {
                        _instance = instance;

                        if (_instance._attribute.DestroyNonSingletonInstances) continue;
                        break;
                    }
                    
                    DestroyImmediate(instance.gameObject);
                }

                if (_instance) return _instance;
                
                var go = new GameObject(GameObjectName);
                
                _instance = go.AddComponent<T>();
                
                if (_instance._attribute.DontDestroyOnLoad)
                {
                    DontDestroyOnLoad(_instance);
                }
                
                return _instance;
            }
        }
        
        public static bool HasInstance => _instance;
        
        private static string GameObjectName => $"[Singleton] {typeof(T).Name}]";
        
        #endregion
        
        private MonoBehaviourSingletonAttribute _attribute;
        
        protected virtual void Awake()
        {
            if (_instance)
            {
                if (_instance._attribute.DestroyNonSingletonInstances)
                {
                    if (string.Equals(gameObject.name, GameObjectName, StringComparison.OrdinalIgnoreCase))
                    {
                        DestroyImmediate(gameObject);
                    }
                    else
                    {
                        Destroy(this);
                    }
                }
            }
            else
            {
                _attribute ??=
                    typeof(T).GetCustomAttribute<MonoBehaviourSingletonAttribute>() ??
                    new MonoBehaviourSingletonAttribute();
                
                _instance = this as T;
                
                if (_attribute.DontDestroyOnLoad)
                {
                    DontDestroyOnLoad(_instance);
                }
            }
        }
    }
}