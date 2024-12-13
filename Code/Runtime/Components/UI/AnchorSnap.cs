using JetBrains.Annotations;
using UnityEngine;

namespace NiGames.Essentials.Components
{
    /// <summary>
    /// A component that provides functionality to snap a RectTransform's anchor automatically.
    /// </summary>
    [DisallowMultipleComponent]
    [RequireComponent(typeof(RectTransform))]
    [AddComponentMenu(Constants.Menu.Component.UI + "Anchor Snap")]
    public sealed class AnchorSnap : MonoBehaviour
    {
        [Tooltip("Container in which the alignment will take place.")] [SerializeField]
        private RectTransform _container;

        private RectTransform _rt;

        private void Awake()
        {
            _rt = (RectTransform)transform;
            _container ??= (RectTransform)transform.parent;
        }

        /// <summary>
        /// Automatically calculates the anchor snap based on the current local position within the container.
        /// </summary>
        [PublicAPI]
        public void Snap()
        {
            var absolutePosition = CalculateAbsolutePosition(_rt);
            var normalizedX = absolutePosition.x / _container.rect.size.x + _container.pivot.x;
            var normalizedY = absolutePosition.y / _container.rect.size.y + _container.pivot.y;

            var anchorX = CalculateAnchorValue(normalizedX);
            var anchorY = CalculateAnchorValue(normalizedY);
            var anchor = new Vector2(anchorX, anchorY);

            var worldPosition = _rt.position;

            _rt.anchorMin = anchor;
            _rt.anchorMax = anchor;

            _rt.position = worldPosition;
        }
        
        private static Vector2 CalculateAbsolutePosition(RectTransform rt)
        {
            var pivotOffset = new Vector2(rt.rect.width * (rt.pivot.x - 0.5f), rt.rect.height * (rt.pivot.y - 0.5f));

            return (Vector2)rt.localPosition - pivotOffset;
        }
        
        private static float CalculateAnchorValue(float v) => v switch
        {
            < 0.33f => 0f,
            >= 0.66f => 1f,
            _ => 0.5f,
        };
    }
}