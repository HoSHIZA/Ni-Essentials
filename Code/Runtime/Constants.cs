using System.Diagnostics.CodeAnalysis;

namespace NiGames.Essentials
{
    [SuppressMessage("ReSharper", "MemberHidesStaticFromOuterClass")]
    [SuppressMessage("ReSharper", "MemberCanBePrivate.Global")]
    [SuppressMessage("ReSharper", "UnusedMember.Global")]
    [SuppressMessage("ReSharper", "UnusedType.Global")]
    public static class Constants
    {
        public const string PACKAGE_NAME = "Essentials";
        public const string PACKAGE_NAMESPACE = "NiGames.Essentials";
        
        public static class Assembly
        {
            public const string RUNTIME = "NiGames.Essentials";
            public const string EDITOR = RUNTIME + ".Editor";
        }
        
        public static class Menu
        {
            public const string ROOT = PACKAGE_NAMESPACE + "/";
            
            public const string UI = "UI/" + ROOT;
            public const string COMPONENTS = ROOT;
        }
    }
}