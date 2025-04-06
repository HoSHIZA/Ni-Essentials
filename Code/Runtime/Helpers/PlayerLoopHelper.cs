using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Runtime.CompilerServices;
using JetBrains.Annotations;
using NiGames.Essentials.Collections;
using NiGames.Essentials.PlayerLoop;
using UnityEngine;
using UnityEngine.LowLevel;
using UnityEngine.PlayerLoop;
using NotNull = JetBrains.Annotations.NotNullAttribute;

namespace NiGames.Essentials
{
    [PublicAPI]
    public enum TimeKind : byte
    {
        Time,
        UnscaledTime,
        Realtime,
    }
    
    [PublicAPI]
    public enum PlayerLoopTiming : byte
    {
        Initialization,
        EarlyUpdate,
        FixedUpdate,
        PreUpdate,
        Update,
        PreLateUpdate,
        PostLateUpdate,
        TimeUpdate,
    }
    
    namespace PlayerLoop
    {
        public struct NiInitialization { }
        public struct NiEarlyUpdate { }
        public struct NiFixedUpdate { }
        public struct NiPreUpdate { }
        public struct NiUpdate { }
        public struct NiPreLateUpdate { }
        public struct NiPostLateUpdate { }
        public struct NiTimeUpdate { }
    }
    
    namespace Helpers
    {
        /// <summary>
        /// Helper class for unity PlayerLoop modification.
        /// </summary>
        [PublicAPI]
        public static class PlayerLoopHelper
        {
            [SuppressMessage("ReSharper", "StaticMemberInGenericType")]
            private static class CallbackCache<T> where T : struct
            {
                private static FastList<Action> _callbacks;
                
                public static void Invoke()
                {
                    for (int i = 0, length = _callbacks.Count; i < length; i++)
                    {
                        _callbacks.ElementAt(i)?.Invoke();
                    }
                }
                
                [MethodImpl(256)]
                public static void Add(Action callback)
                {
                    _callbacks.Add(callback);
                }
                
                [MethodImpl(256)]
                public static void Remove(Action callback)
                {
                    for (var i = 0; i < _callbacks.Count; i++)
                    {
                        if (_callbacks[i] == callback)
                        {
                            _callbacks.RemoveAtSwapBack(i);
                        }
                    }
                }
            }
            
            private static bool _init;
            
            [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterAssembliesLoaded)]
            private static void InitLoops() => InitHelper.DomainSafeInit(ref _init, () =>
            {
                ModifyLoop(systems =>
                {
                    systems.InsertLoop<Initialization, NiInitialization>(static () => CallbackCache<NiInitialization>.Invoke());
                    systems.InsertLoop<EarlyUpdate, NiEarlyUpdate>(static () => CallbackCache<NiEarlyUpdate>.Invoke());
                    systems.InsertLoop<FixedUpdate, NiFixedUpdate>(static () => CallbackCache<NiFixedUpdate>.Invoke());
                    systems.InsertLoop<PreUpdate, NiPreUpdate>(static () => CallbackCache<NiPreUpdate>.Invoke());
                    systems.InsertLoop<Update, NiUpdate>(static () => CallbackCache<NiUpdate>.Invoke());
                    systems.InsertLoop<PreLateUpdate, NiPreLateUpdate>(static () => CallbackCache<NiPreLateUpdate>.Invoke());
                    systems.InsertLoop<PostLateUpdate, NiPostLateUpdate>(static () => CallbackCache<NiPostLateUpdate>.Invoke());
                    systems.InsertLoop<TimeUpdate, NiTimeUpdate>(static () => CallbackCache<NiTimeUpdate>.Invoke());
                });
            });

            /// <summary>
            /// Subscribes the delegate to update PlayerLoop with the specified timing.
            /// </summary>
            [MethodImpl(256)]
            public static void AddCallback(PlayerLoopTiming timing, [NotNull] Action callback)
            {
                switch (timing)
                {
                    case PlayerLoopTiming.Initialization:  CallbackCache<NiInitialization>.Add(callback); break;
                    case PlayerLoopTiming.EarlyUpdate:     CallbackCache<NiEarlyUpdate>.Add(callback); break;
                    case PlayerLoopTiming.FixedUpdate:     CallbackCache<NiFixedUpdate>.Add(callback); break;
                    case PlayerLoopTiming.PreUpdate:       CallbackCache<NiPreUpdate>.Add(callback); break;
                    case PlayerLoopTiming.Update:          CallbackCache<NiUpdate>.Add(callback); break;
                    case PlayerLoopTiming.PreLateUpdate:   CallbackCache<NiPreLateUpdate>.Add(callback); break;
                    case PlayerLoopTiming.PostLateUpdate:  CallbackCache<NiPostLateUpdate>.Add(callback); break;
                    case PlayerLoopTiming.TimeUpdate:      CallbackCache<NiTimeUpdate>.Add(callback); break;
                }
            }

