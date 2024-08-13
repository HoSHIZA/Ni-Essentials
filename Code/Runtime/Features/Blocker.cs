using System;
using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Object = UnityEngine.Object;

namespace NiGames.Essentials
{
    public sealed class Blocker : IDisposable
    {
        private readonly GameObject _object;
        private readonly Image _image;
        
        private float _colorTransitionTime;
        
        /// <summary>
        /// Creates a UI blocker that covers a given canvas content.
        /// </summary>
        /// <param name="rootCanvas">The root canvas.</param>
        /// <param name="content">The content to be blocked.</param>
        /// <param name="onClickBlocker">The action to be executed when the blocker is clicked. Default is null.</param>
        /// <param name="color">The color of the blocker. Default is transparent.</param>
        /// <param name="colorTransitionTime">The duration of the color transition. Default is 0 seconds.</param>
        public Blocker(Canvas rootCanvas, RectTransform content, 
            Action onClickBlocker = null, Color color = default, float colorTransitionTime = 0f)
        {
            var blocker = new GameObject("Blocker");
            
            var rt = blocker.AddComponent<RectTransform>();
            rt.SetParent(rootCanvas.transform, false);
            rt.anchorMin = new Vector2(0, 0);
            rt.anchorMax = new Vector2(1, 1);
            rt.sizeDelta = Vector2.zero;
            
            var blockerCanvas = blocker.AddComponent<Canvas>();
            var contentCanvas = content.GetComponent<Canvas>();
            blockerCanvas.overrideSorting = true;
            blockerCanvas.sortingLayerID = contentCanvas.sortingLayerID;
            blockerCanvas.sortingOrder = contentCanvas.sortingOrder - 1;
            
            var parentCanvas = content.parent.GetComponentInParent<Canvas>();
            if (parentCanvas)
            {
                var components = parentCanvas.GetComponents<BaseRaycaster>();
                foreach (var raycaster in components)
                {
                    if (!blocker.TryGetComponent(raycaster.GetType(), out _))
                    {
                        blocker.AddComponent(raycaster.GetType());
                    }
                }
            }
            else
            {
                if (!blocker.TryGetComponent(out GraphicRaycaster _))
                {
                    blocker.AddComponent<GraphicRaycaster>();
                }
            }
            
            var image = blocker.AddComponent<Image>();
            image.color = Color.clear;
            if (color != Color.clear)
            {
                if (colorTransitionTime > 0)
                {
                    image.StartCoroutine(FadeColor(image, color, colorTransitionTime));
                }
                else
                {
                    image.color = color;
                }
            }
            
            if (onClickBlocker != null)
            {
                blocker.AddComponent<Button>().onClick.AddListener(() => onClickBlocker.Invoke());
            }
            
            _object = blocker;
            _image = image;
            _colorTransitionTime = colorTransitionTime;
        }
        
        /// <summary>
        /// Destroys the created blocker.
        /// </summary>
        public void Dispose()
        {
            if (_colorTransitionTime > 0)
            {
                _image.StartCoroutine(FadeColor(_image, Color.clear, _colorTransitionTime));

                Object.Destroy(_object, _colorTransitionTime);
            }
            else
            {
                Object.Destroy(_object);
            }
        }
        
        /// <summary>
        /// Destroys the created blocker.
        /// </summary>
        public void Dispose(float colorTransitionTime)
        {
            _colorTransitionTime = colorTransitionTime;
            Dispose();
        }
        
        private static IEnumerator FadeColor(Graphic graphic, Color targetColor, float duration)
        {
            var startColor = graphic.color;
            var elapsedTime = 0f;
        
            while (elapsedTime < duration)
            {
                var t = elapsedTime / duration;
                graphic.color = Color.Lerp(startColor, targetColor, t);
                elapsedTime += Time.unscaledDeltaTime;

                yield return null;
            }

            graphic.color = targetColor;
        }
    }
}