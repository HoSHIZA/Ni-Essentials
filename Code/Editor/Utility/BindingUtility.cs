using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace NiGames.Essentials.Editor
{
    /// <summary>
    /// A utility class to help custom VisualElements with data binding to them.
    /// </summary>
    /// <remarks>Uses reflection and depending on the version of unity its internals may change,
    /// please write to the developer in case of error.</remarks>
    public static class BindingUtility
    {
        #region Reflected Data
        
        private const string CREATE_BIND_NAME = "CreateBind";
        private const string BINDING_NAMESPACE = "UnityEditor.UIElements.Bindings";
        private const string SERIALIZED_OBJECT_BINDING_CONTEXT_TYPE_NAME = BINDING_NAMESPACE + ".SerializedObjectBindingContext, UnityEditor";
        private const string SERIALIZED_OBJECT_BINDING_TYPE_NAME = BINDING_NAMESPACE + ".SerializedObjectBinding`1, UnityEditor";
        
        private const string BIND_OBJECT_PROPERTY_NAME = "bindObject";
        private const string BIND_PROPERTY_PROPERTY_NAME = "bindProperty";
        
        private const string SERIALIZED_OBJECT_BIND_EVENT_NAME = "UnityEditor.UIElements.SerializedObjectBindEvent, UnityEditor";
        private const string SERIALIZED_PROPERTY_BIND_EVENT_NAME = "UnityEditor.UIElements.SerializedPropertyBindEvent, UnityEditor";
        
        private static readonly Type _serializedObjectBindingContextType;
        private static readonly Type _serializedObjectBindingType;
        private static readonly Dictionary<Type, MethodInfo> _createBindMethods = new Dictionary<Type, MethodInfo>();
        
        private static readonly Type _serializedObjectBindEventType;
        private static readonly Type _serializedPropertyBindEventType;
        private static readonly PropertyInfo _bindObjectProperty;
        private static readonly PropertyInfo _bindPropertyProperty;
        
        static BindingUtility()
        {
            var serializedObjectBindingContextType = Type.GetType(SERIALIZED_OBJECT_BINDING_CONTEXT_TYPE_NAME);
            var serializedObjectBindingContextConstructor = serializedObjectBindingContextType?.GetConstructor(new Type[]
            {
                typeof(SerializedObject)
            });
            
            if (serializedObjectBindingContextConstructor != null)
            {
                _serializedObjectBindingContextType = serializedObjectBindingContextType;
            }
            
            _serializedObjectBindingType = Type.GetType(SERIALIZED_OBJECT_BINDING_TYPE_NAME);
            
            // Serialized Object Bind Event
            var serializedObjectBindEventType = Type.GetType(SERIALIZED_OBJECT_BIND_EVENT_NAME);
            var bindObjectProperty = serializedObjectBindEventType?
                .GetProperty(BIND_OBJECT_PROPERTY_NAME, BindingFlags.Instance | BindingFlags.Public);
            
            if (serializedObjectBindEventType != null && bindObjectProperty != null &&
                bindObjectProperty.PropertyType == typeof(SerializedObject))
            {
                _serializedObjectBindEventType = serializedObjectBindEventType;
                _bindObjectProperty = bindObjectProperty;
            }
            
            // Serialized Property Bind Event
            var serializedPropertyBindEventType = Type.GetType(SERIALIZED_PROPERTY_BIND_EVENT_NAME);
            var bindPropertyProperty = serializedPropertyBindEventType?
                .GetProperty(BIND_PROPERTY_PROPERTY_NAME, BindingFlags.Instance | BindingFlags.Public);
            
            if (serializedPropertyBindEventType != null && bindPropertyProperty != null &&
                bindPropertyProperty.PropertyType == typeof(SerializedProperty))
            {
                _serializedPropertyBindEventType = serializedPropertyBindEventType;
                _bindPropertyProperty = bindPropertyProperty;
            }
            
            // Verify unity internal changes
            if (_serializedObjectBindingContextType == null || _serializedObjectBindingType == null || 
                _serializedObjectBindEventType == null || _bindObjectProperty == null ||
                _serializedPropertyBindEventType == null || _bindPropertyProperty == null)
            {
                Debug.LogError(
                    $"[{nameof(BindingUtility)}] Unity internals have changed! " +
                    $"Details:\n" +
                    $"- _serializedObjectBindingContextType: {_serializedObjectBindingContextType != null}\n" +
                    $"- _serializedObjectBindingType: {_serializedObjectBindingType != null}\n" +
                    $"- _serializedObjectBindEventType: {_serializedObjectBindEventType != null}\n" +
                    $"- _serializedPropertyBindEventType: {_serializedPropertyBindEventType != null}\n" +
                    $"- _bindObjectProperty: {_bindObjectProperty != null}\n" +
                    $"- _bindPropertyProperty: {_bindPropertyProperty != null}");
            }
        }
        
        #endregion
        
        /// <summary>
        /// Returns <see cref="SerializedProperty"/> if the passed EventBase
        /// is <see cref="UnityEditor.UIElements.SerializedPropertyBindEvent"/>.
        /// </summary>
        /// <remarks>Utilizes reflection as these events are internal to unity.</remarks>
        public static bool TryGetPropertyBindEvent(EventBase evt, out SerializedProperty property)
        {
            property = evt.GetType() == _serializedPropertyBindEventType
                ? _bindPropertyProperty?.GetValue(evt) as SerializedProperty
                : null;
            
            return property != null;
        }
        
        /// <summary>
        /// Returns <see cref="SerializedProperty"/> if the passed EventBase
        /// is <see cref="UnityEditor.UIElements.SerializedObjectBindEvent"/>.
        /// </summary>
        /// <remarks>Utilizes reflection as these events are internal to unity.</remarks>
        public static bool TryGetObjectBindEvent(EventBase evt, out SerializedObject @object)
        {
            @object = evt.GetType() == _serializedObjectBindEventType
                ? _bindObjectProperty?.GetValue(evt) as SerializedObject
                : null;
            
            return @object != null;
        }
        
        /// <summary>
        /// Creates a custom binding <see cref="SerializedProperty"/> for <see cref="INotifyValueChanged{T}"/>
        /// with a custom Getter, Setter, and Comparer.
        /// </summary>
        public static void CreateBind<T>(INotifyValueChanged<T> field, SerializedProperty property, 
            Func<SerializedProperty, T> getter, 
            Action<SerializedProperty, T> setter, 
            Func<T, SerializedProperty, Func<SerializedProperty, T>, bool> comparer)
        {
            if (!_createBindMethods.TryGetValue(typeof(T), out var createBindMethod))
            {
                var serializedObjectBindingType = _serializedObjectBindingType?.MakeGenericType(typeof(T));
                createBindMethod = serializedObjectBindingType?.GetMethod(CREATE_BIND_NAME, BindingFlags.Public | BindingFlags.Static);
                
                _createBindMethods.Add(typeof(T), createBindMethod);
            }
            
            var context = Activator.CreateInstance(_serializedObjectBindingContextType, property.serializedObject);
            
            createBindMethod?.Invoke(null, new object[]
            {
                field, context, property, getter, setter, comparer
            });
        }
        
        /// <summary>
        /// Creates a binding to `<see cref="SerializedProperty.managedReferenceValue"/>` for <see cref="SerializedProperty"/>.
        /// </summary>
        public static void BindManagedReference<T>(INotifyValueChanged<T> field, SerializedProperty property, Action onSet = null)
        {
            CreateBind(field, property,
                getter: static prop => Getter(prop),
                setter: (prop, value) => Setter(prop, value),
                comparer: static (value, prop, getter) => Comparer(value, prop, getter));
            
            return;
            
            static T Getter(SerializedProperty prop)
            {
                return prop.managedReferenceValue is T reference ? reference : default;
            }
            
            void Setter(SerializedProperty prop, T value)
            {
                Undo.RegisterCompleteObjectUndo(prop.serializedObject.targetObject, "Change reference");
                
                prop.serializedObject.Update();
                prop.managedReferenceValue = value;
                prop.serializedObject.ApplyModifiedProperties();
                
                Undo.FlushUndoRecordObjects();
                
                onSet?.Invoke();
            }
            
            static bool Comparer(T value, SerializedProperty prop, Func<SerializedProperty, T> getter)
            {
                return ReferenceEquals(value, getter.Invoke(prop));
            }
        }
        
        /// <summary>
        /// Creates a binding to `<see cref="SerializedProperty.isExpanded"/>` for <see cref="SerializedProperty"/>.
        /// </summary>
        public static void BindFoldout(INotifyValueChanged<bool> foldout, SerializedProperty property, Action onSet = null)
        {
            CreateBind(foldout, property, 
                getter: static prop => Getter(prop), 
                setter: (prop, value) => Setter(prop, value), 
                comparer: (value, prop, getter) => Comparer(value, prop, getter));
            
            return;
            
            static bool Getter(SerializedProperty prop)
            {
                return prop.isExpanded;
            }
            
            void Setter(SerializedProperty prop, bool val)
            {
                prop.isExpanded = val;
                
                onSet?.Invoke();
            }
            
            static bool Comparer(bool val, SerializedProperty prop, Func<SerializedProperty, bool> getter)
            {
                var currentValue = getter.Invoke(prop);
                return val == currentValue;
            }
        }
    }
}