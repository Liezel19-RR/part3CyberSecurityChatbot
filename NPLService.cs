using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace part3.classes
{
    internal class NPLService
    {
    }
}


namespace CybersecurityAwarenessBot.Services
{
    /// <summary>
    /// Simulates Natural Language Processing using keyword detection
    /// Recognizes different ways users phrase their questions
    /// </summary>
    public class NLPService
    {
        private readonly Dictionary<string, List<string>> _actionKeywords;

        public NLPService()
        {
            _actionKeywords = new Dictionary<string, List<string>>
            {
                ["add_task"] = new List<string> { "add task", "new task", "create task", "add a task", "create a task" },
                ["add_reminder"] = new List<string> { "remind me", "set reminder", "add reminder", "remind" },
                ["show_tasks"] = new List<string> { "show tasks", "my tasks", "list tasks", "view tasks" },
                ["show_log"] = new List<string> { "show log", "activity log", "what have you done", "show activity", "history" },
                ["start_quiz"] = new List<string> { "start quiz", "play quiz", "take quiz", "begin quiz", "quiz" },
                ["complete_task"] = new List<string> { "complete", "done", "finish", "mark complete" },
                ["delete_task"] = new List<string> { "delete", "remove", "cancel" },
                ["help"] = new List<string> { "help", "what can you do", "commands", "help me" }
            };
        }

        public string DetectIntent(string userInput)
        {
            if (string.IsNullOrWhiteSpace(userInput))
                return "unknown";

            string input = userInput.ToLower().Trim();

            foreach (var action in _actionKeywords)
            {
                foreach (string keyword in action.Value)
                {
                    if (input.Contains(keyword))
                    {
                        return action.Key;
                    }
                }
            }

            if (ContainsAny(input, new[] { "password", "passwords", "p@ssword" }))
                return "password_topic";

            if (ContainsAny(input, new[] { "phishing", "scam", "fraud" }))
                return "phishing_topic";

            if (ContainsAny(input, new[] { "browsing", "browser", "https", "padlock", "wi-fi", "public wifi" }))
                return "browsing_topic";

            return "unknown";
        }

        private bool ContainsAny(string input, string[] keywords)
        {
            foreach (string keyword in keywords)
            {
                if (input.Contains(keyword))
                    return true;
            }
            return false;
        }

        public string ExtractTaskTitle(string userInput)
        {
            string input = userInput.ToLower();

            string[] patterns = {
                @"add task to (.+)",
                @"add a task to (.+)",
                @"create task to (.+)",
                @"remind me to (.+)",
                @"add reminder to (.+)",
                @"set reminder for (.+)"
            };

            foreach (string pattern in patterns)
            {
                var match = Regex.Match(input, pattern);
                if (match.Success && match.Groups.Count > 1)
                {
                    return match.Groups[1].Value.Trim();
                }
            }

            string[] removeWords = { "add", "task", "new", "a", "to", "remind", "me", "reminder", "set", "for" };
            string result = input;
            foreach (string word in removeWords)
            {
                result = result.Replace(word, "").Replace("  ", " ");
            }

            return result.Trim();
        }

        public string ExtractReminderTime(string userInput)
        {
            string input = userInput.ToLower();

            var match = Regex.Match(input, @"(\d+)\s*days?");
            if (match.Success)
                return match.Groups[1].Value + " days";

            if (input.Contains("tomorrow"))
                return "tomorrow";

            match = Regex.Match(input, @"(\d{4}-\d{2}-\d{2})");
            if (match.Success)
                return match.Groups[1].Value;

            if (input.Contains("now") || input.Contains("today"))
                return "today";

            return "unknown";
        }

        public string GetResponseForIntent(string intent, string userInput)
        {
            switch (intent)
            {
                case "add_task":
                    string task = ExtractTaskTitle(userInput);
                    return $"I'll add '{task}' to your task list. Would you like to set a reminder?";
                case "add_reminder":
                    string reminder = ExtractTaskTitle(userInput);
                    string time = ExtractReminderTime(userInput);
                    return $"I'll remind you to '{reminder}' {(time != "unknown" ? $"in {time}" : "")}.";
                case "show_tasks":
                    return "Here are your current tasks:";
                case "show_log":
                    return "Here is your activity log:";
                case "start_quiz":
                    return "Let's start the cybersecurity quiz! Are you ready?";
                case "password_topic":
                    return "I can help you with password safety! Here are some tips...";
                case "phishing_topic":
                    return "Let's talk about phishing prevention! Here's what you need to know...";
                case "browsing_topic":
                    return "Here are some safe browsing tips...";
                case "help":
                    return "I can help you with:\n" +
                           "• Tasks - Add, complete, or delete tasks\n" +
                           "• Quiz - Start a cybersecurity quiz\n" +
                           "• Reminders - Set reminders for tasks\n" +
                           "• Activity Log - See what I've done for you";
                default:
                    return null;
            }
        }
    }
}