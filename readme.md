# AlephNull.Net.Smtp

A programmable SMTP server written for .NET 4.5.

## Issues and notes

* By default, the buffer pool allocates 1KB to each client (which will always be sufficient.) Returning buffers to the pool is not implemented yet; they are disposed with the session instead.
* Timeouts are not automatically handled. The server reads messages and raises the corresponding event. Other functionality must be provided by the developer.
* No support for Extended SMTP yet.

## Usage

###Example 1
```cs
using System.Collections.Generic;
using System.Net;
using System.Windows.Forms;
using AlephNull.Net.Smtp;

namespace AlephNull
{
    public static class Program
    {
        static void Main(string[] args)
        {
            var clients = new Dictionary<SmtpSession, SmtpEnvelope>();
            var server = new SmtpServer(IPAddress.Any);

            server.Listen((c) => {
                clients.Add(c, new SmtpEnvelope());
                c.Ehlo += (sender, e) => {
                    return "550 not implemented\r\n";
                };
                c.Helo += (sender, e) => {
                    return "250 ok\r\n";
                };
                c.Mail += (sender, e) => {
                    clients[(SmtpSession)sender].ReturnPath = e.Command.ReturnPath;
                    return "250 ok\r\n";
                };
                c.Rcpt += (sender, e) => {
                    clients[(SmtpSession)sender].Recipients.Add(e.Command.RecipientAddress);
                    return "250 ok\r\n";
                };
                c.Rset += (sender, e) => {
                    clients[(SmtpSession)sender] = new SmtpEnvelope();
                    return "250 ok\r\n";
                };
                c.Data += (sender, e) => {
                    return "354 end data with <cr><lf>.<cr><lf>\r\n";
                };
                c.MessageReceived += (sender, e) => {
                    clients[(SmtpSession)sender].MessageBody = e;
                    return "250 ok\r\n";
                };
                c.Noop += (sender, e) => {
                    return "250 ok\r\n";
                };
                c.Quit += (sender, e) => {
                    return "221 closing connection\r\n";
                };
                c.CloseRequested += (sender, e) => {
                    c.Dispose();
                };

                c.StartTransaction("220 my.server.net\r\n");
            });
            Application.Run();
        }
    }
}