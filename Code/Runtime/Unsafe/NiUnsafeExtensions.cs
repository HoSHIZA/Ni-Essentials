using System;
using System.Runtime.CompilerServices;
using JetBrains.Annotations;

namespace NiGames.Essentials.Unsafe
{
    /// <summary>
    /// Extension class for <see cref="IntPtr"/>.
    /// </summary>
    [PublicAPI]
    public static unsafe class NiUnsafeExtensions
    {
        #region To
        
        /// <summary>
        /// Pointer to memory containing data of type <c>T</c>.
        /// </summary>
        [MethodImpl(256)]
        public static T* ToPointer<T>(this IntPtr ptr)
            where T : unmanaged
        {
            return (T*)ptr.ToPointer();
        }
        
        #endregion
        
        #region To Delegate

        /// <summary>
        /// Pointer to the memory containing the delegate.
        /// </summary>
        [MethodImpl(256)]
        public static delegate* <T1> ToDelegate<T1>(this IntPtr ptr)
        {
            return (delegate* <T1>)ptr.ToPointer();
        }
        
        /// <summary>
        /// Pointer to the memory containing the delegate.
        /// </summary>
        [MethodImpl(256)]
        public static delegate* <T1, T2> ToDelegate<T1, T2>(this IntPtr ptr)
        {
            return (delegate* <T1, T2>)ptr.ToPointer();
        }
        
        /// <summary>
        /// Pointer to the memory containing the delegate.
        /// </summary>
        [MethodImpl(256)]
        public static delegate* <T1, T2, T3> ToDelegate<T1, T2, T3>(this IntPtr ptr)
        {
            return (delegate* <T1, T2, T3>)ptr.ToPointer();
        }
        
        /// <summary>
        /// Pointer to the memory containing the delegate.
        /// </summary>
        [MethodImpl(256)]
        public static delegate* <T1, T2, T3, T4> ToDelegate<T1, T2, T3, T4>(this IntPtr ptr)
        {
            return (delegate* <T1, T2, T3, T4>)ptr.ToPointer();
        }

        #endregion
        
        #region To Delegate (Unmanaged)
        
        /// <summary>
        /// Pointer to the memory containing the unmanaged delegate.
        /// </summary>
        [MethodImpl(256)]
        public static delegate* unmanaged[Cdecl]<T1> ToDelegateUnmanaged<T1>(this IntPtr ptr)
            where T1 : unmanaged
        {
            return (delegate* unmanaged[Cdecl]<T1>)ptr.ToPointer();
        }
        
        /// <summary>
        /// Pointer to the memory containing the unmanaged delegate.
        /// </summary>
        [MethodImpl(256)]
        public static delegate* unmanaged[Cdecl]<T1, T2> ToDelegateUnmanaged<T1, T2>(this IntPtr ptr)
            where T1 : unmanaged
            where T2 : unmanaged
        {
            return (delegate* unmanaged[Cdecl]<T1, T2>)ptr.ToPointer();
        }
        
        /// <summary>
        /// Pointer to the memory containing the unmanaged delegate.
        /// </summary>
        [MethodImpl(256)]
        public static delegate* unmanaged[Cdecl]<T1, T2, T3> ToDelegateUnmanaged<T1, T2, T3>(this IntPtr ptr)
            where T1 : unmanaged
            where T2 : unmanaged
            where T3 : unmanaged
        {
            return (delegate* unmanaged[Cdecl]<T1, T2, T3>)ptr.ToPointer();
        }
        
        /// <summary>
        /// Pointer to the memory containing the unmanaged delegate.
        /// </summary>
        [MethodImpl(256)]
        public static delegate* unmanaged[Cdecl]<T1, T2, T3, T4> ToDelegateUnmanaged<T1, T2, T3, T4>(this IntPtr ptr)
            where T1 : unmanaged
            where T2 : unmanaged
            where T3 : unmanaged
            where T4 : unmanaged
        {
            return (delegate* unmanaged[Cdecl]<T1, T2, T3, T4>)ptr.ToPointer();
        }
        
        #endregion
        
        #region To Delegate (Managed)
        
        /// <summary>
        /// Pointer to the memory containing the managed delegate.
        /// </summary>
        [MethodImpl(256)]
        public static delegate* managed<T1> ToDelegateManaged<T1>(this IntPtr ptr)
        {
            return (delegate* managed<T1>)ptr.ToPointer();
        }
        
        /// <summary>
        /// Pointer to the memory containing the managed delegate.
        /// </summary>
        [MethodImpl(256)]
        public static delegate* managed<T1, T2> ToDelegateManaged<T1, T2>(this IntPtr ptr)
        {
            return (delegate* managed<T1, T2>)ptr.ToPointer();
        }
        
        /// <summary>
        /// Pointer to the memory containing the managed delegate.
        /// </summary>
        [MethodImpl(256)]
        public static delegate* managed<T1, T2, T3> ToDelegateManaged<T1, T2, T3>(this IntPtr ptr)
        {
            return (delegate* managed<T1, T2, T3>)ptr.ToPointer();
        }
        
        /// <summary>
        /// Pointer to the memory containing the managed delegate.
        /// </summary>
        [MethodImpl(256)]
        public static delegate* managed<T1, T2, T3, T4> ToDelegateManaged<T1, T2, T3, T4>(this IntPtr ptr)
        {
            return (delegate* managed<T1, T2, T3, T4>)ptr.ToPointer();
        }
        
        #endregion
    }
}