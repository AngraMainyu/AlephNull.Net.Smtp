using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;

namespace AlephNull.IO
{
    class UnmanagedBufferPool
    {
        ConcurrentBag<UnmanagedBuffer> m_pool;

        public UnmanagedBufferPool()
        {
            m_pool = new ConcurrentBag<UnmanagedBuffer>();
        }
        public UnmanagedBuffer Take(int minimumLength)
        {
            if (m_pool.Count == 0) {
                return new UnmanagedBuffer(minimumLength);
            }
            try {
                return m_pool.First(buffer => buffer.Capacity >= minimumLength);
            } catch (InvalidOperationException ioe) {
                return new UnmanagedBuffer(minimumLength);
            }
        }
        public void Add(UnmanagedBuffer buffer)
        {
            m_pool.Add(buffer);
        }
    }
}
