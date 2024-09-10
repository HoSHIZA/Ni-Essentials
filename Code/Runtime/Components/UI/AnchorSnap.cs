using UnityEngine;

namespace NiGames.Essentials.Components
{
    /// <summary>
    /// A component that provides functionality to snap a RectTransform's anchor to a predefined grid.
    /// </summary>
    /// <remarks>
    /// This component creates a predefined grid of segments based on a specified number of rows and columns.
    /// The anchor of the RectTransform can then be snapped to one of the grid segments using the <see cref="Snap"/> method.
    /// </remarks>
    [DisallowMultipleComponent]
    [RequireComponent(typeof(RectTransform))]
    [AddComponentMenu(MenuPath.COMPONENT_MENU_ROOT_UI + "Anchor Snap")]
    public sealed class AnchorSnap : MonoBehaviour
    {
        [Tooltip("Number of equal segments in the grid. It is used for the accuracy of the setting.")]
        [SerializeField] private int _segmentCount = 3;
        
        [Tooltip("Container in which the alignment will take place.")]
        [SerializeField] private RectTransform _container;
        
        private Segment[] _segments;
        
        private RectTransform _rectTransform;
        private RectTransform RectTransform => _rectTransform ? _rectTransform : _rectTransform = (RectTransform)transform;
        
        private void Start()
        {
            _container = _container ? _container : (RectTransform)transform.parent;
            
            Calculate();
        }
        
        private void OnRectTransformDimensionsChange()
        {
            Calculate();
        }
        
        /// <summary>
        /// Calculates the grid of segments based on the number of segments and container RectTransform.
        /// </summary>
        public void Calculate()
        {
            _container.pivot = Vector2.zero;

            var totalSegments = _segmentCount * _segmentCount;
            var segmentWidth = _container.rect.width / _segmentCount;
            var segmentHeight = _container.rect.height / _segmentCount;
            
            if (_segments.Length != totalSegments)
            {
                _segments = new Segment[totalSegments];
            }
            
            for (var i = 0; i < totalSegments; i++)
            {
                var column = i % _segmentCount;
                var row = i / _segmentCount;
                
                var rect = new Rect(segmentWidth * column, segmentHeight * row, segmentWidth, segmentHeight);
                
                var anchor = new Vector2(
                    column == 0 ? 0f : column == _segmentCount - 1 ? 1f : 0.5f,
                    row == 0 ? 0f : row == _segmentCount - 1 ? 1f : 0.5f
                );

                _segments[i] = new Segment(rect, anchor);
            }
        }
        
        /// <summary>
        /// Snaps the anchor of the RectTransform to the closest segment in the grid.
        /// </summary>
        public void Snap()
        {
            var localPosition = RectTransform.localPosition;
            
            foreach (var segment in _segments)
            {
                var rect = segment.Rect;
                
                if (!rect.Contains(localPosition)) continue;
                
                RectTransform.anchorMin = segment.Anchor;
                RectTransform.anchorMax = segment.Anchor;
                RectTransform.localPosition = localPosition;
                    
                break;
            }
        }
        
        /// <summary>
        /// Struct that represents a segment in the grid.
        /// </summary>
        public readonly struct Segment
        {
            public readonly Rect Rect;
            public readonly Vector2 Anchor;

            public Segment(Rect rect, Vector2 anchor)
            {
                Rect = rect;
                Anchor = anchor;
            }
        }
    }
}