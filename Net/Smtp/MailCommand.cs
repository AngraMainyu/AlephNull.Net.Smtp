using System.Net.Mail;
using System.Text.RegularExpressions;
using AlephNull.Net.Smtp;

namespace AlephNull.Net.Smtp
{
    class MailCommand : SmtpCommand
    {
        public MailAddress ReturnPath { get; private set; }

        public MailCommand(string command)
            : base(command)
        {
            var pattern = new Regex(@"FROM:<(.*)>", RegexOptions.IgnoreCase | RegexOptions.Compiled);
            this.ReturnPath = new MailAddress(pattern.Match(this.Parameter).Captures[0].Value);
        }
    }
}
