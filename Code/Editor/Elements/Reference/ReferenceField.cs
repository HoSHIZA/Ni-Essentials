using System;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace NiGames.Essentials.Editor
{
#if UNITY_2023_2_OR_NEWER
    [UxmlElement]
#endif
    // ReSharper disable once PartialTypeWithSinglePart
    public partial class ReferenceField : BindableElement, INotifyValueChanged<object>
    {
        private const string STYLE_SHEET_PATH = "ReferenceField.uss";
        
        public const string USS_CLASS_NAME = "reference-field";
        public const string HEADER_USS_CLASS_NAME = USS_CLASS_NAME + "__header";
        
        private object _value;
        private Type _referenceType;
        private bool _drawCustomDrawer;

        private readonly Foldout _foldout;
        private readonly Toggle _foldoutToggle;
        private readonly TypePickerField _typePicker;
        
        public SerializedProperty Property { get; private set; }
        public PropertyDrawer PropertyDrawer { get; private set; }
        
        #region UXML Attributes
        
#if UNITY_2023_2_OR_NEWER
        [UxmlAttribute("label")]
#endif
        public string Label
        {
            get => _typePicker.label;
            set => _typePicker.label = value;
        }
        
#if UNITY_2023_2_OR_NEWER
        [UxmlAttribute("type")]
#endif
        public Type ReferenceType
        {
            get => _referenceType;
            set
            {
                _typePicker.Type = _referenceType = value;
                
                PopulateContent();
            }
        }
        
#if UNITY_2023_2_OR_NEWER
        [UxmlAttribute("draw-custom-drawer")]
#endif
        public bool DrawCustomDrawer
        {
            get => _drawCustomDrawer;
            set
            {
                _drawCustomDrawer = value;
                
                if (Property != null)
                {
                    PropertyDrawer = PropertyDrawerHelper.GetPropertyDrawerForType(Property.GetManagedValueType());
                }

                PopulateContent();
            }
        }
        
        #endregion
        
        public object value
        {
            get => _value;
            set
            {
                var previous = _value;
                
                if (!ReferenceEquals(previous, value))
                {
                    SetValueWithoutNotify(value);
                    this.SendChangeEvent(previous, value);
                    return;
                }
                
                _value = value;
            }
        }
        
        public override VisualElement contentContainer { get; }
        
        #region Constructors
        
        public ReferenceField() : this(null, null)
        {
        }
        
        public ReferenceField(string label, Type referenceType = null, bool drawCustomDrawer = true)
        {
            _foldout = new Foldout();
            
            _foldoutToggle = _foldout.Q<Toggle>();
            _foldoutToggle.name = "reference-foldout-toggle";
            
            contentContainer = _foldout.Q("unity-content");
            
            _typePicker = new TypePickerField(label, referenceType, TypeFilterMask.SerializedReferencePreset)
            {
                name = "reference-type-picker",
            };
            _typePicker.RegisterValueChangedCallback(evt =>
            {
                SetInstanceOfType(evt.newValue);
                evt.StopPropagation();
            });
            
            var element = new VisualElement();
            element.AddToClassList(HEADER_USS_CLASS_NAME);
            
            element.hierarchy.Add(_foldoutToggle);
            element.hierarchy.Add(_typePicker);
            
            _foldout.hierarchy.Insert(0, element);
            
            hierarchy.Add(_foldout);
            
            AddToClassList(USS_CLASS_NAME);
            this.AddStyleSheet(STYLE_SHEET_PATH);
            
            _typePicker.Type = _referenceType = referenceType;
            _drawCustomDrawer = drawCustomDrawer;
            Label = label;
        }
        
        public ReferenceField(Type referenceType, bool drawCustomDrawer = true) : this(null, referenceType, drawCustomDrawer)
        {
        }
        
        #endregion
        
        public void SetValueWithoutNotify(object val)
        {
            _value = val;
        }

        public void ClearContent()
        {
            contentContainer.Clear();
            
            _foldoutToggle.visible = false;
        }

        public void PopulateContent(SerializedProperty @override = null)
        {
            contentContainer.Clear();
            
            var property = @override ?? Property;
            if (property != null)
            {
                if (_drawCustomDrawer && PropertyDrawer != null)
                {
                    var content = PropertyDrawer.CreatePropertyGUI(property);
                    contentContainer.Add(content);
                }
                else
                {
                    foreach (var child in property.GetChildProperties())
                    {
                        var field = new PropertyField(child);
                        contentContainer.Add(field);
                    }
                }
                
                contentContainer.Bind(property.serializedObject);
            }
            
            var hasChild = contentContainer.childCount != 0;

            _foldoutToggle.visible = hasChild;
        }

        public void PopulateContent(Func<VisualElement> @override, bool fallbackPopulate = true)
        {
            if (fallbackPopulate)
            {
                PopulateContent();
            }
            else
            {
                contentContainer.Clear();
            
                if (@override != null)
                {
                    contentContainer.Add(@override.Invoke());
                }
            
                var hasChild = contentContainer.childCount != 0;

                _foldoutToggle.visible = hasChild;
            }
        }

        private void SetInstanceOfType(string selected)
        {
            value = TypeHelper.CreateInstance(selected);
        }
        
#if UNITY_2023_2_OR_NEWER
        protected override void HandleEventBubbleUp(EventBase evt)
#else
        protected override void ExecuteDefaultActionAtTarget(EventBase evt)
#endif
        {
            if (BindingUtility.TryGetPropertyBindEvent(evt, out var property))
            {
                if (property is { propertyType: SerializedPropertyType.ManagedReference })
                {
                    Setup(property);
                }
                
                evt.StopPropagation();
#if !UNITY_2023_2_OR_NEWER
                evt.PreventDefault();
#endif
                
                return;
            }
            
            return;
            
            void Setup(SerializedProperty prop)
            {
                Property = prop;
                
                BindingUtility.BindFoldout(_foldout, prop);
                
                Label ??= prop.displayName;
                ReferenceType ??= prop.GetManagedFieldType();
                
                if (binding != null) this.Unbind();
                BindingUtility.BindManagedReference(this, prop, SetDataAndPopulate);
                
                SetDataAndPopulate();
                
                return;
                
                void SetDataAndPopulate()
                {
                    var valueType = Property.GetManagedValueType();
                    _typePicker.SetValueWithoutNotify(valueType?.AssemblyQualifiedName);
                    PropertyDrawer = PropertyDrawerHelper.GetPropertyDrawerForType(valueType);

                    PopulateContent();
                }
            }
        }
        
        #region UXML Factory
        
#if !UNITY_2023_2_OR_NEWER
        public new class UxmlFactory : UxmlFactory<ReferenceField, UxmlTraits> { }
        public new class UxmlTraits : BindableElement.UxmlTraits
        {
            private readonly UxmlStringAttributeDescription _label = new UxmlStringAttributeDescription { name = "label" };
            private readonly UxmlTypeAttributeDescription<object> _type = new UxmlTypeAttributeDescription<object> { name = "type" };
            private readonly UxmlBoolAttributeDescription _drawCustomDrawer = new UxmlBoolAttributeDescription { name = "draw-custom-drawer", defaultValue = true };

            public override void Init(VisualElement ve, IUxmlAttributes bag, CreationContext cc)
            {
                base.Init(ve, bag, cc);
                
                var field = (ReferenceField)ve;
                
                field.Label = _label.GetValueFromBag(bag, cc);
                field._referenceType = field._typePicker.Type = _type.GetValueFromBag(bag, cc);
                field._drawCustomDrawer = _drawCustomDrawer.GetValueFromBag(bag, cc);
                
                field.PopulateContent();
            }
        }
#endif
        
        #endregion
    }
}