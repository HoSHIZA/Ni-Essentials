using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace NiGames.Essentials.Components.Layout
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(RectTransform))]
    [AddComponentMenu(Constants.Menu.Component.UI + "Layout/Flow Layout Group")]
    public class FlowLayoutGroup : LayoutGroup
    {
        [SerializeField] private bool _childForceExpandHeight;
        [SerializeField] private bool _childForceExpandWidth;
        [SerializeField] private float _spacing;
        
        private readonly List<RectTransform> _currentRow = new();
        private float _totalLayoutHeight;

        /// <summary>
        /// Gets or sets whether children heights should be forced to the parent's height
        /// </summary>
        public bool ChildForceExpandHeight
        {
            get => _childForceExpandHeight;
            set
            {
                _childForceExpandHeight = value;
                SetDirty();
            }
        }

        /// <summary>
        /// Gets or sets whether children widths should be forced to fill the parent's width
        /// </summary>
        public bool ChildForceExpandWidth
        {
            get => _childForceExpandWidth;
            set
            {
                _childForceExpandWidth = value;
                SetDirty();
            }
        }

        /// <summary>
        /// Gets or sets the spacing between elements
        /// </summary>
        public float Spacing
        {
            get => _spacing;
            set
            {
                _spacing = value;
                SetDirty();
            }
        }

        private bool IsCenterAlign => childAlignment is TextAnchor.LowerCenter or TextAnchor.MiddleCenter or TextAnchor.UpperCenter;
        private bool IsRightAlign => childAlignment is TextAnchor.LowerRight or TextAnchor.MiddleRight or TextAnchor.UpperRight;
        private bool IsMiddleAlign => childAlignment is TextAnchor.MiddleLeft or TextAnchor.MiddleRight or TextAnchor.MiddleCenter;
        private bool IsLowerAlign => childAlignment is TextAnchor.LowerLeft or TextAnchor.LowerRight or TextAnchor.LowerCenter;

        public override void CalculateLayoutInputHorizontal()
        {
            base.CalculateLayoutInputHorizontal();
            
            var width = CalculateMinimumWidth();
            
            SetLayoutInputForAxis(width, -1, -1, 0);
        }

        public override void CalculateLayoutInputVertical()
        {
            _totalLayoutHeight = ArrangeElements(1, true);
        }

        public override void SetLayoutHorizontal()
        {
            ArrangeElements(0, false);
        }

        public override void SetLayoutVertical()
        {
            ArrangeElements(1, false);
        }

        private float ArrangeElements(int axis, bool isCalculatingLayout)
        {
            var rect = rectTransform.rect;
            var availableWidth = rect.width - padding.horizontal;
            var verticalOffset = (float)(IsLowerAlign ? padding.bottom : padding.top);
            
            var rowInfo = new RowInfo();

            for (var i = 0; i < rectChildren.Count; i++)
            {
                var childIndex = IsLowerAlign ? rectChildren.Count - 1 - i : i;
                var child = rectChildren[childIndex];

                ProcessChild(child, availableWidth, ref rowInfo, ref verticalOffset, axis, isCalculatingLayout);
            }

            if (_currentRow.Count > 0)
            {
                LayoutFinalRow(rowInfo, availableWidth, verticalOffset, axis, isCalculatingLayout);
            }

            verticalOffset += IsLowerAlign ? padding.top : padding.bottom;

            if (isCalculatingLayout && axis == 1)
            {
                SetLayoutInputForAxis(verticalOffset, verticalOffset, -1, axis);
            }

            return verticalOffset;
        }

        private void ProcessChild(RectTransform child, float availableWidth, ref RowInfo rowInfo, ref float verticalOffset, int axis, bool isCalculatingLayout)
        {
            var childWidth = LayoutUtility.GetPreferredSize(child, 0);
            var childHeight = LayoutUtility.GetPreferredSize(child, 1);

            childWidth = Mathf.Min(childWidth, availableWidth);

            if (_currentRow.Count > 0)
            {
                rowInfo.Width += _spacing;
            }

            if (rowInfo.Width + childWidth > availableWidth)
            {
                LayoutRow(rowInfo, availableWidth, verticalOffset, axis, isCalculatingLayout);
                verticalOffset += rowInfo.Height + _spacing;
                rowInfo = new RowInfo();
            }

            rowInfo.Width += childWidth;
            _currentRow.Add(child);
            rowInfo.Height = Mathf.Max(rowInfo.Height, childHeight);
        }

        private void LayoutFinalRow(RowInfo rowInfo, float availableWidth, float verticalOffset, int axis, bool isCalculatingLayout)
        {
            if (!isCalculatingLayout)
            {
                var finalOffset = CalculateRowVerticalOffset(rectTransform.rect.height, verticalOffset, rowInfo.Height);
                LayoutRow(rowInfo, availableWidth, finalOffset, axis, false);
            }
            
            _currentRow.Clear();
        }

        private float CalculateRowVerticalOffset(float containerHeight, float currentOffset, float rowHeight)
        {
            if (IsLowerAlign)
            {
                return containerHeight - currentOffset - rowHeight;
            }

            if (IsMiddleAlign)
            {
                return containerHeight * 0.5f - _totalLayoutHeight * 0.5f + currentOffset;
            }
            
            return currentOffset;
        }

        private void LayoutRow(RowInfo rowInfo, float maxWidth, float yOffset, int axis, bool isCalculatingLayout)
        {
            if (isCalculatingLayout)
            {
                _currentRow.Clear();
                return;
            }

            var xOffset = CalculateRowHorizontalOffset(rowInfo.Width, maxWidth);
            var extraWidth = CalculateExtraWidth(maxWidth - rowInfo.Width);

            ArrangeChildrenInRow(xOffset, yOffset, rowInfo.Height, maxWidth, extraWidth, axis);
            _currentRow.Clear();
        }

        private float CalculateRowHorizontalOffset(float rowWidth, float maxWidth)
        {
            if (_childForceExpandWidth) return padding.left;
            
            if (IsCenterAlign)
            {
                return padding.left + (maxWidth - rowWidth) * 0.5f;
            }

            if (IsRightAlign)
            {
                return padding.left + (maxWidth - rowWidth);
            }
            
            return padding.left;
        }

        private float CalculateExtraWidth(float remainingWidth)
        {
            if (!_childForceExpandWidth) return 0;

            var flexibleChildCount = _currentRow.Count(child => LayoutUtility.GetFlexibleWidth(child) > 0f);
            
            return flexibleChildCount > 0 ? remainingWidth / flexibleChildCount : 0;
        }

        private void ArrangeChildrenInRow(float xPos, float yOffset, float rowHeight, float maxWidth, float extraWidth, int axis)
        {
            for (var j = 0; j < _currentRow.Count; j++)
            {
                var index = IsLowerAlign ? _currentRow.Count - 1 - j : j;
                var child = _currentRow[index];

                var childWidth = CalculateChildWidth(child, extraWidth, maxWidth);
                var childHeight = CalculateChildHeight(child, rowHeight);
                var childYPos = CalculateChildYPosition(yOffset, rowHeight, childHeight);

                if (axis == 0) SetChildAlongAxis(child, 0, xPos, childWidth);
                else SetChildAlongAxis(child, 1, childYPos, childHeight);

                xPos += childWidth + _spacing;
            }
        }

        private float CalculateChildWidth(RectTransform child, float extraWidth, float maxWidth)
        {
            var width = LayoutUtility.GetPreferredSize(child, 0);
            
            if (LayoutUtility.GetFlexibleWidth(child) > 0f)
            {
                width += extraWidth;
            }
            
            return Mathf.Min(width, maxWidth);
        }

        private float CalculateChildHeight(RectTransform child, float rowHeight)
        {
            return _childForceExpandHeight ? rowHeight : LayoutUtility.GetPreferredSize(child, 1);
        }

        private float CalculateChildYPosition(float baseOffset, float rowHeight, float childHeight)
        {
            if (IsMiddleAlign)
            {
                return baseOffset + (rowHeight - childHeight) * 0.5f;
            }

            if (IsLowerAlign)
            {
                return baseOffset + (rowHeight - childHeight);
            }
            
            return baseOffset;
        }

        private float CalculateMinimumWidth()
        {
            if (rectChildren.Count == 0) return 0f;
            
            return rectChildren
                .Select(LayoutUtility.GetMinWidth)
                .Max() + padding.horizontal;
        }

        private struct RowInfo
        {
            public float Width;
            public float Height;
        }
    }
}