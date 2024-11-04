using System;
using System.Linq;
using System.Runtime.CompilerServices;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;
using Object = UnityEngine.Object;

namespace NiGames.Essentials.Editor.UI
{
#if UNITY_2023_2_OR_NEWER
    [UxmlElement("TypePickerField")]
#endif
    // ReSharper disable once PartialTypeWithSinglePart
    public partial class TypePickerField : BasePickerField<string>
    {
        private TypePickerControl Picker => (TypePickerControl)Control;
        
        #region UXML Attributes
        
#if UNITY_2023_2_OR_NEWER
        [UxmlAttribute("type")]
#endif
        public Type Type
        {
            get => Picker.Type;
            set => Picker.SetType(value, Filter);
        }
        
#if UNITY_2023_2_OR_NEWER
        [UxmlAttribute("type-filter")]
#endif
        public TypeFilterMask Filter
        {
            get => Picker.Filter;
            set => Picker.SetType(Type, value);
        }
        
        #endregion

        public Type TypeValue => Picker.Value;

        #region Constructors
        
        public TypePickerField() : this(null, null)
        {
        }
        
        public TypePickerField(string label) : base(label, new TypePickerControl())
        {
            AddToClassList(USS_CLASS_NAME);
        }
        
        public TypePickerField(string label, Type baseType, TypeFilterMask filter = TypeFilterMask.SerializablePreset) : 
            this(label)
        {
            Picker.SetType(baseType, filter);
        }
        
        public TypePickerField(Type baseType, TypeFilterMask filter = TypeFilterMask.SerializablePreset) : 
            this(null, baseType, filter)
        {
        }

        #endregion
        
#if UNITY_2023_2_OR_NEWER
        protected override void HandleEventBubbleUp(EventBase evt)
#else
        protected override void ExecuteDefaultActionAtTarget(EventBase evt)
#endif
        {
            if (BindingUtility.TryGetPropertyBindEvent(evt, out var property))
            {
                if (property.propertyType is SerializedPropertyType.ManagedReference or SerializedPropertyType.String)
                {
                    if (binding != null) this.Unbind();
                    
                    label ??= property.displayName;
                    
                    if (property.propertyType == SerializedPropertyType.ManagedReference)
                    {
                        Type ??= property.GetManagedFieldType();
                        Filter = TypeFilterMask.SerializedReferencePreset;
                        
                        BindingUtility.CreateBind(this, property, 
                            getter: static prop => GetterManaged(prop), 
                            setter: static (prop, v) => SetterManaged(prop, v), 
                            comparer: static (value, prop, getter) => Comparer(value, prop, getter));
                    }
                    else if (property.propertyType == SerializedPropertyType.String)
                    {
                        Type ??= typeof(object);
                        
                        BindingUtility.CreateBind(this, property, 
                            getter: static prop => GetterString(prop), 
                            setter: static (prop, v) => SetterString(prop, v), 
                            comparer: static (value, prop, getter) => Comparer(value, prop, getter));
                    }
                }
                
                evt.StopPropagation();
#if !UNITY_2023_2_OR_NEWER
                evt.PreventDefault();
#endif
            }
            
            return;
            
            static string GetterManaged(SerializedProperty prop)
            {
                return prop.GetManagedValueType()?.AssemblyQualifiedName;
            }

            static void SetterManaged(SerializedProperty prop, string v)
            {
                Undo.RegisterCompleteObjectUndo(prop.serializedObject.targetObject, "Change reference");
                
                prop.serializedObject.Update();
                prop.managedReferenceValue = TypeHelper.CreateInstance(v);
                prop.serializedObject.ApplyModifiedProperties();
                
                Undo.FlushUndoRecordObjects();
            }
            
            static string GetterString(SerializedProperty prop)
            {
                return prop.stringValue;
            }

            static void SetterString(SerializedProperty prop, string v)
            {
                Undo.RegisterCompleteObjectUndo(prop.serializedObject.targetObject, "Change type string");
                
                prop.serializedObject.Update();
                prop.stringValue = v;
                prop.serializedObject.ApplyModifiedProperties();
                
                Undo.FlushUndoRecordObjects();
            }

            static bool Comparer(string value, SerializedProperty prop, Func<SerializedProperty, string> getter)
            {
                return string.Equals(value, getter.Invoke(prop), StringComparison.Ordinal);
            }
        }
        
        private class TypeProvider : PickerProvider<string>
        {
        }
        
