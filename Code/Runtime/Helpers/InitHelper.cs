using System;

namespace NiGames.Essentials.Helpers
{
    public static class InitHelper
    {
        /// <summary>
        /// Uses safe static initialization, calls the <c>action</c> callback if initialization is required.
        /// Not called again when <c>DisableDomainReload</c> is enabled in the editor.
        /// </summary>
        public static void DomainSafeInit(ref bool init, Action action)
        {
#if UNITY_EDITOR
            var domainReloadDisabled = 
                UnityEditor.EditorSettings.enterPlayModeOptionsEnabled && 
                (UnityEditor.EditorSettings.enterPlayModeOptions & UnityEditor.EnterPlayModeOptions.DisableDomainReload) != 0;
            
            if (init && !domainReloadDisabled) return;
#else
            if (init) return;
#endif
            
            action?.Invoke();
            
            init = true;
        }
    }
}