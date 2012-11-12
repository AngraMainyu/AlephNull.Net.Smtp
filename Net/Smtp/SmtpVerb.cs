
namespace AlephNull.Net.Smtp
{
    public class SmtpVerb
    {
        // SMTP
        public const string HELO = "HELO";
        public const string MAIL = "MAIL";
        public const string DATA = "DATA";
        public const string RCPT = "RCPT";
        public const string QUIT = "QUIT";
        public const string NOOP = "NOOP";
        public const string RSET = "RSET";
        // ESMTP - Not implemented
        public const string EHLO = "EHLO";
    }
}
