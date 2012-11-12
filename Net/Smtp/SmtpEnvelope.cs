using System.Collections.Generic;
using System.Net.Mail;

namespace AlephNull.Net.Smtp
{
    class SmtpEnvelope
    {
        public MailAddress ReturnPath { get; set; }
        public List<MailAddress> Recipients { get; private set; }
        public string MessageBody { get; set; }

        public SmtpEnvelope()
        {
            Recipients = new List<MailAddress>();
        }
    }
}
