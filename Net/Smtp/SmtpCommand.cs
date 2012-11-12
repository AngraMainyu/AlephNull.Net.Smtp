using System.Text.RegularExpressions;

namespace AlephNull.Net.Smtp
{
    public class SmtpCommand
    {
        public string Verb { get; private set; }
        public string Parameter { get; private set; }

        public SmtpCommand(string verb, string param)
        {
            this.Verb = verb;
            this.Parameter = param;
        }
        public SmtpCommand(string command) {
            var cmdPattern = new Regex(@"(?<verb>\w+)(\s(?<param>.*))?\r\n", RegexOptions.Compiled);
            var match = cmdPattern.Match(command);

            this.Verb = match.Groups["verb"].Value;
            this.Parameter = match.Groups["param"].Value;
        }
    }
}
