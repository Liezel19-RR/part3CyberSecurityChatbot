using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace part3
{
    internal class TypingEffect
    {
    }
}

namespace cyber_security_bottttt
{
    class TypingEffect
    {
        public async Task Type(TextBox box, string message)
        {
            foreach (char c in message)
            {
                box.AppendText(c.ToString());
                await Task.Delay(25);
            }

            box.AppendText("\n\n");
            box.ScrollToEnd();
        }
    }
}