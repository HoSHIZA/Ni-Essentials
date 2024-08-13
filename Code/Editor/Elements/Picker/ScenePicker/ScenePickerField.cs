using System;
using System.Linq;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;
using Object = UnityEngine.Object;

namespace NiGames.Essentials.Editor
{
#if UNITY_2023_2_OR_NEWER
    [UxmlElement("ScenePickerField")]
#endif
    // ReSharper disable once PartialTypeWithSinglePart
    public partial class ScenePickerField : BasePickerField<string>
    {
        #region Constructors
        
        public ScenePickerField() : this(null)
        {
        }
        
        public ScenePickerField(string label) : base(label, new ScenePickerControl())
        {
            AddToClassList(USS_CLASS_NAME);
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
                if (property.propertyType == SerializedPropertyType.String)
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
            
            static string Getter(SerializedProperty prop)
            {
                return prop.stringValue;
            }
            
            static void Setter(SerializedProperty prop, string v)
            {
                Undo.RegisterCompleteObjectUndo(prop.serializedObject.targetObject, "Change scene");
                
                prop.serializedObject.Update();
                prop.stringValue = v;
                prop.serializedObject.ApplyModifiedProperties();
                
                Undo.FlushUndoRecordObjects();
            }
            
            static bool Comparer(string value, SerializedProperty prop, Func<SerializedProperty, string> getter)
            {
                return string.Equals(value, getter.Invoke(prop), StringComparison.Ordinal);
            }
        }
        
        private class SceneProvider : PickerProvider<string>
        {
        }
        
        private class ScenePickerControl : PickerControl, IDragReceiver
        {
            public string Value { get; private set; }
            
            private readonly string[] _scenes;
            
            private static Texture _sceneAssetIcon;
            
            static ScenePickerControl()
            {
                _sceneAssetIcon = AssetPreview.GetMiniTypeThumbnail(typeof(SceneAsset));
            }
            
            public ScenePickerControl()
            {
                _scenes = new string[SceneManager.sceneCountInBuildSettings];
                for (var i = 0; i < _scenes.Length; i++)
                {
                    _scenes[i] = SceneManager.GetSceneByBuildIndex(i).name;
                }
                
                var paths = _scenes.Prepend(NONE);
                var items = _scenes.Prepend(null);
                
                Provider = ScriptableObject.CreateInstance<SceneProvider>();
                Provider.Setup("Scenes", paths, items, GetIcon, OnSelected);
                
                SetLabel(_sceneAssetIcon, GetLabelText());
                
                this.MakeDragReceiver();
            }
            
            public override void SetValueWithoutNotify(string newValue)
            {
                if (Value == newValue) return;
                
                if (string.IsNullOrEmpty(newValue) || _scenes.Contains(newValue))
                {
                    Value = newValue;
                    
                    SetLabel(_sceneAssetIcon, GetLabelText());
                }
                else
                {
                    Debug.LogWarning($"[{nameof(ScenePickerField)}]{(string.IsNullOrEmpty(name) ? "" : $"({name})")} Scene '{newValue}' not found in build settings");
                }
            }
            
            #region IDrawReceiver Impl
            
            public bool IsDragValid(Object[] objects, object data)
            {
                if (objects.Length <= 0) return false;
                
                return objects[0] as SceneAsset ?? false;
            }
            
            public void AcceptDrag(Object[] objects, object data)
            {
                if (objects[0] is not SceneAsset scene) return;
                if (!_scenes.Contains(scene.name)) return;
                
                OnSelected(scene.name);
            }
            
            #endregion
            
            private static Texture GetIcon(string s)
            {
                return _sceneAssetIcon ??= AssetPreview.GetMiniTypeThumbnail(typeof(SceneAsset));
            }
            
            private string GetLabelText()
            {
                return string.IsNullOrEmpty(Value) 
                    ? "None (Scene)" 
                    : $"{Value} ({SceneManager.GetSceneByName(Value).buildIndex.ToString()})";
            }
            
            private void OnSelected(string selected)
            {
                if (Value == selected) return;
                
                this.SendChangeEvent(Value, selected);
            }
        }
        
        #region UXML Factory
        
#if !UNITY_2023_2_OR_NEWER
        public new class UxmlFactory : UxmlFactory<ScenePickerField, UxmlTraits> { }
        public new class UxmlTraits : BaseFieldTraits<string, UxmlStringAttributeDescription>
        {
            public override void Init(VisualElement element, IUxmlAttributes bag, CreationContext cc)
            {
                base.Init(element, bag, cc);
            }
        }
#endif

        #endregion
    }
}