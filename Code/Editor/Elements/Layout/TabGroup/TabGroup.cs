using System.Collections.Generic;
using UnityEngine.UIElements;

namespace NiGames.Essentials.Editor.UI
{
    public class TabGroup : VisualElement
    {
        private const string STYLE_SHEET_PATH = "TabGroup.uss";

        private readonly VisualElement _tabsContainer;
        private readonly VisualElement _contentContainer;
        private readonly List<(Button button, VisualElement content)> _tabs = new();
        private int _activeTabIndex = -1;

        public override VisualElement contentContainer => _contentContainer;

        public int ActiveTabIndex
        {
            get => _activeTabIndex;
            set
            {
                if (value >= 0 && value < _tabs.Count && _activeTabIndex != value)
                {
                    SetActiveTab(value);
                }
            }
        }

        public TabGroup()
        {
            AddToClassList("tab-group");
            
            _tabsContainer = new VisualElement { name = "tabs" };
            _tabsContainer.AddToClassList("tab-group__tabs");
            hierarchy.Add(_tabsContainer);
            
            _contentContainer = new VisualElement { name = "content" };
            _contentContainer.AddToClassList("tab-group__content");
            hierarchy.Add(_contentContainer);
            
            this.AddStyleSheet(STYLE_SHEET_PATH);

            RegisterCallback<AttachToPanelEvent>(OnAttachToPanel);
        }

        private void OnAttachToPanel(AttachToPanelEvent evt)
        {
            GenerateTabs();
        }

        private void GenerateTabs()
        {
            _tabsContainer.Clear();
            _tabs.Clear();

            for (int i = 0; i < _contentContainer.childCount; i++)
            {
                var content = _contentContainer[i];
                var tabName = content.name ?? $"Tab {i + 1}";
                
                var button = new Button { text = tabName };
                button.AddToClassList("tab-group__tab");
                
                var index = i;
                button.clicked += () => SetActiveTab(index);
                
                _tabsContainer.Add(button);
                content.style.display = DisplayStyle.None;
                
                _tabs.Add((button, content));
            }

            if (_tabs.Count > 0)
            {
                SetActiveTab(0);
            }
        }

        private void SetActiveTab(int index)
        {
            if (_activeTabIndex >= 0 && _activeTabIndex < _tabs.Count)
            {
                _tabs[_activeTabIndex].button.RemoveFromClassList("tab-group__tab--active");
                _tabs[_activeTabIndex].content.style.display = DisplayStyle.None;
            }
            
            _activeTabIndex = index;
            _tabs[index].button.AddToClassList("tab-group__tab--active");
            _tabs[index].content.style.display = DisplayStyle.Flex;
        }

        public new void Add(VisualElement element)
        {
            base.Add(element);
            if (panel != null)
            {
                GenerateTabs();
            }
        }

        public new void Remove(VisualElement element)
        {
            base.Remove(element);
            if (panel != null)
            {
                GenerateTabs();
            }
        }
        
        #region UXML Factory
        
        public new class UxmlFactory : UxmlFactory<TabGroup, UxmlTraits> {}
        
        public new class UxmlTraits : VisualElement.UxmlTraits
        {
            public override IEnumerable<UxmlChildElementDescription> uxmlChildElementsDescription
            {
                get { yield return new UxmlChildElementDescription(typeof(VisualElement)); }
            }
        }
        
        #endregion
    }
}