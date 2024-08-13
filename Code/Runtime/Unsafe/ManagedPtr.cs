using System;
using System.Runtime.InteropServices;

namespace NiGames.Essentials.Unsafe
{
    /// <summary>
    /// A structure to keep track of a pointer to a managed object.
    /// </summary>
    public unsafe struct ManagedPtr<T> : IDisposable, IEquatable<ManagedPtr<T>>
        where T : class
    {
        private IntPtr _ptr;
        private GCHandle _handle;
        
        public T Value => (T)_handle.Target;
        public bool IsValid => _ptr.ToPointer() != null;
        
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
    }
}