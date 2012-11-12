using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace AlephNull.Net
{
    class TcpServer
    {
        TcpListener Listener;
        public bool Listening { get; private set; }

        public TcpServer(IPAddress address, int port)
        {
            Listener = new TcpListener(address, port);
        }
        public virtual Task Listen(Action<TcpClient> acceptClientAction)
        {
            Listener.Start();
            Listening = true;

            return Task.Factory.StartNew(async () => {
                while (Listening) {
                    acceptClientAction(await Listener.AcceptTcpClientAsync());
                }
            });
        }
        public virtual void Stop()
        {
            Listening = false;
            Listener.Stop();
        }
    }
}
