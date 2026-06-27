using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace cyber_security_bottttt
{
    public class NLPSimulator
    {
        public enum Intent
        {
            AddTask,
            AddReminder,
            ShowTasks,
            DeleteTask,
            CompleteTask,
            StartQuiz,
            ShowActivityLog,
            ShowFullLog,
            Help,
            Unknown
        }

        public struct NLPResult
        {
            public Intent Intent { get; set; }
            public string ExtractedTitle { get; set; }
            public string ExtractedDescription { get; set; }
            public int ExtractedDays { get; set; }
            public int ExtractedTaskId { get; set; }
        }

        public NLPResult ProcessInput(string input)
        {
            input = input.ToLower().Trim();
            NLPResult result = new NLPResult();
            result.Intent = Intent.Unknown;
            result.ExtractedDays = -1;
            result.ExtractedTaskId = -1;

            // Check for show full log first (more specific)
            if (input.Contains("show more") || input.Contains("full log") || input.Contains("complete log"))
            {
                result.Intent = Intent.ShowFullLog;
                return result;
            }

            if (IsAddTaskIntent(input))
            {
                result.Intent = Intent.AddTask;
                ExtractTaskDetails(input, ref result);
            }
            else if (IsReminderIntent(input))
            {
                result.Intent = Intent.AddReminder;
                ExtractTaskDetails(input, ref result);
                ExtractDays(input, ref result);
            }
            else if (IsShowTasksIntent(input))
            {
                result.Intent = Intent.ShowTasks;
            }
            else if (IsDeleteTaskIntent(input))
            {
                result.Intent = Intent.DeleteTask;
                ExtractTaskId(input, ref result);
            }
            else if (IsCompleteTaskIntent(input))
            {
                result.Intent = Intent.CompleteTask;
                ExtractTaskId(input, ref result);
            }
            else if (IsQuizIntent(input))
            {
                result.Intent = Intent.StartQuiz;
            }
            else if (IsActivityLogIntent(input))
            {
                result.Intent = Intent.ShowActivityLog;
            }
            else if (IsHelpIntent(input))
            {
                result.Intent = Intent.Help;
            }

            return result;
        }

        private bool IsAddTaskIntent(string input)
        {
            return Regex.IsMatch(input, @"\b(add|create|new)\s+\b(task|todo|to do)\b") ||
                   Regex.IsMatch(input, @"\badd\s+.*\s+task\b") ||
                   input.Contains("add task") ||
                   input.Contains("new task") ||
                   input.Contains("create task");
        }

        private bool IsReminderIntent(string input)
        {
            return Regex.IsMatch(input, @"\b(remind|reminder)\b") ||
                   Regex.IsMatch(input, @"\bset\s+reminder\b") ||
                   input.Contains("remind me");
        }

        private bool IsShowTasksIntent(string input)
        {
            return Regex.IsMatch(input, @"\b(show|view|list|display)\s+(my\s+)?tasks?\b") ||
                   input.Contains("show task") ||
                   input.Contains("view task") ||
                   input.Contains("list task") ||
                   input.Contains("my tasks");
        }

        private bool IsDeleteTaskIntent(string input)
        {
            return Regex.IsMatch(input, @"\b(delete|remove)\s+(task\s+#?\d+)\b") ||
                   Regex.IsMatch(input, @"\bdelete task\b") ||
                   Regex.IsMatch(input, @"\bremove task\b") ||
                   input.Contains("delete task");
        }

        private bool IsCompleteTaskIntent(string input)
        {
            return Regex.IsMatch(input, @"\b(complete|finish|done)\s+(task\s+#?\d+)\b") ||
                   Regex.IsMatch(input, @"\bmark\s+as\s+complete\b") ||
                   input.Contains("complete task") ||
                   input.Contains("task complete");
        }

        private bool IsQuizIntent(string input)
        {
            return Regex.IsMatch(input, @"\b(quiz|game|play)\b") ||
                   input.Contains("take quiz") ||
                   input.Contains("start quiz") ||
                   input.Contains("cybersecurity quiz") ||
                   input.Contains("let's play");
        }

        private bool IsActivityLogIntent(string input)
        {
            return Regex.IsMatch(input, @"\b(activity|action)\s+log\b") ||
                   input.Contains("show activity") ||
                   input.Contains("what have you done") ||
                   input.Contains("show log") ||
                   input.Contains("view log");
        }

        private bool IsHelpIntent(string input)
        {
            return Regex.IsMatch(input, @"\bhelp\b") ||
                   Regex.IsMatch(input, @"^what can you do") ||
                   Regex.IsMatch(input, @"^commands") ||
                   input.Contains("how to use");
        }

        private void ExtractTaskDetails(string input, ref NLPResult result)
        {
            string text = input;

            foreach (string prefix in new[] { "add task ", "add a task ", "create task ", "create a task ", "new task ", "remind me to ", "set reminder " })
            {
                if (text.StartsWith(prefix))
                {
                    text = text.Substring(prefix.Length);
                    break;
                }
            }

            if (text.Contains(" to "))
            {
                string[] parts = text.Split(new[] { " to " }, StringSplitOptions.None);
                result.ExtractedTitle = parts[0].Trim();
                if (parts.Length > 1)
                    result.ExtractedDescription = parts[1].Trim();
                else
                    result.ExtractedDescription = "";
            }
            else if (text.Contains(" - "))
            {
                string[] parts = text.Split(new[] { " - " }, StringSplitOptions.None);
                result.ExtractedTitle = parts[0].Trim();
                if (parts.Length > 1)
                    result.ExtractedDescription = parts[1].Trim();
                else
                    result.ExtractedDescription = "";
            }
            else
            {
                result.ExtractedTitle = text.Trim();
                result.ExtractedDescription = "";
            }

            foreach (string phrase in new[] { " set reminder", " remind me", " add reminder", " create reminder" })
            {
                if (result.ExtractedTitle.Contains(phrase))
                {
                    result.ExtractedTitle = result.ExtractedTitle.Replace(phrase, "").Trim();
                }
            }

            if (string.IsNullOrEmpty(result.ExtractedTitle))
            {
                result.ExtractedTitle = "Cybersecurity task";
            }
        }

        private void ExtractDays(string input, ref NLPResult result)
        {
            var match = Regex.Match(input, @"(\d+)\s+(day|days|week|weeks)");
            if (match.Success)
            {
                int days = int.Parse(match.Groups[1].Value);
                if (match.Groups[2].Value.Contains("week"))
                    days *= 7;
                result.ExtractedDays = days;
            }

            if (input.Contains("tomorrow"))
                result.ExtractedDays = 1;

            if (input.Contains("next week"))
                result.ExtractedDays = 7;
        }

        private void ExtractTaskId(string input, ref NLPResult result)
        {
            var match = Regex.Match(input, @"#?(\d+)");
            if (match.Success)
            {
                result.ExtractedTaskId = int.Parse(match.Groups[1].Value);
            }
        }
    }
}