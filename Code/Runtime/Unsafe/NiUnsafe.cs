using System;
using System.Runtime.CompilerServices;
using JetBrains.Annotations;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;

namespace NiGames.Essentials.Unsafe
{
    /// <summary>
    /// A class providing a wrapper around <see cref="UnsafeUtility"/>,
    /// as well as implementing other potentially useful methods for handling memory and unsafe code.
    /// </summary>
    [PublicAPI]
    public static unsafe class NiUnsafe
    {
        #region Malloc

        /// <summary>
        /// Allocate memory.
        /// </summary>
        /// <remarks><c>UnsafeUtility.Malloc</c> wrapper.</remarks>
        [MethodImpl(256)]
        public static void* Malloc(long size, Allocator allocator = Allocator.Persistent)
        {
            return UnsafeUtility.Malloc(size, UnsafeUtility.AlignOf<byte>(), allocator);
        }

        /// <summary>
        /// Allocate memory.
        /// </summary>
        /// <remarks><c>UnsafeUtility.Malloc</c> wrapper.</remarks>
        [MethodImpl(256)]
        public static void* Malloc(long size, int alignment, Allocator allocator = Allocator.Persistent)
        {
            return UnsafeUtility.Malloc(size, alignment, allocator);
        }

        #endregion
        
        #region Malloc (Generic)
        
        /// <summary>
        /// Allocate memory. Uses size and alignment of type <c>T</c>.
        /// </summary>
        /// <remarks><c>UnsafeUtility.Malloc</c> wrapper.</remarks>
        [MethodImpl(256)]
        public static void* Malloc<T>(Allocator allocator = Allocator.Persistent)
            where T : struct
        {
            return UnsafeUtility.Malloc(UnsafeUtility.SizeOf<T>(), UnsafeUtility.AlignOf<T>(), allocator);
        }
        
        /// <summary>
        /// Allocate memory. Uses size of type <c>T</c>.
        /// </summary>
        /// <remarks><c>UnsafeUtility.Malloc</c> wrapper.</remarks>
        [MethodImpl(256)]
        public static void* Malloc<T>(int alignment, Allocator allocator = Allocator.Persistent)
            where T : struct
        {
            return UnsafeUtility.Malloc(UnsafeUtility.SizeOf<T>(), alignment, allocator);
        }
        
        #endregion

        #region ReAlloc

        /// <summary>
        /// Reallocate memory.
        /// </summary>
        [MethodImpl(256)]
        public static void* ReAlloc(void* ptr, long oldSize, long newSize, int alignment, Allocator allocator = Allocator.Persistent)
        {
            var newPtr = Malloc(newSize, alignment, allocator);
            UnsafeUtility.MemCpy(newPtr, ptr, Math.Min(oldSize, newSize));
            Free(ptr, allocator);
            return newPtr;
        }

        /// <summary>
        /// Reallocate memory.
        /// </summary>
        [MethodImpl(256)]
        public static void* ReAlloc(void* ptr, long oldSize, long newSize, Allocator allocator = Allocator.Persistent)
        {
            var newPtr = Malloc(newSize, allocator);
            UnsafeUtility.MemCpy(newPtr, ptr, Math.Min(oldSize, newSize));
            Free(ptr, allocator);
            return newPtr;
        }

        #endregion

        #region Free

        /// <summary>
        /// Free memory.
        /// </summary>
        /// <remarks><c>UnsafeUtility.Free</c> wrapper.</remarks>
        [MethodImpl(256)]
        public static void Free(void* memory, Allocator allocator = Allocator.Persistent)
        {
            UnsafeUtility.Free(memory, allocator);
        }

        /// <summary>
        /// Free memory. Uses <c>IntPtr.ToPointer()</c> to get a memory pointer.
        /// </summary>
        /// <remarks><c>UnsafeUtility.Free</c> wrapper.</remarks>
        [MethodImpl(256)]
        public static void Free(IntPtr ptr, Allocator allocator = Allocator.Persistent)
        {
            UnsafeUtility.Free(ptr.ToPointer(), allocator);
        }

