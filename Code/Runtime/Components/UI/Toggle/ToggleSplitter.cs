using UnityEngine;
using UnityEngine.UI;

namespace NiGames.Essentials.Components
{
    [RequireComponent(typeof(Toggle))]
    [AddComponentMenu(Constants.Menu.Component.UI + "Toggle/Toggle Splitter")]
    public class ToggleSplitter : MonoBehaviour
    {
        [SerializeField] private Toggle.ToggleEvent _on = new();
        [SerializeField] private Toggle.ToggleEvent _off = new();

        private Toggle _toggle;

        private void Awake()
        {
            _toggle = GetComponent<Toggle>();
            _toggle.onValueChanged.AddListener(v => UpdateValue(v));
        }

        public void UpdateValue(bool isOn)
        {
            if (isOn)
            {
                _on?.Invoke(true);
            }
            else
            {
                _off?.Invoke(false);
            }
        }

#if UNITY_EDITOR
        private void OnValidate()
        {
            if (_toggle) return;
            
            _toggle = GetComponent<Toggle>();
            _toggle.onValueChanged.AddListener(UpdateValue);
        }
#endif
    }
}