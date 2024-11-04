using UnityEngine.UIElements;

namespace NiGames.Essentials.Editor.UI
{
    public class Card : VisualElement
    {
        private const string STYLE_SHEET_PATH = "Card.uss";
        
        public const string USS_CLASS_NAME = "ni-card";
        
        public const string HEADER_CLASS_NAME = USS_CLASS_NAME + "__header";
        public const string HEADER_EXPANDED_CLASS_NAME = HEADER_CLASS_NAME + "--expanded";
        
        public const string HEADER_FOLDOUT_ARROW_CLASS_NAME = HEADER_CLASS_NAME + "__foldout-arrow";
        public const string HEADER_FOLDOUT_ARROW_EXPANDED_CLASS_NAME = HEADER_FOLDOUT_ARROW_CLASS_NAME + "--expanded";
        
        public const string HEADER_LABEL_CLASS_NAME = HEADER_CLASS_NAME + "__label";
        
        public const string HEADER_CONTROL_CONTAINER_CLASS_NAME = HEADER_CLASS_NAME + "__control-container";
        
        public const string CONTENT_CLASS_NAME = USS_CLASS_NAME + "__content";
        
        private readonly IManipulator _manipulator;
        
        private Label _header;
        private VisualElement _content;
        private VisualElement _headerContainer;
        private VisualElement _controlContainer;
        private VisualElement _foldoutArrow;
        
        private bool _isCollapsable;
        private bool _isExpanded;
        
        public override VisualElement contentContainer => _content;
        
        public string Title
        {
            get => _header.text;
            private set => _header.text = value;
        }
        
        public bool IsCollapsable
        {
            get => _isCollapsable;
            set
            {
                _isCollapsable = value;
                _foldoutArrow.SetDisplay(value);
                
                if (value)
                {
                    _headerContainer.AddManipulator(_manipulator);
                }
                else
                {
                    _headerContainer.RemoveManipulator(_manipulator);
                }
            }
        }

        public bool IsExpanded
        {
            get => !_isCollapsable || _isExpanded;
            set
            {
                if (_isCollapsable)
                {
                    _isExpanded = value;
                    UpdateContentVisibility();
                }
                else
                {
                    _headerContainer.EnableInClassList(HEADER_EXPANDED_CLASS_NAME, value);
                }
            }
        }
        
        public Card() : this(null)
        {
        }
        
        public Card(string title)
        {
            _manipulator = new Clickable(() => IsExpanded = !IsExpanded);
            
            BuildUI(title);
            
            this.AddStyleSheet(STYLE_SHEET_PATH);
            
            IsCollapsable = false;
        }

        private void BuildUI(string title)
        {
            AddToClassList(USS_CLASS_NAME);
            
            _headerContainer = new VisualElement() { name = "header" };
            _headerContainer.AddToClassList(HEADER_CLASS_NAME);
            
            _foldoutArrow = new VisualElement() { name = "foldout" };
            _foldoutArrow.AddToClassList(HEADER_FOLDOUT_ARROW_CLASS_NAME);
            _headerContainer.Add(_foldoutArrow);
            
            _header = new Label(title) { name = "title" };
            _header.AddToClassList(HEADER_LABEL_CLASS_NAME);
            _headerContainer.Add(_header);
            
            _controlContainer = new VisualElement() { name = "control" };
            _controlContainer.AddToClassList(HEADER_CONTROL_CONTAINER_CLASS_NAME);
            
            var control = CreateControl();
            if (control != null)
            {
                _controlContainer.Add(control);
            }
            _headerContainer.Add(_controlContainer);
            
            hierarchy.Add(_headerContainer);
            
            _content = new VisualElement() { name = "content" };
            _content.AddToClassList(CONTENT_CLASS_NAME);
            hierarchy.Add(_content);
        }

        private void UpdateContentVisibility()
        {
            var isExpanded = IsExpanded;
            
            _content.SetDisplay(isExpanded);
            
            _headerContainer.EnableInClassList(HEADER_EXPANDED_CLASS_NAME, isExpanded);
            _foldoutArrow.EnableInClassList(HEADER_FOLDOUT_ARROW_EXPANDED_CLASS_NAME, isExpanded);
        }

        public virtual VisualElement CreateControl()
        {
            return null;
        }
        
        #region UXML Factory
        
        public new class UxmlFactory : UxmlFactory<Card, UxmlTraits> {}
        public new class UxmlTraits : VisualElement.UxmlTraits
        {
            private readonly UxmlStringAttributeDescription _title = new() { name = "title", defaultValue = "Title" };
            private readonly UxmlBoolAttributeDescription _isCollapsable = new() { name = "is-collapsable" };
            private readonly UxmlBoolAttributeDescription _isExpanded = new() { name = "is-expanded", defaultValue = true };
            
            public override void Init(VisualElement ve, IUxmlAttributes bag, CreationContext cc)
            {
                base.Init(ve, bag, cc);
                
                var card = (Card)ve;
                card.Title = _title.GetValueFromBag(bag, cc);
                card.IsCollapsable = _isCollapsable.GetValueFromBag(bag, cc);
                card.IsExpanded = _isExpanded.GetValueFromBag(bag, cc);
            }
        }
        
        #endregion
    }
}