using System.Diagnostics.CodeAnalysis;

namespace NiGames.Essentials
{
    [SuppressMessage("ReSharper", "MemberHidesStaticFromOuterClass")]
    [SuppressMessage("ReSharper", "MemberCanBePrivate.Global")]
    [SuppressMessage("ReSharper", "UnusedMember.Global")]
    [SuppressMessage("ReSharper", "UnusedType.Global")]
    public static class Constants
    {
        public const string ROOT = "NiGames";
        public const string PACKAGE_NAME = "Essentials";
        public const string PACKAGE_NAMESPACE = ROOT + "." + PACKAGE_NAME; 
        
        public static class Assembly
        {
            public const string RUNTIME = PACKAGE_NAMESPACE;
            public const string EDITOR = RUNTIME + ".Editor";
        }
        
        public static class Menu
        {
            public const string ROOT = Constants.ROOT + "/";
            
            public static class Component
            {
                public const string UI = "UI/" + ROOT;
                public const string MISCELLANEOUS = UI + "Miscellaneous/";
            }
        }
    }
}