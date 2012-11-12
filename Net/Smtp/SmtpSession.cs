using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using AlephNull.IO;

namespace AlephNull.Net.Smtp
{
    class SmtpSession : IDisposable
    {
        public event SmtpMessageReceivedEventHandler MessageReceived;
        public event EventHandler CloseRequested;
        
        public event SmtpEventHandler<HeloCommand> Helo;
        public event SmtpEventHandler<MailCommand> Mail;
        public event SmtpEventHandler<RcptCommand> Rcpt;
        public event SmtpEventHandler<DataCommand> Data;
        public event SmtpEventHandler<NoopCommand> Noop;
        public event SmtpEventHandler<RsetCommand> Rset;
        public event SmtpEventHandler<QuitCommand> Quit;

        public event SmtpEventHandler<EhloCommand> Ehlo;

        public event EventHandler<SmtpCommand> UnhandledCommand;

        private TcpClient m_client;
        private UnmanagedBuffer m_buffer;
        private List<SmtpCommand> m_commands = new List<SmtpCommand>();
        private CancellationTokenSource m_cts = new CancellationTokenSource();
        private Task m_session;

        private static Dictionary<string, Type> s_commandTypeMap = new Dictionary<string, Type>()
        {
            { SmtpVerb.HELO, typeof(HeloCommand) },
            { SmtpVerb.MAIL, typeof(MailCommand) },
            { SmtpVerb.RCPT, typeof(RcptCommand) },
            { SmtpVerb.QUIT, typeof(QuitCommand) },
            { SmtpVerb.RSET, typeof(RsetCommand) },
            { SmtpVerb.NOOP, typeof(NoopCommand) },
            { SmtpVerb.DATA, typeof(DataCommand) },

            { SmtpVerb.EHLO, typeof(EhloCommand) },
        };

        internal SmtpSession(TcpClient client, UnmanagedBuffer buffer)
        {
            m_client = client;
            m_buffer = buffer;
        }
        public Task StartTransaction(string greeting)
        {
            if (string.IsNullOrEmpty(greeting)) {
                throw new ArgumentException();
            }
            var t = new Task(async () => {
                await m_client.GetStream().WriteAsync(UnicodeEncoding.ASCII.GetBytes(greeting), 0, greeting.Length);

                var cmdBuilder = new StringBuilder();
                while (!m_cts.Token.IsCancellationRequested) {

                    while (m_client.Available > 0 || cmdBuilder.Length == 0) {
                        var bytes = await m_client.GetStream().ReadAsync((byte[])m_buffer, 0, m_buffer.Capacity);
                        cmdBuilder.Append(UnicodeEncoding.ASCII.GetString((byte[])m_buffer, 0, bytes));
                    }

                    var str = cmdBuilder.ToString();
                    var verb = new Regex(@"(?<verb>\w+)").Match(str).Groups["verb"].Value;
                    var command = Activator.CreateInstance(s_commandTypeMap[verb], new object[] { str });
                    cmdBuilder.Clear();

                    string response = null;
                    switch (((SmtpCommand)command).Verb) {
                        case SmtpVerb.HELO:
                            response = Helo(this, new SmtpEventArgs<HeloCommand>((HeloCommand)command));
                            break;
                        case SmtpVerb.MAIL:
                            response = Mail(this, new SmtpEventArgs<MailCommand>((MailCommand)command));
                            break;
                        case SmtpVerb.RCPT:
                            response = Rcpt(this, new SmtpEventArgs<RcptCommand>((RcptCommand)command));
                            break;
                        case SmtpVerb.DATA:
                            response = Data(this, new SmtpEventArgs<DataCommand>((DataCommand)command));
                            break;
                        case SmtpVerb.RSET:
                            response = Rset(this, new SmtpEventArgs<RsetCommand>((RsetCommand)command));
                            break;
                        case SmtpVerb.NOOP:
                            response = Noop(this, new SmtpEventArgs<NoopCommand>((NoopCommand)command));
                            break;
                        case SmtpVerb.QUIT:
                            response = Quit(this, new SmtpEventArgs<QuitCommand>((QuitCommand)command));
                            break;
                        case SmtpVerb.EHLO:
                            response = Ehlo(this, new SmtpEventArgs<EhloCommand>((EhloCommand)command));
                            break;
                        default:
                            UnhandledCommand(this, (SmtpCommand)command);
                            response = "500 unrecognized command\r\n";
                            break;
                    }
                    if (!string.IsNullOrEmpty(response)) {
                        await m_client.GetStream().WriteAsync(UnicodeEncoding.ASCII.GetBytes(response), 0, response.Length);
                        if (((SmtpCommand)command).Verb == SmtpVerb.QUIT) {
                            CloseRequested(this, new EventArgs());
                        }
                        if (((SmtpCommand)command).Verb == SmtpVerb.DATA && response.StartsWith("354")) {
                            var messageBuilder = new StringBuilder();
                            while(!messageBuilder.ToString().EndsWith("\r\n.\r\n")) {
                                var bytes = await m_client.GetStream().ReadAsync((byte[])m_buffer, 0, m_buffer.Capacity);
                                messageBuilder.Append(UnicodeEncoding.ASCII.GetString((byte[])m_buffer, 0, bytes));
                            }
                            response = MessageReceived(this, messageBuilder.ToString());
                            await m_client.GetStream().WriteAsync(UnicodeEncoding.ASCII.GetBytes(response), 0, response.Length);
                            messageBuilder.Clear();
                        }
                    }
                }
            }, m_cts.Token);
            t.Start();
            return m_session = t;
        }
        public void Dispose()
        {
            m_cts.Cancel();
            m_client.Close();

            Dispose(true);
            GC.SuppressFinalize(this);
        }
        protected virtual void Dispose(bool disposing)
        {
            if (disposing) {
                m_cts.Dispose();
            }
        }
    }
}
