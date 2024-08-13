using System;
using System.Linq;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;
using Object = UnityEngine.Object;

namespace NiGames.Essentials.Editor
{
#if UNITY_2023_2_OR_NEWER
    [UxmlElement("ObjectPickerField")]
#endif
    // ReSharper disable once PartialTypeWithSinglePart
    public partial class ObjectPickerField : BasePickerField<Object>
    {
        private ObjectPickerControl Picker => (ObjectPickerControl)Control;

        #region UXML Attributes

#if UNITY_2023_2_OR_NEWER
        [UxmlAttribute("type")]
#endif
        public Type Type
        {
            get => Picker.Type;
            set => Picker.SetType(value);
        }

        #endregion

        #region Constructors

        public ObjectPickerField() : this(null, null)
        {
        }

        public ObjectPickerField(string label) : base(label, new ObjectPickerControl())
        {
            AddToClassList(USS_CLASS_NAME);
        }

        public ObjectPickerField(string label, Type type) : this(label)
        {
            Type = type;
        }

        public ObjectPickerField(Type type) : this(null, type)
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
                if (property.propertyType == SerializedPropertyType.ObjectReference)
                {
                    if (binding != null) this.Unbind();

                    label ??= property.displayName;

                    BindingUtility.CreateBind(this, property, Getter, Setter, Comparer);
                }

                evt.StopPropagation();
#if !UNITY_2023_2_OR_NEWER
                evt.PreventDefault();
#endif
            }
            
            return;

            static Object Getter(SerializedProperty prop)
            {
                return prop.objectReferenceValue;
            }

            static void Setter(SerializedProperty prop, Object v)
            {
                Undo.RegisterCompleteObjectUndo(prop.serializedObject.targetObject, "Change object reference");

                prop.serializedObject.Update();
                prop.objectReferenceValue = v;
                prop.serializedObject.ApplyModifiedProperties();

                Undo.FlushUndoRecordObjects();
            }

            static bool Comparer(Object value, SerializedProperty prop, Func<SerializedProperty, Object> getter)
            {
                return value.Equals(getter.Invoke(prop));
            }
        }

        private class ObjectProvider : PickerProvider<Object>
        {
        }

        private class ObjectPickerControl : PickerControl, IDragReceiver
        {
            public Type Type { get; private set; }

            private Object _value;

            private readonly Button _inspectButton;

            public ObjectPickerControl()
            {
                _inspectButton = new Button(Inspect);
                _inspectButton.SetEnabled(false);

                Add(_inspectButton);
                SetLabel(null, GetLabelText());

                this.MakeDragReceiver();
            }

            public override void SetValueWithoutNotify(Object newValue)
            {
                if (_value == newValue) return;
                
                if (!newValue || (Type != null && Type.IsInstanceOfType(newValue)))
                {
                    _value = newValue;
                    _inspectButton.SetEnabled(_value);

                    SetLabel(GetIconForType(_value), GetLabelText());
                }
                else
                {
                    Debug.LogWarning($"[{nameof(ObjectPickerField)}] '{newValue.name}' is not a object of type '{Type}'");
                }
            }

            public void SetType(Type type)
            {
                if (type != Type)
                {
                    if (Provider)
                    {
                        Object.DestroyImmediate(Provider);
                    }

                    Provider = null;
                    Type = type;

                    if (type != null && typeof(Object).IsAssignableFrom(type))
                    {
                        Provider = ScriptableObject.CreateInstance<ObjectProvider>();

                        if (typeof(Component).IsAssignableFrom(type) || typeof(GameObject) == type)
                        {
                            var objects = ObjectHelper.GetObjectsWithPaths(type, true);

                            var paths = objects.Paths.Prepend(NONE);
                            var items = objects.Items.Prepend(null);

                            Provider.Setup(type.Name, paths, items, GetIconForType, OnSelected);
                        }
                        else
                        {
                            var objects = AssetHelper.GetAssetsWithPaths(type);

                            var paths = objects.Paths.Prepend(NONE);
                            var items = objects.Items.Prepend(null);

                            Provider.Setup(type.Name, paths, items, GetIconForType, OnSelected);
                        }
                    }

                    if (type == null || (_value && !type.IsInstanceOfType(_value)))
                    {
                        OnSelected(null);
                    }
                    else
                    {
                        SetLabel(GetIconForType(_value), GetLabelText());
                    }
                }
            }

            private Texture GetIconForType(Object obj)
            {
                if (obj) return AssetPreview.GetMiniThumbnail(obj);

                return Type == null ? null : AssetPreview.GetMiniTypeThumbnail(Type);
            }

            private string GetLabelText()
            {
                return _value 
                    ? $"{_value.name} ({Type?.Name ?? "Typeless"})" 
                    : $"None ({Type?.Name ?? "Typeless"})";
            }

            private void OnSelected(Object selected)
            {
                if (_value == selected) return;
                
                this.SendChangeEvent(_value, selected);
            }

            private void Inspect()
            {
                if (_value)
                {
                    Selection.activeObject = _value;
                }
            }

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
                OnSelected(objects[0]);
            }
        }

        #region UXML Factory

#if !UNITY_2023_2_OR_NEWER
        public new class UxmlFactory : UxmlFactory<ObjectPickerField, UxmlTraits> { }
        public new class UxmlTraits : BaseField<Object>.UxmlTraits
        {
            private readonly UxmlTypeAttributeDescription<Type> _type = new UxmlTypeAttributeDescription<Type> { name = "type" };
            private readonly UxmlStringAttributeDescription _value = new UxmlStringAttributeDescription { name = "value" };

            public override void Init(VisualElement element, IUxmlAttributes bag, CreationContext cc)
            {
                var field = (ObjectPickerField)element;

                var type = _type.GetValueFromBag(bag, cc);
                var value = _value.GetValueFromBag(bag, cc);

                field.Type = type;

                if (!string.IsNullOrEmpty(value))
                {
                    field.SetValueWithoutNotify(AssetDatabase.LoadAssetAtPath(value, field.Type));
                }

                base.Init(element, bag, cc);
            }
        }
#endif

        #endregion
    }
}