using UnityEngine;

namespace NiGames.Essentials.Components
{
    [DisallowMultipleComponent]
    [DefaultExecutionOrder(64000)]
    [AddComponentMenu(Constants.Menu.Component.MISCELLANEOUS + "DontDestroyOnLoad")]
    public sealed class DontDestroyOnLoad : MonoBehaviour
    {
        private void Awake()
        {
            DontDestroyOnLoad(gameObject);
        }
    }
}