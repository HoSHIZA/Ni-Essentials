using System;

namespace NiGames.Essentials
{
    [Serializable]
    public struct TypeReference
    {
        private string _typeFullName;
        
        public TypeReference(string typeFullName)
        {
            _typeFullName = typeFullName;
        }
        
        public TypeReference(Type type)
        {
            _typeFullName = type.FullName;
        }
        
        public Type SystemType
        {
            get => string.IsNullOrEmpty(_typeFullName) ? null : Type.GetType(_typeFullName);
            set => _typeFullName = value?.AssemblyQualifiedName;
        }
        
        public string FullName
        {
            get => _typeFullName;
            set => _typeFullName = value;
        }
        
        public static implicit operator string(TypeReference typeRef)
        {
            return typeRef.FullName;
        }
        
        public static implicit operator Type(TypeReference typeRef)
        {
            return typeRef.SystemType;
        }
        
        public static implicit operator TypeReference(string typeFullName)
        {
            return new TypeReference(typeFullName);
        }
        
        public static implicit operator TypeReference(Type type)
        {
            return new TypeReference(type);
        }
    }
}