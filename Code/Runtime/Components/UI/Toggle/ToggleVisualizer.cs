using System;
using UnityEngine;
using UnityEngine.UI;

namespace NiGames.Essentials.Components
{
    [RequireComponent(typeof(Toggle))]
    [AddComponentMenu(Constants.Menu.Component.UI + "Toggle/Toggle Visualize")]
    public class ToggleVisualizer : MonoBehaviour
    {
        [SerializeField] private bool _setPreferredTargetGraphic;
        
        [Space]
        [SerializeField] private GameObject _on;
        [SerializeField] private GameObject _off;
        
        private Toggle _toggle;
        private Image _onImage;
        private Image _offImage;
        
        public GameObject OnObject => _on;
        public GameObject OffObject => _off;
        
        public Image OnObjectImage => _onImage ? _onImage : _onImage = _on.GetComponent<Image>();
        public Image OffObjectImage => _offImage ? _offImage : _offImage = _off.GetComponent<Image>();
        public Image CurrentObjectImage => _toggle.isOn ? OnObjectImage : OffObjectImage;
        
        private void Awake()
        {
            if (!_on)
            {
                throw new NullReferenceException("On object cannot be null");
            }
            
            if (!_off)
            {
                throw new NullReferenceException("Off object cannot be null");
            }
            
            _toggle = GetComponent<Toggle>();
            
            UpdateDisplay(_toggle.isOn);
            
            _toggle.onValueChanged.AddListener(v => UpdateDisplay(v));
        }
        
        public void UpdateDisplay(bool isOn)
        {
            _on.SetActive(isOn);
            _off.SetActive(!isOn);

            var current = CurrentObjectImage;
            if (_setPreferredTargetGraphic && current)
            {
                _toggle.targetGraphic = current;
                _toggle.graphic = current;
            }
        }
        
#if UNITY_EDITOR
        private void OnValidate()
        {
            if (_toggle) return;
            if (!_on || !_off) return;
            
            _toggle = GetComponent<Toggle>();
            _toggle.onValueChanged.AddListener(UpdateDisplay);
        }
#endif
    }
}