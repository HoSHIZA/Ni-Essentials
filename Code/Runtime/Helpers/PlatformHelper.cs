using JetBrains.Annotations;
using UnityEngine;

namespace NiGames.Essentials
{
    [PublicAPI]
    public enum Platform
    {
        Unknown,
        Desktop,
        Mobile,
        Console,
    }
    
    namespace Helpers
    {
        [PublicAPI]
        public static class PlatformHelper
        {
            public static Platform GetCurrentPlatform()
            {
                switch (Application.platform)
                {
                    case RuntimePlatform.WindowsEditor:
                    case RuntimePlatform.WindowsPlayer:
                    case RuntimePlatform.OSXEditor:
                    case RuntimePlatform.OSXPlayer:
                    case RuntimePlatform.LinuxEditor:
                    case RuntimePlatform.LinuxPlayer:
                        return Platform.Desktop;
                    case RuntimePlatform.Android:
                    case RuntimePlatform.IPhonePlayer:
                        return Platform.Mobile;
                    case RuntimePlatform.PS4:
                    case RuntimePlatform.PS5:
                    case RuntimePlatform.XboxOne:
                    case RuntimePlatform.Switch:
                        return Platform.Console;
                    default:
                        return Platform.Unknown;
                }
            }
        }
    }
}