using System;
using System.Runtime.CompilerServices;
using NiGames.Essentials.Unsafe;

namespace NiGames.Essentials.Easing
{
    public static class EaseUtility
    {
        /// <summary>
        /// Evaluates <c>t</c> with the given <c>ease</c>.
        /// </summary>
        [MethodImpl(256)]
        public static float Evaluate(float t, Ease ease)
        {
            return GetFunction(ease).Invoke(t);
        }
        
        /// <summary>
        /// Evaluates <c>t</c> with the given ease <c>type</c> and <c>power</c>.
        /// </summary>
        [MethodImpl(256)]
        public static float Evaluate(float t, EaseType type, EasePower power)
        {
            return GetFunction(type, power).Invoke(t);
        }
        
        /// <summary>
        /// Get the function corresponding to <c>ease</c>, if <c>ease</c> is equal to <c>Ease.Custom</c>, returns <c>customFunc</c>.
        /// </summary>
        [MethodImpl(256)]
        public static Func<float, float> GetFunction(Ease ease, Func<float, float> customFunc)
        {
            return ease is Ease.Custom 
                ? customFunc 
                : GetFunction(ease);
        }
        
        /// <summary>
        /// Get an unsafe delegate pointer corresponding to <c>ease</c>.
        /// </summary>
        [MethodImpl(256)]
        public static unsafe delegate*<float, float> GetFunctionPtr(Ease ease)
        {
            return GetFunction(ease).Method.MethodHandle.GetFunctionPointer().ToDelegate<float, float>();
        }
        
        /// <summary>
        /// Get an unsafe delegate pointer corresponding to ease <c>type</c> and <c>power</c>.
        /// </summary>
        [MethodImpl(256)]
        public static unsafe delegate*<float, float> GetFunctionPtr(EaseType type, EasePower power)
        {
            return GetFunction(type, power).Method.MethodHandle.GetFunctionPointer().ToDelegate<float, float>();
        }
        
        /// <summary>
        /// Get the function corresponding to <c>ease</c>.
        /// </summary>
        public static Func<float, float> GetFunction(Ease ease)
        {
            switch (ease)
            {
                case Ease.Linear:            return EaseFunction.Linear;
                case Ease.InQuad:            return EaseFunction.InQuad;
                case Ease.OutQuad:           return EaseFunction.OutQuad;
                case Ease.InOutQuad:         return EaseFunction.InOutQuad;
                case Ease.InCubic:           return EaseFunction.InCubic;
                case Ease.OutCubic:          return EaseFunction.OutCubic;
                case Ease.InOutCubic:        return EaseFunction.InOutCubic;
                case Ease.InQuart:           return EaseFunction.InQuart;
                case Ease.OutQuart:          return EaseFunction.OutQuart;
                case Ease.InOutQuart:        return EaseFunction.InOutQuart;
                case Ease.InQuint:           return EaseFunction.InQuint;
                case Ease.OutQuint:          return EaseFunction.OutQuint;
                case Ease.InOutQuint:        return EaseFunction.InOutQuint;
                case Ease.InSine:            return EaseFunction.InSine;
                case Ease.OutSine:           return EaseFunction.OutSine;
                case Ease.InOutSine:         return EaseFunction.InOutSine;
                case Ease.InExpo:            return EaseFunction.InExpo;
                case Ease.OutExpo:           return EaseFunction.OutExpo;
                case Ease.InOutExpo:         return EaseFunction.InOutExpo;
                case Ease.InCirc:            return EaseFunction.InCirc;
                case Ease.OutCirc:           return EaseFunction.OutCirc;
                case Ease.InOutCirc:         return EaseFunction.InOutCirc;
                case Ease.InBack:            return EaseFunction.InBack;
                case Ease.OutBack:           return EaseFunction.OutBack;
                case Ease.InOutBack:         return EaseFunction.InOutBack;
                case Ease.InElastic:         return EaseFunction.InElastic;
                case Ease.OutElastic:        return EaseFunction.OutElastic;
                case Ease.InOutElastic:      return EaseFunction.InOutElastic;
                case Ease.InBounce:          return EaseFunction.InBounce;
                case Ease.OutBounce:         return EaseFunction.OutBounce;
                case Ease.InOutBounce:       return EaseFunction.InOutBounce;
                default:                     return EaseFunction.Linear;
            }
        }
        
