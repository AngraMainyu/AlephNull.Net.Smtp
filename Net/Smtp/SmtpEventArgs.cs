using System;

namespace AlephNull.Net.Smtp
{
   public class SmtpEventArgs<TSmtpCommand> : EventArgs where TSmtpCommand : SmtpCommand
    {
        public TSmtpCommand Command { get; private set; }

        public SmtpEventArgs(SmtpCommand command)
        {
            this.Command = (TSmtpCommand)command;
        }
        
    }
}
