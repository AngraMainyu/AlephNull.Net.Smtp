namespace AlephNull.Net.Smtp
{
    public delegate string SmtpEventHandler<TSmtpCommand>(object sender, SmtpEventArgs<TSmtpCommand> e) where TSmtpCommand : SmtpCommand;
}
