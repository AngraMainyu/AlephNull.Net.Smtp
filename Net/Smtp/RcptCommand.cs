using System.Net.Mail;
using System.Text.RegularExpressions;

namespace AlephNull.Net.Smtp
{
    public class RcptCommand : SmtpCommand
    {
        public MailAddress RecipientAddress { get; private set; }

        public RcptCommand(string command)
            : base(command)
        {
            var pattern = new Regex(@"TO:<(.*)>", RegexOptions.IgnoreCase | RegexOptions.Compiled);
            this.RecipientAddress = new MailAddress(pattern.Match(this.Parameter).Captures[0].Value);
        }
    }
}
