using System;
using System.Runtime.InteropServices;

namespace AlephNull.IO
{
    class UnmanagedBuffer
    {
        byte[] m_buffer;

        public int Capacity { get { return m_buffer.Length; } }

        public static explicit operator byte[](UnmanagedBuffer buffer)
        {
            return buffer.m_buffer;
        }
        public static explicit operator UnmanagedBuffer(byte[] buffer)
        {
            return new UnmanagedBuffer(buffer);
        }

        public UnmanagedBuffer(int length)
        {
            m_buffer = new byte[length];
        }
        public UnmanagedBuffer(byte[] buffer)
        {
            m_buffer = buffer;
        }
        unsafe public T Read<T>(int position) where T : struct
        {
            fixed (byte* pBuffer = m_buffer) {
                return (T)Marshal.PtrToStructure((IntPtr)(pBuffer + position), typeof(T));
            }
        }
        unsafe public String ReadString(int position)
        {
            fixed (byte* pBuffer = m_buffer) {
                return new String((sbyte*)(pBuffer + position));
            }
        }
        unsafe public String ReadString(int position, int length)
        {
            fixed (byte* pBuffer = m_buffer) {
                return new String((sbyte*)(pBuffer + position), 0, length);
            }
        }
        public byte ReadByte(int position)
        {
            return Read<byte>(position);
        }
        public sbyte ReadSByte(int position)
        {
            return Read<sbyte>(position);
        }
        public short ReadInt16(int position)
        {
            return Read<Int16>(position);
        }
        public ushort ReadUInt16(int position)
        {
            return Read<UInt16>(position);
        }
        public int ReadInt32(int position)
        {
            return Read<Int32>(position);
        }
        public uint ReadUInt32(int position)
        {
            return Read<UInt32>(position);
        }
        public long ReadInt64(int position)
        {
            return Read<Int64>(position);
        }
        public ulong ReadUInt64(int position)
        {
            return Read<UInt64>(position);
        }
        public Single ReadSingle(int position)
        {
            return Read<Single>(position);
        }
        public Double ReadDouble(int position)
        {
            return Read<Double>(position);
        }
        public Decimal ReadDecimal(int position)
        {
            return Read<Decimal>(position);
        }
    }
}