            /// <summary>
            /// Unsubscribe the delegate to update PlayerLoop with the specified timing.
            /// </summary>
            [MethodImpl(256)]
            public static void RemoveCallback(PlayerLoopTiming timing, [NotNull] Action callback)
            {
                switch (timing)
                {
                    case PlayerLoopTiming.Initialization:  CallbackCache<NiInitialization>.Remove(callback); break;
                    case PlayerLoopTiming.EarlyUpdate:     CallbackCache<NiEarlyUpdate>.Remove(callback); break;
                    case PlayerLoopTiming.FixedUpdate:     CallbackCache<NiFixedUpdate>.Remove(callback); break;
                    case PlayerLoopTiming.PreUpdate:       CallbackCache<NiPreUpdate>.Remove(callback); break;
                    case PlayerLoopTiming.Update:          CallbackCache<NiUpdate>.Remove(callback); break;
                    case PlayerLoopTiming.PreLateUpdate:   CallbackCache<NiPreLateUpdate>.Remove(callback); break;
                    case PlayerLoopTiming.PostLateUpdate:  CallbackCache<NiPostLateUpdate>.Remove(callback); break;
                    case PlayerLoopTiming.TimeUpdate:      CallbackCache<NiTimeUpdate>.Remove(callback); break;
                }
            }

            #region Insert & Apply

            /// <summary>
            /// Inserts a new <c>PlayerLoopSystem</c> into a matching <c>TLoop</c>. Sets the updated <c>PlayerLoop</c>.
            /// </summary>
            [MethodImpl(256)]
            public static void InsertLoop<TRunner>(PlayerLoopTiming timing, PlayerLoopSystem.UpdateFunction updateDelegate)
                where TRunner : struct
            {
                InsertLoop<TRunner>(timing.GetLoopTypeFromTiming(), updateDelegate);
            }

            /// <summary>
            /// Inserts a new <c>PlayerLoopSystem</c> into a matching <c>TLoop</c>. Sets the updated <c>PlayerLoop</c>.
            /// </summary>
            [MethodImpl(256)]
            public static void InsertLoop<TLoop, TRunner>(PlayerLoopSystem.UpdateFunction updateDelegate)
                where TLoop : struct 
                where TRunner : struct
            {
                InsertLoop<TRunner>(typeof(TLoop), updateDelegate);
            }

            /// <summary>
            /// Inserts a new <c>PlayerLoopSystem</c> into a matching <c>TLoop</c>. Sets the updated <c>PlayerLoop</c>.
            /// </summary>
            public static void InsertLoop<TRunner>(Type loopType, PlayerLoopSystem.UpdateFunction updateDelegate)
                where TRunner : struct
            {
                var playerLoop = UnityEngine.LowLevel.PlayerLoop.GetCurrentPlayerLoop();
                var newSubSystemList = playerLoop.subSystemList;
                
                newSubSystemList.InsertLoop<TRunner>(loopType, updateDelegate);
                
                playerLoop.subSystemList = newSubSystemList;
                UnityEngine.LowLevel.PlayerLoop.SetPlayerLoop(playerLoop);
            }
            
            #endregion
            
            #region Insert

            /// <summary>
            /// Inserts a new <c>PlayerLoopSystem</c> into a matching PlayerLoop <c>timing</c>.
            /// </summary>
            [MethodImpl(256)]
            public static void InsertLoop<TRunner>(this PlayerLoopSystem[] loopSystems, PlayerLoopTiming timing, PlayerLoopSystem.UpdateFunction updateDelegate)
                where TRunner : struct
            {
                InsertLoop<TRunner>(loopSystems, timing.GetLoopTypeFromTiming(), updateDelegate);
            }

            /// <summary>
            /// Inserts a new <c>PlayerLoopSystem</c> into a matching <c>TLoop</c>.
            /// </summary>
            [MethodImpl(256)]
            public static void InsertLoop<TLoop, TRunner>(this PlayerLoopSystem[] loopSystems, PlayerLoopSystem.UpdateFunction updateDelegate)
                where TLoop : struct 
                where TRunner : struct
            {
                InsertLoop<TRunner>(loopSystems, typeof(TLoop), updateDelegate);
            }
            
            /// <summary>
            /// Inserts a new <c>PlayerLoopSystem</c> into a matching <c>loopType</c>.
            /// </summary>
            public static void InsertLoop<TRunner>(this PlayerLoopSystem[] loopSystems, Type loopType, PlayerLoopSystem.UpdateFunction updateDelegate)
                where TRunner : struct
            {
                for (var i = 0; i < loopSystems.Length; i++)
                {
                    ref var loop = ref loopSystems[i];
                    
                    if (loop.type != loopType) continue;
                    
                    var sourceArray = loop.subSystemList
                        .Where(t => t.type != typeof(TRunner))
                        .ToArray();
                    var newSubSystemsArray = new PlayerLoopSystem[sourceArray.Length + 1];
                    
                    Array.Copy(sourceArray, 0, newSubSystemsArray, 1, sourceArray.Length);
                    
                    newSubSystemsArray[0] = new PlayerLoopSystem
                    {
                        type = typeof(TRunner),
                        updateDelegate = updateDelegate,
                    };
                    
                    loop.subSystemList = newSubSystemsArray;
                    
                    break;
                }
            }
            
