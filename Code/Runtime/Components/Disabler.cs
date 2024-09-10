using System.Runtime.CompilerServices;
using UnityEngine;

namespace NiGames.Essentials.Components
{
    [DisallowMultipleComponent]
    [DefaultExecutionOrder(64000)]
    [AddComponentMenu(MenuPath.COMPONENT_MENU_ROOT + "Disabler")]
    public sealed class Disabler : MonoBehaviour
    {
        [SerializeField] private Mode _mode = Mode.OnAwake;
        
        [Space]
        [Tooltip("Disables self if empty.")]
        [SerializeField] private Object _target;
        
        private void Awake()
        {
            if (_mode != Mode.OnAwake) return;

            DisableInternal();
        }
        
        private void Start()
        {
            if (_mode != Mode.OnStart) return;

            DisableInternal();
        }
        
        [MethodImpl(256)]
        private void DisableInternal()
        {
            if (_target)
            {
                switch (_target)
                {
                    case Behaviour behaviour:
                        behaviour.enabled = false;
                        break;
                    case GameObject go:
                        go.SetActive(false);
                        break;
                }
            }
            else
            {
                gameObject.SetActive(false);
            }
        }
        
        private enum Mode
        {
            OnAwake,
            OnStart,
        }
    }
}