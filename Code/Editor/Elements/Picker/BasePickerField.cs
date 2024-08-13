using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

namespace NiGames.Essentials.Editor
{
    public abstract class BasePickerField<T> : BaseField<T> where T : class
    {
        private const string STYLE_SHEET_PATH = "BasePickerField.uss";
        
        public const string USS_CLASS_NAME = "picker-field";
        public const string INPUT_USS_CLASS_NAME = USS_CLASS_NAME + "__input";
        public const string LABEL_USS_CLASS_NAME = USS_CLASS_NAME + "__label";
        public const string BUTTON_USS_CLASS_NAME = INPUT_USS_CLASS_NAME + "__button";
        public const string ICON_USS_CLASS_NAME = BUTTON_USS_CLASS_NAME + "__icon";
        public const string INPUT_LABEL_USS_CLASS_NAME = BUTTON_USS_CLASS_NAME + "__label";
        
        protected readonly PickerControl Control;
        
        protected BasePickerField(string label, VisualElement visualInput) : base(label, visualInput)
        {
        }
        
        protected BasePickerField(string label, PickerControl control) : base(label, control)
        {
            Control = control;
            Control.RegisterCallback<ChangeEvent<T>>(evt =>
            {
                if (evt.currentTarget != Control) return;
                
                base.value = evt.newValue;
                evt.StopImmediatePropagation();
            });
            
            labelElement.AddToClassList(LABEL_USS_CLASS_NAME);
            
            AddToClassList(alignedFieldUssClassName);
            AddToClassList(USS_CLASS_NAME);
            
            this.AddStyleSheet(STYLE_SHEET_PATH);
        }
        
        public override void SetValueWithoutNotify(T newValue)
        {
            base.SetValueWithoutNotify(newValue);
            
            Control.SetValueWithoutNotify(newValue);
        }
        
        protected abstract class PickerControl : VisualElement
        {
            protected const string NONE = "None";
            
            private readonly Button _button;
            private readonly Image _icon;
            private readonly TextElement _label;

            protected PickerProvider<T> Provider;

            public abstract void SetValueWithoutNotify(T newValue);

            public PickerControl()
            {
                _button = new Button();
                _button.AddToClassList(BUTTON_USS_CLASS_NAME);
                _button.AddToClassList(BasePopupField<T, T>.inputUssClassName);
                _button.clicked += () =>
                {
                    if (Provider)
                    {
                        SearchWindow.Open(
                            new SearchWindowContext(GUIUtility.GUIToScreenPoint(
                                new Vector2(worldBound.center.x, worldBound.yMax + worldBound.height - 4)), worldBound.width), 
                            Provider);
                    }
                };
                
                _icon = new Image { pickingMode = PickingMode.Ignore };
                _icon.AddToClassList(ICON_USS_CLASS_NAME);
                
                _label = new Label { pickingMode = PickingMode.Ignore };
                _label.AddToClassList(INPUT_LABEL_USS_CLASS_NAME);
                
                var arrow = new VisualElement { pickingMode = PickingMode.Ignore };
                arrow.AddToClassList(BasePopupField<T, T>.arrowUssClassName);
                
                _button.Add(_icon);
                _button.Add(_label);
                _button.Add(arrow);
                
                Add(_button);
            }
            
            protected void SetLabel(Texture icon, string text)
            {
                ((INotifyValueChanged<string>)_label).SetValueWithoutNotify(text);
                _icon.image = icon;
                _icon.SetDisplay(icon);
            }
        }
    }
}