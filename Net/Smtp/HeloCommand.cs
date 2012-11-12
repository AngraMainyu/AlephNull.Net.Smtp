using System.Net.Mail;
using System.Text.RegularExpressions;

namespace AlephNull.Net.Smtp
{
    public class HeloCommand : SmtpCommand
    {
        public string ClientHostname { get; private set; }

        public HeloCommand(string command)
            : base(command)
        {
            this.ClientHostname = base.Parameter;
        }
    }
}
