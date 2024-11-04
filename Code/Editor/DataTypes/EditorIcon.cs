using System;
using UnityEditor;
using UnityEngine;

namespace NiGames.Essentials.Editor
{
    [Serializable]
    internal struct EditorIcon
    {
        [SerializeField] private string _unityIconName;
        [SerializeField] private Texture2D _texture;
        
        public EditorIcon(string unityIconName) : this()
        {
            _unityIconName = unityIconName;
        }

        public EditorIcon(Texture2D texture) : this()
        {
            _texture = texture;
        }
        
        public Texture2D GetTexture()
        {
            if (_texture) return _texture;

            var log = Debug.unityLogger.logEnabled;
            Debug.unityLogger.logEnabled = false;
            var icon = (Texture2D)EditorGUIUtility.IconContent(_unityIconName)?.image;
            Debug.unityLogger.logEnabled = log;
            
            return icon;
        }

        public static implicit operator Texture2D(EditorIcon icon)
        {
            return icon.GetTexture();
        }

        public static implicit operator EditorIcon(Texture2D texture)
        {
            return new EditorIcon(texture);
        }

        public static implicit operator EditorIcon(string unityIconName)
        {
            return new EditorIcon(unityIconName);
        }
    }
}