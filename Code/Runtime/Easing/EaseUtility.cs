using System;
using System.Runtime.CompilerServices;
using NiGames.Essentials.Unsafe;

namespace NiGames.Essentials
{
    public static class EaseUtility
    {
        [MethodImpl(256)]
        public static float Evaluate(float t, Ease ease)
        {
            return GetFunction(ease).Invoke(t);
        }
        
        [MethodImpl(256)]
        public static float Evaluate(float t, EaseType type, EasePower power)
        {
            return GetFunction(type, power).Invoke(t);
        }
        
        [MethodImpl(256)]
        public static Func<float, float> GetFunction(Ease ease, Func<float, float> customFunc)
        {
            return ease is Ease.Custom 
                ? customFunc 
                : GetFunction(ease);
        }
        
        [MethodImpl(256)]
        public static unsafe delegate*<float, float> GetFunctionPtr(Ease ease)
        {
            return GetFunction(ease).Method.MethodHandle.GetFunctionPointer().ToDelegate<float, float>();
        }
        
        [MethodImpl(256)]
        public static unsafe delegate*<float, float> GetFunctionPtr(EaseType type, EasePower power)
        {
            return GetFunction(type, power).Method.MethodHandle.GetFunctionPointer().ToDelegate<float, float>();
        }
        
        public static Func<float, float> GetFunction(Ease ease)
        {
            switch (ease)
            {
                case Ease.Linear:            return Easing.Linear;
                case Ease.InQuad:            return Easing.InQuad;
                case Ease.OutQuad:           return Easing.OutQuad;
                case Ease.InOutQuad:         return Easing.InOutQuad;
                case Ease.InCubic:           return Easing.InCubic;
                case Ease.OutCubic:          return Easing.OutCubic;
                case Ease.InOutCubic:        return Easing.InOutCubic;
                case Ease.InQuart:           return Easing.InQuart;
                case Ease.OutQuart:          return Easing.OutQuart;
                case Ease.InOutQuart:        return Easing.InOutQuart;
                case Ease.InQuint:           return Easing.InQuint;
                case Ease.OutQuint:          return Easing.OutQuint;
                case Ease.InOutQuint:        return Easing.InOutQuint;
                case Ease.InSine:            return Easing.InSine;
                case Ease.OutSine:           return Easing.OutSine;
                case Ease.InOutSine:         return Easing.InOutSine;
                case Ease.InExpo:            return Easing.InExpo;
                case Ease.OutExpo:           return Easing.OutExpo;
                case Ease.InOutExpo:         return Easing.InOutExpo;
                case Ease.InCirc:            return Easing.InCirc;
                case Ease.OutCirc:           return Easing.OutCirc;
                case Ease.InOutCirc:         return Easing.InOutCirc;
                case Ease.InBack:            return Easing.InBack;
                case Ease.OutBack:           return Easing.OutBack;
                case Ease.InOutBack:         return Easing.InOutBack;
                case Ease.InElastic:         return Easing.InElastic;
                case Ease.OutElastic:        return Easing.OutElastic;
                case Ease.InOutElastic:      return Easing.InOutElastic;
                case Ease.InBounce:          return Easing.InBounce;
                case Ease.OutBounce:         return Easing.OutBounce;
                case Ease.InOutBounce:       return Easing.InOutBounce;
                default:                     return Easing.Linear;
            }
        }
        
        public static Func<float, float> GetFunction(EaseType type, EasePower power)
        {
            if (type is EaseType.Custom)     return Easing.Linear;
            
            switch (power)
            {
                case EasePower.Linear:       return Easing.Linear;
                case EasePower.Quad:
                    switch (type)
                    {
                        case EaseType.In:    return Easing.InQuad;
                        case EaseType.Out:   return Easing.OutQuad;
                        case EaseType.InOut: return Easing.InOutQuad;
                    } break;
                case EasePower.Cubic:
                    switch (type)
                    {
                        case EaseType.In:    return Easing.InCubic;
                        case EaseType.Out:   return Easing.OutCubic;
                        case EaseType.InOut: return Easing.InOutCubic;
                    } break;
                case EasePower.Quart:
                    switch (type)
                    {
                        case EaseType.In:    return Easing.InQuart;
                        case EaseType.Out:   return Easing.OutQuart;
                        case EaseType.InOut: return Easing.InOutQuart;
                    } break;
                case EasePower.Quint:
                    switch (type)
                    {
                        case EaseType.In:    return Easing.InQuint;
                        case EaseType.Out:   return Easing.OutQuint;
                        case EaseType.InOut: return Easing.InOutQuint;
                    } break;
                case EasePower.Sine:
                    switch (type)
                    {
                        case EaseType.In:    return Easing.InSine;
                        case EaseType.Out:   return Easing.OutSine;
                        case EaseType.InOut: return Easing.InOutSine;
                    } break;
                case EasePower.Expo:
                    switch (type)
                    {
                        case EaseType.In:    return Easing.InExpo;
                        case EaseType.Out:   return Easing.OutExpo;
                        case EaseType.InOut: return Easing.InOutExpo;
                    } break;
                case EasePower.Circ:
                    switch (type)
                    {
                        case EaseType.In:    return Easing.InCirc;
                        case EaseType.Out:   return Easing.OutCirc;
                        case EaseType.InOut: return Easing.InOutCirc;
                    } break;
                case EasePower.Back:
                    switch (type)
                    {
                        case EaseType.In:    return Easing.InBack;
                        case EaseType.Out:   return Easing.OutBack;
                        case EaseType.InOut: return Easing.InOutBack;
                    } break;
                case EasePower.Elastic:
                    switch (type)
                    {
                        case EaseType.In:    return Easing.InElastic;
                        case EaseType.Out:   return Easing.OutElastic;
                        case EaseType.InOut: return Easing.InOutElastic;
                    } break;
                case EasePower.Bounce:
                    switch (type)
                    {
                        case EaseType.In:    return Easing.InBounce;
                        case EaseType.Out:   return Easing.OutBounce;
                        case EaseType.InOut: return Easing.InOutBounce;
                    } break;
            }

            return Easing.Linear;
        }
        
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