        #endregion

        #region As

        /// <summary>
        /// Reinterprets the reference as a reference of a different type.
        /// </summary>
        [MethodImpl(256)]
        public static ref TNew As<TFrom, TNew>(ref TFrom from)
        {
            return ref UnsafeUtility.As<TFrom, TNew>(ref from);
        }

        /// <summary>
        /// Gets a reference to the struct at its current location in memory.
        /// </summary>
        [MethodImpl(256)]
        public static ref T AsRef<T>(void* ptr)
            where T : struct
        {
            return ref UnsafeUtility.AsRef<T>(ptr);
        }

        #endregion

        #region Align

        [MethodImpl(256)]
        public static T* Align<T>(T* ptr, int alignment)
            where T : unmanaged
        {
            var addr = (ulong)ptr;
            return (T*)((addr + (ulong)(alignment - 1)) & ~(ulong)(alignment - 1));
        }

        [MethodImpl(256)]
        public static bool IsAligned<T>(T* ptr, int alignment)
            where T : unmanaged
        {
            return ((ulong)ptr & (ulong)(alignment - 1)) == 0;
        }

        #endregion

        #region Copy Structure

        /// <summary>
        /// Copies <c>sizeof(T)</c> bytes from <c>ptr</c> to <c>output</c>.
        /// </summary>
        [MethodImpl(256)]
        public static void CopyPtrToStructure<T>(void* ptr, out T output)
            where T : struct
        {
            UnsafeUtility.CopyPtrToStructure(ptr, out output);
        }

        /// <summary>
        /// Copies <c>sizeof(T)</c> bytes from <c>input</c> to <c>ptr</c>.
        /// </summary>
        [MethodImpl(256)]
        public static void CopyStructureToPtr<T>(ref T input, void* ptr)
            where T : struct
        {
            UnsafeUtility.CopyStructureToPtr(ref input, ptr);
        }

        #endregion

        #region Mem

        /// <summary>
        /// Copy memory.
        /// </summary>
        [MethodImpl(256)]
        public static void MemCpy(void* destination, void* source, long size)
        {
            UnsafeUtility.MemCpy(destination, source, size);
        }

        /// <summary>
        /// Move memory.
        /// </summary>
        [MethodImpl(256)]
        public static void MemMove(void* destination, void* source, long size)
        {
            UnsafeUtility.MemMove(destination, source, size);
        }

        /// <summary>
        /// Set memory.
        /// </summary>
        [MethodImpl(256)]
        public static void MemSet(void* destination, byte value, long size)
        {
            UnsafeUtility.MemSet(destination, value, size);
        }

        /// <summary>
        /// Clear memory.
        /// </summary>
        [MethodImpl(256)]
        public static void MemClear(void* destination, long size)
        {
            UnsafeUtility.MemClear(destination, size);
        }

        /// <summary>
        /// Compare memory.
        /// </summary>
        [MethodImpl(256)]
        public static int MemCmp(void* ptr1, void* ptr2, long size)
        {
            return UnsafeUtility.MemCmp(ptr1, ptr2, size);
        }

        /// <summary>
        /// Swap memory. If <c>checkOverlap</c> is true, throws an exception if memory blocks overlap
        /// </summary>
        public static void MemSwap(void* ptr1, void* ptr2, long size, bool checkOverlap = true)
        {
            var dst = (byte*)ptr1;
            var src = (byte*)ptr2;
            
            if (checkOverlap && dst + size > src && src + size > dst)
            {
                throw new InvalidOperationException("MemSwap memory blocks are overlapped.");
            }
            
            var tmp = stackalloc byte[1024];
            
            while (size > 0)
            {
                var numBytes = Math.Min(size, 1024);
                MemCpy(tmp, dst, numBytes);
                MemCpy(dst, src, numBytes);
                MemCpy(src, tmp, numBytes);
                
                size -= numBytes;
                src += numBytes;
                dst += numBytes;
            }
        }
        
        #endregion
    }
}