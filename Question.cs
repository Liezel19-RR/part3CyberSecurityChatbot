using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace part3.classes
{
    internal class Class3
    {
    }
}

namespace CybersecurityAwarenessBot.Models
{
    /// <summary>
    /// Represents a cybersecurity quiz question
    /// </summary>
    public class Question
    {
        /// <summary>
        /// The question text displayed to the user
        /// </summary>
        public string Text { get; set; }

        /// <summary>
        /// List of answer options (A, B, C, D)
        /// </summary>
        public List<string> Options { get; set; }

        /// <summary>
        /// Index (0-3) of the correct answer in the Options list
        /// </summary>
        public int CorrectAnswerIndex { get; set; }

        /// <summary>
        /// Explanation shown after user answers, to reinforce learning
        /// </summary>
        public string Explanation { get; set; }

        /// <summary>
        /// Category of the question (e.g., "Phishing", "Passwords", "Browsing")
        /// </summary>
        public string Category { get; set; }

        /// <summary>
        /// Constructor to create a quiz question
        /// </summary>
        public Question(string text, List<string> options, int correctIndex, string explanation, string category = "General")
        {
            Text = text;
            Options = options;
            CorrectAnswerIndex = correctIndex;
            Explanation = explanation;
            Category = category;
        }
    }
}