using System;

namespace NiGames.Essentials
{
    public abstract class Singleton<T>
        where T : Singleton<T>, new()
    {
        #region Static
        
        private static readonly Lazy<T> LazyInstance = new Lazy<T>(() => 
        {
            var instance = new T();
            instance.Initialize();
            return instance;
        });
        
        public static T Instance => LazyInstance.Value;
        
        public static bool HasInstance => LazyInstance.IsValueCreated;
        
        #endregion
        
        protected virtual void Initialize() { }
    }
}