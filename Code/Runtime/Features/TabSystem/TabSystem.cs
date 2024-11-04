using System;
using System.Collections.Generic;
using System.Linq;

namespace NiGames.Essentials.Tabs
{
    public class TabSystem
    {
        private readonly Dictionary<string, ITabContent> _tabs = new();
        
        public string ActiveTabKey { get; private set; }
        
        public event Action<string> OnTabSelected;
        public event Action<string> OnTabAdded;
        public event Action<string> OnTabRemoved;
        
        public IReadOnlyDictionary<string, ITabContent> Tabs => _tabs;
        
        public ITabContent ActiveTab => ActiveTabKey != null ? _tabs[ActiveTabKey] : null;
        public int ActiveTabIndex => Array.IndexOf(_tabs.Keys.ToArray(), ActiveTabKey);
        
        public bool Has(string key)
        {
            return !string.IsNullOrEmpty(key) && _tabs.Count > 0 && _tabs.ContainsKey(key);
        }
        
        public string GetTabKeyFromIndex(int index)
        {
            return index >= _tabs.Count 
                ? null 
                : _tabs.Keys.ElementAt(index);
        }
        
        public void Add(string key, ITabContent content)
        {
            if (!_tabs.TryAdd(key, content)) return;

            OnTabAdded?.Invoke(key);
            
            if (ActiveTabKey == null)
            {
                Select(key);
            }
        }

        public void Remove(string key)
        {
            if (!Has(key)) return;

            _tabs.Remove(key);
            OnTabRemoved?.Invoke(key);

            if (ActiveTabKey == key)
            {
                Select(_tabs.Keys.FirstOrDefault());
            }
        }
        
        public bool TrySelect(string key)
        {
            try
            {
                Select(key, throwIfNotFound: true);
                
                return true;
            }
            catch (KeyNotFoundException)
            {
                return false;
            }
        }

        public void Select(string key, bool throwIfNotFound = false)
        {
            if (!Has(key) || key == ActiveTabKey)
            {
                if (throwIfNotFound)
                {
                    throw new KeyNotFoundException();
                }
                
                return;
            }

            var previousTab = ActiveTab;
            ActiveTabKey = key;
            
            previousTab?.OnDeselected();
            ActiveTab?.OnSelected();
            
            OnTabSelected?.Invoke(key);
        }

        public void SelectFirst()
        {
            Select(_tabs.Keys.FirstOrDefault());
        }

        public void SelectLast()
        {
            Select(_tabs.Keys.LastOrDefault());
        }

        public void SelectNext(bool cycle = true)
        {
            if (_tabs.Count == 0) return;
            
            var selectNext = false;
            for (var i = 0; i < _tabs.Keys.Count; i++)
            {
                var key = _tabs.Keys.ElementAt(i);
                
                if (selectNext)
                {
                    Select(key);
                    return;
                }
                
                if (key == ActiveTabKey)
                {
                    selectNext = true;
                }
            }
            
            if (cycle)
            {
                SelectFirst();
            }
        }

        public void SelectPrevious(bool cycle = true)
        {
            if (_tabs.Count == 0) return;
            
            var selectNext = false;
            for (var i = _tabs.Keys.Count; i > 0; i--)
            {
                var key = _tabs.Keys.ElementAt(i);
                
                if (selectNext)
                {
                    Select(key);
                    return;
                }
                
                if (key == ActiveTabKey)
                {
                    selectNext = true;
                }
            }
            
            if (cycle)
            {
                SelectLast();
            }
        }

        public void Clear()
        {
            _tabs.Clear();
            ActiveTabKey = null;
        }
    }
}