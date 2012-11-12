using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;
using AlephNull.IO;
using AlephNull.Net;

namespace AlephNull.Net.Smtp
{
    class SmtpServer : TcpServer
    {
        UnmanagedBufferPool m_pool;

        public SmtpServer(IPAddress address, int port = 25) : base(address, port)
        {
            m_pool = new UnmanagedBufferPool();
        }
        public Task Listen(System.Action<SmtpSession> acceptClientAction)
        {
            return Listen((TcpClient c) => {
                var buffer = m_pool.Take(1024);
                var session = new SmtpSession(c, buffer);
                acceptClientAction(session);
            });
        }
    }
}
