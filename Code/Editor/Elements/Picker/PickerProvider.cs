using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

namespace NiGames.Essentials.Editor
{
    public class PickerProvider<T> : ScriptableObject, ISearchWindowProvider 
        where T : class
    {
        private Texture2D _emptyIcon;
        private string _title;
        
        private string[] _paths;
        private T[] _items;
        private Func<T, Texture> _getIcon;
        private Action<T> _onSelected;
        
        private void OnEnable()
        {
            _emptyIcon = new Texture2D(1, 1);
            _emptyIcon.SetPixel(0, 0, new Color(0, 0, 0, 0));
            _emptyIcon.Apply();
        }
        
        private void OnDisable()
        {
            DestroyImmediate(_emptyIcon);
        }
        
        public void Setup(string title, string[] paths, T[] items, Func<T, Texture> getIcon, Action<T> onSelected)
        {
            _title = title;
            _paths = paths;
            _items = items;
            _getIcon = getIcon;
            _onSelected = onSelected;
        }
        
        public void Setup(string title, IEnumerable<string> paths, IEnumerable<T> items, 
            Func<T, Texture> getIcon, Action<T> onSelected)
        {
            _title = title;
            _paths = paths.ToArray();
            _items = items.ToArray();
            _getIcon = getIcon;
            _onSelected = onSelected;
        }
        
        public List<SearchTreeEntry> CreateSearchTree(SearchWindowContext context)
        {
            var tree = new List<SearchTreeEntry>
            {
                new SearchTreeGroupEntry(new GUIContent(_title), 0)
            };

            if (_paths == null || _items == null) return tree;
            
            var groups = new List<string>();

            for (var index = 0; index < _paths.Length && index < _items.Length; index++)
            {
                var path = _paths[index];
                var item = _items[index];
                
                var icon = _getIcon.Invoke(item);
                
                var menus = path.Split('/');
                var createIndex = -1;
                
                for (var i = 0; i < menus.Length - 1; i++)
                {
                    var group = menus[i];
                    if (i >= groups.Count)
                    {
                        createIndex = i;
                        break;
                    }
                    
                    if (groups[i] == group) continue;
                    
                    groups.RemoveRange(i, groups.Count - i);
                    createIndex = i;
                    
                    break;
                }
                
                if (createIndex >= 0)
                {
                    for (var i = createIndex; i < menus.Length - 1; i++)
                    {
                        var group = menus[i];
                        groups.Add(group);
                        tree.Add(new SearchTreeGroupEntry(new GUIContent(group))
                        {
                            level = i + 1,
                        });
                    }
                }
                
                tree.Add(new SearchTreeEntry(new GUIContent(menus.Last(), icon == null ? _emptyIcon : icon))
                {
                    level = menus.Length, 
                    userData = item,
                });
            }

            return tree;
        }

        public bool OnSelectEntry(SearchTreeEntry searchTreeEntry, SearchWindowContext context)
        {
            _onSelected?.Invoke(searchTreeEntry.userData as T);
            
            return true;
        }
    }
}