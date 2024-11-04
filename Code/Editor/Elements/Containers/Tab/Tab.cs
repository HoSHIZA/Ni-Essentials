using UnityEngine;
using UnityEngine.UIElements;

namespace NiGames.Essentials.Editor.UI
{
    public class Tab : VisualElement
    {
        private string _displayName;
        private Texture2D _icon;
        private bool _generateButton;
        
        private TabView _tabView;
        
        public string DisplayName
        {
            get => string.IsNullOrEmpty(_displayName) ? name : _displayName;
            private set => _displayName = value;
        }
        
        public bool GenerateButton
        {
            get => _generateButton;
            set => _generateButton = value;
        }
        
        public Tab() : this(null)
        {
        }
        
        public Tab(string displayName, bool generateButton = true, Texture2D icon = null)
        {
            _displayName = displayName;
            _icon = icon;
            _generateButton = generateButton;
        }
        
        #region UXML Factory
        
        public new class UxmlFactory : UxmlFactory<Tab, UxmlTraits> { }
        public new class UxmlTraits : VisualElement.UxmlTraits
        {
            private readonly UxmlStringAttributeDescription _displayName = new() { name = "display-name" };
            private readonly UxmlBoolAttributeDescription _generateButton = new() { name = "generate-button" };
            // private readonly UxmlDes _icon = new() { name = "icon" };
            
            public override void Init(VisualElement ve, IUxmlAttributes bag, CreationContext cc)
            {
                base.Init(ve, bag, cc);
                
                var tab = (Tab)ve;
                tab.DisplayName = _displayName.GetValueFromBag(bag, cc);
                tab.GenerateButton = _generateButton.GetValueFromBag(bag, cc);
            }
        }

        #endregion
    }
}