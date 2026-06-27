using part3;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace part3
{
    internal class Phishing
    {
    }
}

namespace cyber_security_bottttt
{
    class Phishing : SecurityTopic
    {
        public override string GetInfo()
        {
            return "Never click suspicious emails or unknown links.";
        }
    }
}