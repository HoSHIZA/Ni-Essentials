using System;
using UnityEngine;

namespace NiGames.Essentials.Easing
{
    /// <summary>
    /// A structure representing <c>Ease</c>.
    /// </summary>
    [Serializable]
    public struct EaseData
    {
        [SerializeField] private EaseType _easeType;
        [SerializeField] private EasePower _easePower;
        [SerializeField] private AnimationCurve _curve;
        
        private readonly Func<float, float> _function;
        
        public EaseData(Ease ease, AnimationCurve curve = null) : this()
        {
            _easeType = EaseUtility.GetEaseType(ease);
            _easePower = EaseUtility.GetEasePower(ease);
            _curve = curve;

            if (_easeType == EaseType.Custom && _curve == null)
            {
                _function = EaseUtility.GetFunction(ease);
            }
        }
        
        public EaseData(Func<float, float> func) : this()
        {
            _easeType = EaseType.Custom;
            _function = func;
        }
        
        public EaseData(AnimationCurve curve) : this()
        {
            _easeType = EaseType.Custom;
            _curve = curve;
        }
        
        /// <summary>
        /// Evaluates <c>t</c> with the selected <c>ease</c>.
        /// </summary>
        public float Evaluate(float t)
        {
            if (_easeType == EaseType.Custom)
            {
                return _function?.Invoke(t) ?? (_curve?.Evaluate(t) ?? EaseFunction.Linear(t));
            }
            
            if (_easePower == EasePower.Linear)
            {
                return EaseUtility.Evaluate(t, Ease.Linear);
            }
            
            return EaseUtility.Evaluate(t, _easeType, _easePower);
        }
        
        public static implicit operator EaseData(Ease ease)
        {
            return new EaseData(ease);
        }
        
        public static implicit operator EaseData(Func<float, float> func)
        {
            return new EaseData(func);
        }
        
        public static implicit operator EaseData(AnimationCurve curve)
        {
            return new EaseData(curve);
        }

        public static EaseData Default = new(Ease.Linear);
    }
}