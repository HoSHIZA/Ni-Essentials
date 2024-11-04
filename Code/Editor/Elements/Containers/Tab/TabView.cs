using NiGames.Essentials.Tabs;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.UIElements.Experimental;

namespace NiGames.Essentials.Editor.UI
{
    public class TabView : VisualElement
    {
        public enum TabButtonPanelAlign
        {
            Stretch,
            Flex,
        }
        
        public enum TabButtonPanelOrientation
        {
            Vertical,
            Horizontal,
        }
        
        public const string USS_CLASS_NAME = "ni-tab-view";
        public const string HORIZONTAL_VARIANT_CLASS_NAME = USS_CLASS_NAME + "--horizontal";
        public const string VERTICAL_VARIANT_CLASS_NAME = USS_CLASS_NAME + "--vertical";
        public const string RESIZABLE_VARIANT_CLASS_NAME = USS_CLASS_NAME + "--resizable";
        
        public const string TWO_PANE_SPLIT_VIEW_CLASS_NAME = USS_CLASS_NAME + "__two-pane-split-view";
        
        public const string HORIZONTAL_SCROLLER_CLASS_NAME = USS_CLASS_NAME + "__horizontal-scroller";
        
        public const string TAB_BUTTONS_CLASS_NAME = USS_CLASS_NAME + "__buttons";
        public const string TAB_BUTTONS_SHOW_BACKGROUND_SELECTION_CLASS_NAME = TAB_BUTTONS_CLASS_NAME + "--show-background-selection";
        public const string TAB_BUTTONS_SHOW_SELECTION_LINE_CLASS_NAME = TAB_BUTTONS_CLASS_NAME + "--show-selection-line";
        public const string TAB_BUTTONS_SHOW_SEPARATORS_CLASS_NAME = TAB_BUTTONS_CLASS_NAME + "--show-separators";
        public const string TAB_BUTTONS_ALIGN_STRETCH_VARIANT_CLASS_NAME = TAB_BUTTONS_CLASS_NAME + "--stretch";
        public const string TAB_BUTTONS_ALIGN_FLEX_START_VARIANT_CLASS_NAME = TAB_BUTTONS_CLASS_NAME + "--flex";
        
        public const string TAB_CONTENT_CLASS_NAME = USS_CLASS_NAME + "__content";
        
        public const string TAB_BUTTON_CLASS_NAME = USS_CLASS_NAME + "__button";
        public const string TAB_BUTTON_SELECTED_CLASS_NAME = TAB_BUTTON_CLASS_NAME + "--selected";
        public const string TAB_BUTTON_FIRST_CLASS_NAME = TAB_BUTTON_CLASS_NAME + "--first";
        public const string TAB_BUTTON_LAST_CLASS_NAME = TAB_BUTTON_CLASS_NAME + "--last";
        public const string TAB_BUTTON_CLICKABLE_CLASS_NAME = TAB_BUTTON_CLASS_NAME + "__clickable";
        public const string TAB_BUTTON_ICON_CLASS_NAME = TAB_BUTTON_CLASS_NAME + "__icon";
        public const string TAB_BUTTON_LABEL_CLASS_NAME = TAB_BUTTON_CLASS_NAME + "__label";
        
        private readonly TabSystem _tabSystem = new();
        private readonly ScrollView _tabButtonsScrollView;
        private readonly VisualElement _tabButtonsView;
        private readonly VisualElement _tabButtonsContainer;
        private readonly VisualElement _tabContentContainer;
        private readonly TwoPaneSplitView _twoPaneSplitView;
        private readonly Scroller _horizontalScroller;
        
        private string _saveStateKey;
        private ScrollViewMode _orientation;
        private TabButtonPanelOrientation _currentOrientation;
        private TabButtonPanelAlign _buttonPanelAlign;
        private bool _buttonPanelResizable;
        
        public bool ShowBackgroundSelection
        {
            get => _tabButtonsContainer.ClassListContains(TAB_BUTTONS_SHOW_BACKGROUND_SELECTION_CLASS_NAME);
            set => _tabButtonsContainer.EnableInClassList(TAB_BUTTONS_SHOW_BACKGROUND_SELECTION_CLASS_NAME, value);
        }
        
        public bool ShowSelectionLine
        {
            get => _tabButtonsContainer.ClassListContains(TAB_BUTTONS_SHOW_SELECTION_LINE_CLASS_NAME);
            set => _tabButtonsContainer.EnableInClassList(TAB_BUTTONS_SHOW_SELECTION_LINE_CLASS_NAME, value);
        }
        
