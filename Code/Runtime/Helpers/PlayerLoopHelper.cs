using System;
using System.Linq;
using System.Runtime.CompilerServices;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.LowLevel;
using UnityEngine.PlayerLoop;

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
            private static bool _init;
            
            public static event Action OnInitialization;
            public static event Action OnEarlyUpdate;
            public static event Action OnFixedUpdate;
            public static event Action OnPreUpdate;
            public static event Action OnUpdate;
            public static event Action OnPreLateUpdate;
            public static event Action OnPostLateUpdate;
            public static event Action OnTimeUpdate;
            
            [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterAssembliesLoaded)]
            private static void InitLoops() => InitHelper.DomainSafeInit(ref _init, () =>
            {
                ModifyLoop(systems =>
                {
                    InsertLoop<Initialization, PlayerLoop.NiInitialization>(systems, static () => OnInitialization?.Invoke());
                    InsertLoop<EarlyUpdate, PlayerLoop.NiEarlyUpdate>(systems, static () => OnEarlyUpdate?.Invoke());
                    InsertLoop<FixedUpdate, PlayerLoop.NiFixedUpdate>(systems, static () => OnFixedUpdate?.Invoke());
                    InsertLoop<PreUpdate, PlayerLoop.NiPreUpdate>(systems, static () => OnPreUpdate?.Invoke());
                    InsertLoop<Update, PlayerLoop.NiUpdate>(systems, static () => OnUpdate?.Invoke());
                    InsertLoop<PreLateUpdate, PlayerLoop.NiPreLateUpdate>(systems, static () => OnPreLateUpdate?.Invoke());
                    InsertLoop<PostLateUpdate, PlayerLoop.NiPostLateUpdate>(systems, static () => OnPostLateUpdate?.Invoke());
                    InsertLoop<TimeUpdate, PlayerLoop.NiTimeUpdate>(systems, static () => OnTimeUpdate?.Invoke());
                });
            });
            
            /// <summary>
            /// Subscribes the delegate to update PlayerLoop with the specified time.
            /// </summary>
            [MethodImpl(256)]
            public static void Subscribe(PlayerLoopTiming timing, [NotNull] Action callback)
            {
                switch (timing)
                {
                    case PlayerLoopTiming.Initialization:  OnInitialization += callback; break;
                    case PlayerLoopTiming.EarlyUpdate:     OnEarlyUpdate += callback; break;
                    case PlayerLoopTiming.FixedUpdate:     OnFixedUpdate += callback; break;
                    case PlayerLoopTiming.PreUpdate:       OnPreUpdate += callback; break;
                    case PlayerLoopTiming.Update:          OnUpdate += callback; break;
                    case PlayerLoopTiming.PreLateUpdate:   OnPreLateUpdate += callback; break;
                    case PlayerLoopTiming.PostLateUpdate:  OnPostLateUpdate += callback; break;
                    case PlayerLoopTiming.TimeUpdate:      OnTimeUpdate += callback; break;
                }
            }
            
            /// <summary>
            /// Unsubscribe the delegate to update PlayerLoop with the specified time.
            /// </summary>
            [MethodImpl(256)]
            public static void Unsubscribe(PlayerLoopTiming timing, [NotNull] Action callback)
            {
                switch (timing)
                {
                    case PlayerLoopTiming.Initialization:  OnInitialization -= callback; break;
                    case PlayerLoopTiming.EarlyUpdate:     OnEarlyUpdate -= callback; break;
                    case PlayerLoopTiming.FixedUpdate:     OnFixedUpdate -= callback; break;
                    case PlayerLoopTiming.PreUpdate:       OnPreUpdate -= callback; break;
                    case PlayerLoopTiming.Update:          OnUpdate -= callback; break;
                    case PlayerLoopTiming.PreLateUpdate:   OnPreLateUpdate -= callback; break;
                    case PlayerLoopTiming.PostLateUpdate:  OnPostLateUpdate -= callback; break;
                    case PlayerLoopTiming.TimeUpdate:      OnTimeUpdate -= callback; break;
                }
            }
            
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
            /// Inserts a new <c>PlayerLoopSystem</c> into a matching <c>TLoop</c>. Sets the updated <c>PlayerLoop</c>.
            /// </summary>
            public static void InsertLoop<TLoop, TRunner>(PlayerLoopSystem.UpdateFunction updateDelegate)
                where TLoop : struct 
                where TRunner : struct
            {
                var playerLoop = UnityEngine.LowLevel.PlayerLoop.GetCurrentPlayerLoop();
                var newSubSystemList = playerLoop.subSystemList;
                
                InsertLoop<TLoop, TRunner>(newSubSystemList, updateDelegate);
                
                playerLoop.subSystemList = newSubSystemList;
                UnityEngine.LowLevel.PlayerLoop.SetPlayerLoop(playerLoop);
            }
            
            /// <summary>
            /// Inserts a new <c>PlayerLoopSystem</c> into a matching <c>TLoop</c>.
            /// </summary>
            public static void InsertLoop<TLoop, TRunner>(this PlayerLoopSystem[] loopSystems, PlayerLoopSystem.UpdateFunction updateDelegate)
                where TLoop : struct 
                where TRunner : struct
            {
                for (var i = 0; i < loopSystems.Length; i++)
                {
                    ref var loop = ref loopSystems[i];
                    
                    if (loop.type != typeof(TLoop)) continue;
                    
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
            
            /// <summary>
            /// Attempts to remove <c>PlayerLoopSystem</c> from the corresponding <c>TLoop</c>.
            /// </summary>
            public static bool TryRemoveLoop<TLoop, TRunner>(this PlayerLoopSystem[] loopSystems)
                where TLoop : struct
                where TRunner : struct
            {
                for (var i = 0; i < loopSystems.Length; i++)
                {
                    ref var loop = ref loopSystems[i];
                    
                    if (loop.type != typeof(TLoop)) continue;
                    
                    if (TryRemoveLoop<TRunner>(ref loop)) return true;
                }
                
                return false;
            }
            
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
        }
    }
}