        /// <summary>
        /// Get the function corresponding to ease <c>type</c> and <c>power</c>.
        /// </summary>
        public static Func<float, float> GetFunction(EaseType type, EasePower power)
        {
            if (type is EaseType.Custom)     return EaseFunction.Linear;
            
            switch (power)
            {
                case EasePower.Linear:       return EaseFunction.Linear;
                case EasePower.Quad:
                    switch (type)
                    {
                        case EaseType.In:    return EaseFunction.InQuad;
                        case EaseType.Out:   return EaseFunction.OutQuad;
                        case EaseType.InOut: return EaseFunction.InOutQuad;
                    } break;
                case EasePower.Cubic:
                    switch (type)
                    {
                        case EaseType.In:    return EaseFunction.InCubic;
                        case EaseType.Out:   return EaseFunction.OutCubic;
                        case EaseType.InOut: return EaseFunction.InOutCubic;
                    } break;
                case EasePower.Quart:
                    switch (type)
                    {
                        case EaseType.In:    return EaseFunction.InQuart;
                        case EaseType.Out:   return EaseFunction.OutQuart;
                        case EaseType.InOut: return EaseFunction.InOutQuart;
                    } break;
                case EasePower.Quint:
                    switch (type)
                    {
                        case EaseType.In:    return EaseFunction.InQuint;
                        case EaseType.Out:   return EaseFunction.OutQuint;
                        case EaseType.InOut: return EaseFunction.InOutQuint;
                    } break;
                case EasePower.Sine:
                    switch (type)
                    {
                        case EaseType.In:    return EaseFunction.InSine;
                        case EaseType.Out:   return EaseFunction.OutSine;
                        case EaseType.InOut: return EaseFunction.InOutSine;
                    } break;
                case EasePower.Expo:
                    switch (type)
                    {
                        case EaseType.In:    return EaseFunction.InExpo;
                        case EaseType.Out:   return EaseFunction.OutExpo;
                        case EaseType.InOut: return EaseFunction.InOutExpo;
                    } break;
                case EasePower.Circ:
                    switch (type)
                    {
                        case EaseType.In:    return EaseFunction.InCirc;
                        case EaseType.Out:   return EaseFunction.OutCirc;
                        case EaseType.InOut: return EaseFunction.InOutCirc;
                    } break;
                case EasePower.Back:
                    switch (type)
                    {
                        case EaseType.In:    return EaseFunction.InBack;
                        case EaseType.Out:   return EaseFunction.OutBack;
                        case EaseType.InOut: return EaseFunction.InOutBack;
                    } break;
                case EasePower.Elastic:
                    switch (type)
                    {
                        case EaseType.In:    return EaseFunction.InElastic;
                        case EaseType.Out:   return EaseFunction.OutElastic;
                        case EaseType.InOut: return EaseFunction.InOutElastic;
                    } break;
                case EasePower.Bounce:
                    switch (type)
                    {
                        case EaseType.In:    return EaseFunction.InBounce;
                        case EaseType.Out:   return EaseFunction.OutBounce;
                        case EaseType.InOut: return EaseFunction.InOutBounce;
                    } break;
            }

            return EaseFunction.Linear;
        }
        
        /// <summary>
        /// Get <c>EaseType</c> corresponding to <c>ease</c>.
        /// </summary>
        public static EaseType GetEaseType(Ease ease)
        {
            if (ease is Ease.Custom) return EaseType.Custom;
            
            var name = Enum.GetName(typeof(Ease), ease);
            
            if (name == null) return default;
            
            if (name.StartsWith("InOut"))   return EaseType.InOut;
            if (name.StartsWith("In"))      return EaseType.In;
            if (name.StartsWith("Out"))     return EaseType.Out;
            
            return EaseType.Custom;
        }
        
        /// <summary>
        /// Get <c>EasePower</c> corresponding to <c>ease</c>.
        /// </summary>
        public static EasePower GetEasePower(Ease ease)
        {
            if (ease is Ease.Custom) return EasePower.Linear;
            
            var name = Enum.GetName(typeof(Ease), ease);
            
            if (name == null) return default;
            
            if (name.StartsWith("InOut"))        name = name.Substring(5);
            else if (name.StartsWith("In"))      name = name.Substring(2);
            else if (name.StartsWith("Out"))     name = name.Substring(3);
            
            var result = Enum.Parse<EasePower>(name);
            
            return result;
        }
    }
}