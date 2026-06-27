using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace part3
{
    internal class Emotion
    {
    }
}


namespace cyber_security_bottttt
{
    class Emotion
    {
        public string GetEmotion(string input)
        {
            input = input.ToLower();

            if (input.Contains("good") || input.Contains("great"))
                return "😊 I'm glad you're feeling good!";

            if (input.Contains("bad") || input.Contains("sad"))
                return "😔 I'm here if you need help.";

            if (input.Contains("angry"))
                return "😡 Take a deep breath. Let's fix the issue.";

            return "";
        }
    }
}