        public bool ShowSeparators
        {
            get => _tabButtonsContainer.ClassListContains(TAB_BUTTONS_SHOW_SEPARATORS_CLASS_NAME);
            set => _tabButtonsContainer.EnableInClassList(TAB_BUTTONS_SHOW_SEPARATORS_CLASS_NAME, value);
        }
        
        public ScrollViewMode Orientation
        {
            get => _orientation;
            set
            {
                _orientation = value;
                _currentOrientation = value switch
                {
                    ScrollViewMode.VerticalAndHorizontal => TabButtonPanelOrientation.Horizontal,
                    ScrollViewMode.Horizontal => TabButtonPanelOrientation.Horizontal,
                    _ => TabButtonPanelOrientation.Vertical
                };
                
                UpdateButtonPanelOrientation();
            }
        }
        
        public TabButtonPanelOrientation CurrentOrientation
        {
            get => _currentOrientation;
            set
            {
                if (_orientation != ScrollViewMode.VerticalAndHorizontal) return;
                
                _currentOrientation = value;
                UpdateButtonPanelOrientation();
            }
        }
        
        public TabButtonPanelAlign ButtonPanelAlign
        {
            get => _buttonPanelAlign;
            set
            {
                _buttonPanelAlign = value;
                UpdateButtonPanelAlign();
            }
        }
        
        public bool ButtonPanelResizable
        {
            get => _buttonPanelResizable;
            set
            {
                _buttonPanelResizable = value;
                UpdateButtonPanelResizable();
            }
        }
        
        public override VisualElement contentContainer => _tabContentContainer;
        
        public TabView()
        {
            this.AddStyleSheetWithSimilarName();
            
            AddToClassList(USS_CLASS_NAME);
            
            _tabButtonsView = new VisualElement() { name = "tab-buttons-view" };
            
            _tabButtonsScrollView = new ScrollView()
            {
                horizontalScrollerVisibility = ScrollerVisibility.Hidden,
                horizontalPageSize = 2,
            };
            _tabButtonsScrollView.horizontalScroller.valueChanged += _ => UpdateHorizontalScrollerVisibility();
            _tabButtonsScrollView.RegisterCallback<GeometryChangedEvent, TabView>((_, args) => args.UpdateHorizontalScrollerVisibility(), this);
            _tabButtonsView.Add(_tabButtonsScrollView);
            
            _tabButtonsContainer = new VisualElement { name = "tab-buttons" };
            _tabButtonsContainer.AddToClassList(TAB_BUTTONS_CLASS_NAME);
            _tabButtonsScrollView.Add(_tabButtonsContainer);
            
            _tabContentContainer = new VisualElement { name = "tab-content" };
            _tabContentContainer.AddToClassList(TAB_CONTENT_CLASS_NAME);
            
            _twoPaneSplitView = new TwoPaneSplitView
            {
                fixedPaneIndex = 0,
                fixedPaneInitialDimension = 120,
                orientation = TwoPaneSplitViewOrientation.Horizontal,
            };
            _twoPaneSplitView.AddToClassList(TWO_PANE_SPLIT_VIEW_CLASS_NAME);
            
            var draglineAnchor = _twoPaneSplitView.Q(className: "unity-two-pane-split-view__dragline-anchor");
            draglineAnchor.RegisterCallback<ContextClickEvent, TwoPaneSplitView>((_, splitView) =>
            {
                var menu = new GenericDropdownMenu();
                menu.AddItem("Reset", false, () => splitView.fixedPane.style.width = 120);
                menu.DropDown(draglineAnchor.worldBound, draglineAnchor);
            }, _twoPaneSplitView);
            
            _horizontalScroller = new Scroller()
            {
                pickingMode = PickingMode.Ignore,
                direction = SliderDirection.Horizontal,
            };
            _horizontalScroller.Q(className: Scroller.sliderUssClassName)?.RemoveFromHierarchy();
            _horizontalScroller.lowButton.SetAction(() => _tabButtonsScrollView.horizontalScroller.ScrollPageUp(), 0, 2);
            _horizontalScroller.highButton.SetAction(() => _tabButtonsScrollView.horizontalScroller.ScrollPageDown(), 0, 2);
            _horizontalScroller.AddToClassList(HORIZONTAL_SCROLLER_CLASS_NAME);
            
            hierarchy.Add(_tabButtonsView);
            hierarchy.Add(_tabContentContainer);
            
            UpdateButtonPanelOrientation();
            UpdateButtonPanelAlign();
            UpdateButtonPanelResizable();
            
            RegisterCallback<AttachToPanelEvent, TabView>(static (_, view) => view.RebuildTabs(), this);
            RegisterCallback<DetachFromPanelEvent, TabView>(static (_, view) => view.RebuildTabs(), this);
        }
        
