using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace part3
{
    internal class SafeBrowsing
    {
    }
}

namespace cyber_security_bottttt
{
    class SafeBrowsing : SecurityTopic
    {
        public override string GetInfo()
        {
            return "Always check HTTPS before entering personal info.";
        }
    }
}