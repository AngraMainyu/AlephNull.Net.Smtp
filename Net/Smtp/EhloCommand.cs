using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AlephNull.Net.Smtp;

namespace AlephNull.Net.Smtp
{
    public class EhloCommand : SmtpCommand
    {
        public string ClientHostname { get; private set; }

        public EhloCommand(string command)
            : base(command)
        {
            this.ClientHostname = base.Parameter;
        }
    }
}