        private void UpdateHorizontalScrollerVisibility()
        {
            _horizontalScroller.lowButton.SetDisplay(_tabButtonsScrollView.horizontalScroller.value != 0);
            _horizontalScroller.highButton.SetDisplay(
                _tabButtonsScrollView.horizontalScroller.value < 
                _tabButtonsScrollView.contentContainer.worldBound.width - _tabButtonsScrollView.contentViewport.layout.width);
        }
        
        private void UpdateButtonPanelOrientation()
        {
            EnableInClassList(HORIZONTAL_VARIANT_CLASS_NAME, _orientation is ScrollViewMode.Horizontal);
            EnableInClassList(VERTICAL_VARIANT_CLASS_NAME, _orientation is ScrollViewMode.Vertical);
            
            switch (_orientation)
            {
                case ScrollViewMode.Vertical:   SetVertical(); break;
                case ScrollViewMode.Horizontal: SetHorizontal(); break;
                default:
                    switch (_currentOrientation)
                    {
                        case TabButtonPanelOrientation.Vertical: SetVertical(); break;
                        default:                                 SetHorizontal(); break;
                    }
                    break;
            }

            return;
            
            void SetHorizontal()
            {
                _tabButtonsScrollView.mode = ScrollViewMode.Horizontal;
                _tabButtonsScrollView.verticalScrollerVisibility = ScrollerVisibility.Hidden;
                    
                _tabButtonsView.Add(_horizontalScroller);
            }
            
            void SetVertical()
            {
                _tabButtonsScrollView.mode = ScrollViewMode.Vertical;
                _tabButtonsScrollView.verticalScrollerVisibility = ScrollerVisibility.Auto;
                    
                _horizontalScroller.RemoveFromHierarchy();
            }
        }
        
        private void UpdateButtonPanelAlign()
        {
            if (_buttonPanelAlign is TabButtonPanelAlign.Stretch)
            {
                
            }
            
            _tabButtonsContainer.EnableInClassList(TAB_BUTTONS_ALIGN_STRETCH_VARIANT_CLASS_NAME, _buttonPanelAlign is TabButtonPanelAlign.Stretch);
            _tabButtonsContainer.EnableInClassList(TAB_BUTTONS_ALIGN_FLEX_START_VARIANT_CLASS_NAME, _buttonPanelAlign is TabButtonPanelAlign.Flex);
        }
        
        private void UpdateButtonPanelResizable()
        {
            _twoPaneSplitView.RemoveFromHierarchy();
            _tabButtonsView.RemoveFromHierarchy();
            _tabContentContainer.RemoveFromHierarchy();
            
            _tabContentContainer.style.left = new StyleLength(StyleKeyword.Null);
            _tabContentContainer.style.right = new StyleLength(StyleKeyword.Null);
            _tabButtonsView.style.width = new StyleLength(StyleKeyword.Null);
            
            if (_orientation is ScrollViewMode.Horizontal)
            {
                hierarchy.Add(_tabButtonsView);
                hierarchy.Add(_tabContentContainer);
                
                RemoveFromClassList(RESIZABLE_VARIANT_CLASS_NAME);
            }
            else if (_orientation is ScrollViewMode.Vertical)
            {
                if (_buttonPanelResizable)
                {
                    _twoPaneSplitView.Add(_tabButtonsView);
                    _twoPaneSplitView.Add(_tabContentContainer);
                    
                    hierarchy.Add(_twoPaneSplitView);
                }
                else
                {
                    hierarchy.Add(_tabButtonsView);
                    hierarchy.Add(_tabContentContainer);
                }
                
                EnableInClassList(RESIZABLE_VARIANT_CLASS_NAME, _buttonPanelResizable);
            }
        }
        
        internal void RebuildTabs()
        {
            _tabButtonsContainer.Clear();
            
            VisualElement button = null;
            foreach (var element in contentContainer.Children())
            {
                var tabContent = new TabContent(element);

                if (tabContent.GenerateButton)
                {
                    var lastButton = button;
                    button = CreateTabButton(element.name, tabContent.DisplayName);

                    if (lastButton == null)
                    {
                        button.AddToClassList(TAB_BUTTON_FIRST_CLASS_NAME);
                    }

                    _tabButtonsContainer.Add(button);
                }

                _tabSystem.Add(element.name, tabContent);
            }
            
            button?.AddToClassList(TAB_BUTTON_LAST_CLASS_NAME);
            
            if (!string.IsNullOrEmpty(_saveStateKey))
            {
                var savedTab = EditorPrefs.GetString(_saveStateKey);
                
                if (_tabSystem.TrySelect(savedTab))
                {
                    return;
                }
            }
            
            _tabSystem.SelectFirst();
        }