        private class TypePickerControl : PickerControl, IDragReceiver
        {
            private Texture _typeIcon;
            
            public Type Value { get; private set; }
            public Type Type { get; private set; }
            public TypeFilterMask Filter { get; private set; }
            
            private string ValueName => Value?.AssemblyQualifiedName ?? string.Empty;
            
            public TypePickerControl()
            {
                SetLabel(null, GetLabelText());

                this.MakeDragReceiver();
            }
            
            public override void SetValueWithoutNotify(string newValue)
            {
                if (ValueName == newValue) return;
                
                var type = GetType(newValue);
                
                if (string.IsNullOrEmpty(newValue) || (Type != null && Type.IsAssignableFrom(type)))
                {
                    Value = type;
                    
                    SetLabel(GetIconForType(type), GetLabelText());
                }
                else
                {
                    Debug.LogWarning($"[{nameof(TypePickerField)}]{(string.IsNullOrEmpty(name) ? "" : $"({name})")} '{newValue}' is not derived from type '{Type}'");
                }
            }
            
            #region IDrawReceiver Impl
            
            public bool IsDragValid(Object[] objects, object data)
            {
                if (objects.Length <= 0) return false;
                
                var obj = objects[0];
                
                if (obj == null) return false;
                
                var drag = obj.GetType();
                
                return Type != null && Type.IsAssignableFrom(drag);
            }
            
            public void AcceptDrag(Object[] objects, object data)
            {
                OnSelected(objects[0].GetType().AssemblyQualifiedName);
            }
            
            #endregion
            
            public void SetType(Type type, TypeFilterMask filter = TypeFilterMask.SerializablePreset)
            {
                if (type == Type && filter == Filter) return;
                
                if (Provider)
                {
                    Object.DestroyImmediate(Provider);
                }
                
                Provider = null;
                Type = type;
                Filter = filter;
                
                if (type != null)
                {
                    var list = TypeHelper.GetTypesWithPaths(type, filter);

                    var paths = list.Paths.Prepend(NONE);
                    var items = list.Items.Select(t => t.AssemblyQualifiedName).Prepend(string.Empty);
                    
                    Provider = ScriptableObject.CreateInstance<TypeProvider>();
                    Provider.Setup(type.Name, paths, items, GetIconForType, OnSelected);

                    _typeIcon = AssetPreview.GetMiniTypeThumbnail(type);
                }
                
                if (type == null || (Value != null && !type.IsAssignableFrom(Value)))
                {
                    OnSelected(null);
                }
                else
                {
                    SetLabel(GetIconForType(type), GetLabelText());
                }
            }

            private string GetLabelText()
            {
                return Value == null 
                    ? $"None ({Type?.Name ?? "No Base Type"})" 
                    : TypeHelper.GetTypeDisplayName(Value);
            }

            private Texture GetIconForType(string typeName)
            {
                return GetIconForType(GetType(typeName));
            }

            private Texture GetIconForType(Type type)
            {
                if (type == null) return _typeIcon;
                
                var icon = AssetPreview.GetMiniTypeThumbnail(type);
                
                return icon ? icon : _typeIcon;
            }
            
            private void OnSelected(string selected)
            {
                if (ValueName == selected) return;
                
                this.SendChangeEvent(ValueName, selected);
            }
            
            [MethodImpl(256)]
            private static Type GetType(string typeName)
            {
                return string.IsNullOrEmpty(typeName) ? null : Type.GetType(typeName, false);
            }
        }
        
        #region UXML Factory
        
#if !UNITY_2023_2_OR_NEWER
        public new class UxmlFactory : UxmlFactory<TypePickerField, UxmlTraits> { }
        public new class UxmlTraits : BaseFieldTraits<string, UxmlStringAttributeDescription>
        {
            private readonly UxmlTypeAttributeDescription<Type> _type = new UxmlTypeAttributeDescription<Type> { name = "type" };
            private readonly UxmlEnumAttributeDescription<TypeFilterMask> _filter = new UxmlEnumAttributeDescription<TypeFilterMask> { name = "type-filter", defaultValue = TypeFilterMask.SerializedReferencePreset };
            
            public override void Init(VisualElement element, IUxmlAttributes bag, CreationContext cc)
            {
                var field = (TypePickerField)element;
                
                var type = _type.GetValueFromBag(bag, cc);
                var filter = _filter.GetValueFromBag(bag, cc);
                
                field.Type = type;
                field.Filter = filter;
                
                base.Init(element, bag, cc);
            }
        }
#endif

        #endregion
    }
}