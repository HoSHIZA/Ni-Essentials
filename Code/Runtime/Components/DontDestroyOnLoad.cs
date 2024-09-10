using UnityEngine;

namespace NiGames.Essentials.Components
{
    [DisallowMultipleComponent]
    [DefaultExecutionOrder(64000)]
    public sealed class DontDestroyOnLoad : MonoBehaviour
    {
        private void Awake()
        {
            DontDestroyOnLoad(gameObject);
        }
    }
}