        private VisualElement CreateTabButton(string key, string displayName, Texture2D icon = null)
        {
            var button = new VisualElement() { name = key };
            button.AddToClassList(TAB_BUTTON_CLASS_NAME);
            
            var elementIcon = new VisualElement()
            {
                style =
                {
                    backgroundImage = new StyleBackground(icon),
                }
            };
            elementIcon.AddToClassList(TAB_BUTTON_ICON_CLASS_NAME);
            elementIcon.SetDisplay(icon);
            
            var label = new Label(displayName);
            label.AddToClassList(TAB_BUTTON_LABEL_CLASS_NAME);
            
            var clickable = new VisualElement() { tooltip = tooltip };
            clickable.AddToClassList(TAB_BUTTON_CLICKABLE_CLASS_NAME);
            
            clickable.RegisterCallback<ClickEvent, (TabSystem tabSystem, string saveStateKey)>((_, args) =>
            {
                args.tabSystem.Select(key);
                if (!string.IsNullOrEmpty(args.saveStateKey))
                {
                    EditorPrefs.SetString(args.saveStateKey, key);
                }
            }, (_tabSystem, _saveStateKey));
            
            _tabSystem.OnTabSelected += selected =>
            {
                button.EnableInClassList(TAB_BUTTON_SELECTED_CLASS_NAME, selected == key);
            };
            
            clickable.Add(elementIcon);
            clickable.Add(label);
            
            button.Add(clickable);
            
            return button;
        }

        private class TabContent : ITabContent
        {
            private readonly VisualElement _element;
            private readonly Tab _tab;
            
            public string DisplayName => _tab?.DisplayName ?? _element.name;
            public bool GenerateButton => _tab?.GenerateButton ?? true;
            
            public TabContent(VisualElement element)
            {
                _element = element;
                _tab = element as Tab;
                _element.SetDisplay(false);
            }
            
            public void OnSelected()
            {
                _element.style.opacity = 0;
                _element.SetDisplay(true);
                
                _element.experimental.animation.Start(
                    new StyleValues { opacity = 0 },
                    new StyleValues { opacity = 1 },
                    200);
            }
            
            public void OnDeselected()
            {
                _element.SetDisplay(false);
            }
        }
        
        #region UXML Factory
        
        public new class UxmlFactory : UxmlFactory<TabView, UxmlTraits> { }
        public new class UxmlTraits : VisualElement.UxmlTraits
        {
            private readonly UxmlStringAttributeDescription _saveStateKey = new() { name = "save-state-key" };
            
            private readonly UxmlBoolAttributeDescription _showBackgroundSelection = new() { name = "show-background-selection", defaultValue = true };
            private readonly UxmlBoolAttributeDescription _showSelectionLine = new() { name = "show-selection-line", defaultValue = true };
            private readonly UxmlBoolAttributeDescription _showSeparators = new() { name = "show-separators", defaultValue = true };
            
            private readonly UxmlEnumAttributeDescription<ScrollViewMode> _orientation = new() { name = "orientation" };
            
            private readonly UxmlEnumAttributeDescription<TabButtonPanelAlign> _buttonPanelAlign = new() { name = "button-panel-align" };
            private readonly UxmlBoolAttributeDescription _buttonPanelResizable = new() { name = "button-panel-resizable" };
            
            public override void Init(VisualElement ve, IUxmlAttributes bag, CreationContext cc)
            {
                base.Init(ve, bag, cc);
                
                var view = (TabView)ve;
                view._saveStateKey = _saveStateKey.GetValueFromBag(bag, cc);
                
                view.ShowBackgroundSelection = _showBackgroundSelection.GetValueFromBag(bag, cc);
                view.ShowSelectionLine = _showSelectionLine.GetValueFromBag(bag, cc);
                view.ShowSeparators = _showSeparators.GetValueFromBag(bag, cc);
                
                view.Orientation = _orientation.GetValueFromBag(bag, cc);
                
                view.ButtonPanelAlign = _buttonPanelAlign.GetValueFromBag(bag, cc);
                view.ButtonPanelResizable = _buttonPanelResizable.GetValueFromBag(bag, cc);
            }
        }
        
        #endregion
    }
}