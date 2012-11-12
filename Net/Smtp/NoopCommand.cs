namespace AlephNull.Net.Smtp
{
    public class NoopCommand : SmtpCommand
    {
        public NoopCommand(string command)
            : base(command)
        {

        }
    }
}
