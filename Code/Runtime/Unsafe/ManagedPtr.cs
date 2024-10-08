﻿using System;
using System.Runtime.InteropServices;

namespace NiGames.Essentials.Unsafe
{
    /// <summary>
    /// A structure to keep track of a pointer to a managed object.
    /// </summary>
    public unsafe struct ManagedPtr<T> : IDisposable, IEquatable<ManagedPtr<T>>
    {
        private IntPtr _ptr;
        private GCHandle _handle;
        
        public T Value => (T)_handle.Target;
        public bool IsValid => _ptr.ToPointer() != null;
        public bool IsAllocated => _handle.IsAllocated;
        
        public ManagedPtr(T data)
        {
            _handle = data != null ? GCHandle.Alloc(data) : default;
            _ptr = GCHandle.ToIntPtr(_handle);
        }
        
        public void Dispose()
        {
            if (_handle.IsAllocated)
            {
                _handle.Free();
            }
            
            _ptr = IntPtr.Zero;
        }
        
        public bool Equals(ManagedPtr<T> other)
        {
            return other._ptr == _ptr;
        }
        
        public static implicit operator ManagedPtr<T>(T value)
        {
            return new ManagedPtr<T>(value);
        }
        
        public static implicit operator T(ManagedPtr<T> ptr)
        {
            return ptr.Value;
        }
    }
}