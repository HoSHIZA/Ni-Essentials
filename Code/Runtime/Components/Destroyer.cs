using System.Runtime.CompilerServices;
using UnityEngine;

namespace NiGames.Essentials.Components
{
    [DisallowMultipleComponent]
    [DefaultExecutionOrder(64000)]
    [AddComponentMenu(MenuPath.COMPONENT_MENU_ROOT + "Destroyer")]
    public sealed class Destroyer : MonoBehaviour
    {
        [SerializeField] private Mode _mode = Mode.OnAwake;
        
        [Space]
        [Tooltip("Destroy self if empty.")]
        [SerializeField] private Object _target;
        
        private void Awake()
        {
            if (_mode != Mode.OnAwake) return;
            
            DestroyInternal();
        }
        
        private void Start()
        {
            if (_mode != Mode.OnStart) return;
            
            DestroyInternal();
        }

        [MethodImpl(256)]
        private void DestroyInternal()
        {
            Destroy(_target ? _target : gameObject);
        }
        
        private enum Mode
        {
            OnAwake,
            OnStart,
        }
    }
}