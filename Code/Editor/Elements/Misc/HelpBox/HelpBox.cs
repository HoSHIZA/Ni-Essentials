using UnityEngine;
using UnityEngine.UIElements;

namespace NiGames.Essentials.Editor.UI
{
    // ReSharper disable once PartialTypeWithSinglePart
    public partial class HelpBox : VisualElement
    {
        private const string STYLE_SHEET_PATH = "HelpBox.uss";

        public const string USS_CLASS_NAME = "nis-help-box";
        
        public const string ICON_USS_CLASS_NAME = USS_CLASS_NAME + "__icon";
        public const string ICON_INFO_USS_CLASS_NAME = ICON_USS_CLASS_NAME + "--info";
        public const string ICON_WARN_USS_CLASS_NAME = ICON_USS_CLASS_NAME + "--warn";
        public const string ICON_ERROR_USS_CLASS_NAME = ICON_USS_CLASS_NAME + "--error";
        
        public const string LABEL_USS_CLASS_NAME = USS_CLASS_NAME + "__label";
        
        private readonly VisualElement _icon;
        private readonly Label _text;

        private string _iconClass;
        private HelpBoxMessageType _messageType;
        private TextAnchor _textAnchor;
        
        public string Text
        {
            get => _text.text;
            private set => _text.text = value;
        }
        
        public HelpBoxMessageType MessageType
        {
            get => _messageType;
            set
            {
                if (_messageType == value) return;
                
                _messageType = value;
                
                UpdateIcon(value);
            }
        }

        public TextAnchor TextAnchor
        {
            get => _textAnchor;
            set => _text.style.unityTextAlign = value;
        }
        
        public HelpBox() : this(string.Empty)
        {
        }
        
        public HelpBox(string text, HelpBoxMessageType messageType = HelpBoxMessageType.None, TextAnchor textAnchor = TextAnchor.UpperLeft)
        {
            AddToClassList(USS_CLASS_NAME);
            
            _icon = new VisualElement();
            _icon.AddToClassList(ICON_USS_CLASS_NAME);
            _messageType = messageType;
            UpdateIcon(messageType);
            
            _text = new Label(text);
            _text.AddToClassList(LABEL_USS_CLASS_NAME);
            _text.style.unityTextAlign = textAnchor;
            
            Add(_text);
            
            this.AddStyleSheet(STYLE_SHEET_PATH);
        }
        
        private void UpdateIcon(HelpBoxMessageType messageType)
        {
            if (!string.IsNullOrEmpty(_iconClass))
            {
                _icon.RemoveFromClassList(_iconClass);
            }

            _iconClass = GetIconClass(messageType);

            if (_iconClass == null)
            {
                _icon.RemoveFromHierarchy();
            }
            else
            {
                _icon.AddToClassList(_iconClass);

                if (_icon.parent == null)
                {
                    Insert(0, _icon);
                }
            }
        }
        
        private string GetIconClass(HelpBoxMessageType messageType)
        {
            return messageType switch
            {
                HelpBoxMessageType.Info => ICON_INFO_USS_CLASS_NAME,
                HelpBoxMessageType.Warning => ICON_WARN_USS_CLASS_NAME,
                HelpBoxMessageType.Error => ICON_ERROR_USS_CLASS_NAME,
                _ => null
            };
        }
        
        #region UXML Factory
        
        private new class UxmlFactory : UxmlFactory<HelpBox, UxmlTraits> {}
        private new class UxmlTraits : VisualElement.UxmlTraits
        {
            private readonly UxmlStringAttributeDescription _text = new() { name = "text", defaultValue = "Text" };
            private readonly UxmlEnumAttributeDescription<HelpBoxMessageType> _messageType = new() { name = "message-type" };
            private readonly UxmlEnumAttributeDescription<TextAnchor> _textAnchor = new() { name = "text-anchor" };
            
            public override void Init(VisualElement element, IUxmlAttributes bag, CreationContext cc)
            {
                base.Init(element, bag, cc);
                
                var helpBox = (HelpBox)element;
                helpBox.Text = _text.GetValueFromBag(bag, cc);
                helpBox.MessageType = _messageType.GetValueFromBag(bag, cc);
                helpBox.TextAnchor = _textAnchor.GetValueFromBag(bag, cc);
            }
        }
        
        #endregion
    }
}