            #endregion
            
            #region Remove
            
            /// <summary>
            /// Attempts to remove <c>PlayerLoopSystem</c> from the matching PlayerLoop <c>timing</c>.
            /// </summary>
            [MethodImpl(256)]
            public static bool TryRemoveLoop<TRunner>(this PlayerLoopSystem[] loopSystems, PlayerLoopTiming timing)
                where TRunner : struct
            {
                return TryRemoveLoop<TRunner>(loopSystems, timing.GetLoopTypeFromTiming());
            }
            
            /// <summary>
            /// Attempts to remove <c>PlayerLoopSystem</c> from the matching <c>TLoop</c>.
            /// </summary>
            [MethodImpl(256)]
            public static bool TryRemoveLoop<TLoop, TRunner>(this PlayerLoopSystem[] loopSystems)
                where TLoop : struct
                where TRunner : struct
            {
                return TryRemoveLoop<TRunner>(loopSystems, typeof(TLoop));
            }
            
            /// <summary>
            /// Attempts to remove <c>PlayerLoopSystem</c> from the matching <c>loopType</c>.
            /// </summary>
            public static bool TryRemoveLoop<TRunner>(this PlayerLoopSystem[] loopSystems, Type loopType)
                where TRunner : struct
            {
                for (var i = 0; i < loopSystems.Length; i++)
                {
                    ref var loop = ref loopSystems[i];
                    
                    if (loop.type != loopType) continue;
                    
                    if (TryRemoveLoop<TRunner>(ref loop)) return true;
                }
                
                return false;
            }
            
            // ! Other
            
            /// <summary>
            /// Makes an attempt to remove <c>TRunner</c> from the PlayerLoopSystem.
            /// </summary>
            public static bool TryRemoveLoop<TRunner>(ref PlayerLoopSystem system) 
                where TRunner : struct
            {
                var subSystems = system.subSystemList;
                
                if (subSystems == null) return false;
                
                for (var i = 0; i < subSystems.Length; i++)
                {
                    var subSystem = subSystems[i];
                    
                    if (subSystem.type == typeof(TRunner)) 
                    {
                        var newSubSystems = new PlayerLoopSystem[subSystems.Length - 1];
                        
                        Array.Copy(subSystems, newSubSystems, i);
                        Array.Copy(subSystems, i + 1, newSubSystems, i, subSystems.Length - i - 1);
                        
                        system.subSystemList = newSubSystems;
                        
                        return true;
                    }
                    
                    if (TryRemoveLoop<TRunner>(ref subSystems[i])) return true;
                }
                
                return false;
            }
            
            #endregion

            #region Helpers
            
            /// <summary>
            /// Method to modify <c>PlayerLoop</c>, after calling the <c>onModify</c> delegate, sets the updated <c>PlayerLoop</c>.
            /// </summary>
            public static void ModifyLoop([NotNull] Action<PlayerLoopSystem[]> onModify)
            {
                var playerLoop = UnityEngine.LowLevel.PlayerLoop.GetCurrentPlayerLoop();
                var newSubSystemList = playerLoop.subSystemList;
                
                onModify.Invoke(newSubSystemList);
                
                playerLoop.subSystemList = newSubSystemList;
                UnityEngine.LowLevel.PlayerLoop.SetPlayerLoop(playerLoop);
            }

            /// <summary>
            /// Gets the <c>PlayerLoopSystem</c> type from timing.
            /// </summary>
            public static Type GetLoopTypeFromTiming(this PlayerLoopTiming timing) => timing switch
            {
                PlayerLoopTiming.Initialization => typeof(Initialization),
                PlayerLoopTiming.EarlyUpdate => typeof(EarlyUpdate),
                PlayerLoopTiming.FixedUpdate => typeof(FixedUpdate),
                PlayerLoopTiming.PreUpdate => typeof(PreUpdate),
                PlayerLoopTiming.Update => typeof(Update),
                PlayerLoopTiming.PreLateUpdate => typeof(PreLateUpdate),
                PlayerLoopTiming.PostLateUpdate => typeof(PostLateUpdate),
                _ => typeof(TimeUpdate),
            };
            
            public static Type GetNiLoopTypeFromTiming(this PlayerLoopTiming timing) => timing switch
            {
                PlayerLoopTiming.Initialization => typeof(NiInitialization),
                PlayerLoopTiming.EarlyUpdate => typeof(NiEarlyUpdate),
                PlayerLoopTiming.FixedUpdate => typeof(NiFixedUpdate),
                PlayerLoopTiming.PreUpdate => typeof(NiPreUpdate),
                PlayerLoopTiming.Update => typeof(NiUpdate),
                PlayerLoopTiming.PreLateUpdate => typeof(NiPreLateUpdate),
                PlayerLoopTiming.PostLateUpdate => typeof(NiPostLateUpdate),
                _ => typeof(NiTimeUpdate),
            };

            #endregion
        }
    }
}