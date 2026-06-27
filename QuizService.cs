using CybersecurityAwarenessBot.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace part3.classes
{
    internal class QuizService
    {
    }
}


namespace CybersecurityAwarenessBot.Services
{
    /// <summary>
    /// Handles all quiz-related logic including questions, scoring, and feedback
    /// </summary>
    public class QuizService
    {
        private List<Question> _questions;
        private int _currentQuestionIndex;
        private int _score;
        private bool _isQuizActive;
        private Random _random;

        public QuizService()
        {
            _random = new Random();
            _questions = new List<Question>();
            _isQuizActive = false;
            _score = 0;
            _currentQuestionIndex = 0;
            LoadQuestions();
        }

        private void LoadQuestions()
        {
            _questions = new List<Question>
            {
                new Question(
                    "What should you do if you receive an email asking for your password?",
                    new List<string> { "Reply with your password", "Delete the email", "Report it as phishing", "Ignore it" },
                    2,
                    "Reporting phishing emails helps prevent scams and protects others.",
                    "Phishing"
                ),
                new Question(
                    "What is a strong password?",
                    new List<string> { "Your pet's name", "123456", "A mix of letters, numbers, and symbols", "Your birthday" },
                    2,
                    "Strong passwords use a combination of uppercase, lowercase, numbers, and special characters.",
                    "Passwords"
                ),
                new Question(
                    "What does HTTPS stand for?",
                    new List<string> { "Hyper Text Transfer Protocol Secure", "High Tech Transfer Protocol", "Hyper Transfer Text Secure", "None of the above" },
                    0,
                    "HTTPS stands for Hyper Text Transfer Protocol Secure, which encrypts data between your browser and website.",
                    "Browsing"
                ),
                new Question(
                    "True or False: You should use the same password for all your accounts.",
                    new List<string> { "True", "False" },
                    1,
                    "Using the same password for all accounts is dangerous. If one account is compromised, all are at risk.",
                    "Passwords"
                ),
                new Question(
                    "What is a common sign of a phishing email?",
                    new List<string> { "Professional design", "Urgent language asking for action", "Correct spelling", "Known sender" },
                    1,
                    "Phishing emails often use urgent language to pressure you into acting quickly without thinking.",
                    "Phishing"
                ),
                new Question(
                    "What should you do on public Wi-Fi?",
                    new List<string> { "Do online banking", "Use a VPN", "Share passwords", "Click all ads" },
                    1,
                    "Always use a VPN on public Wi-Fi to protect your data from hackers.",
                    "Browsing"
                ),
                new Question(
                    "Two-Factor Authentication (2FA) adds an extra layer of security. What is 2FA?",
                    new List<string> { "Two passwords", "Password + code from your phone", "Two usernames", "Fingerprint only" },
                    1,
                    "2FA requires something you know (password) and something you have (phone code) to log in.",
                    "Passwords"
                ),
                new Question(
                    "Social engineering is...",
                    new List<string> { "Engineering software", "A type of virus", "Manipulating people to give information", "Network security" },
                    2,
                    "Social engineering tricks people into revealing confidential information through manipulation.",
                    "General"
                ),
                new Question(
                    "True or False: It's safe to click on links in messages from unknown numbers.",
                    new List<string> { "True", "False" },
                    1,
                    "Never click links from unknown numbers. They could lead to phishing sites or malware.",
                    "Phishing"
                ),
                new Question(
                    "What does a padlock icon in your browser address bar indicate?",
                    new List<string> { "The site is safe to visit", "Your password is saved", "The connection is secure (HTTPS)", "You are logged in" },
                    2,
                    "The padlock icon indicates a secure connection using HTTPS, meaning your data is encrypted.",
                    "Browsing"
                ),
                new Question(
                    "What is the best way to store passwords?",
                    new List<string> { "In a text file", "On a sticky note", "Using a password manager", "In your email" },
                    2,
                    "Password managers securely store and generate strong passwords for all your accounts.",
                    "Passwords"
                ),
                new Question(
                    "True or False: Public computers are safe for online banking.",
                    new List<string> { "True", "False" },
                    1,
                    "Public computers may have keyloggers or malware that can steal your banking information.",
                    "Browsing"
                )
            };
        }

        public List<Question> GetQuestions()
        {
            return _questions;
        }

        public Question GetCurrentQuestion()
        {
            if (_currentQuestionIndex < _questions.Count)
                return _questions[_currentQuestionIndex];
            return null;
        }

        public bool SubmitAnswer(int selectedIndex)
        {
            if (!_isQuizActive || _currentQuestionIndex >= _questions.Count)
                return false;

            var question = _questions[_currentQuestionIndex];
            bool isCorrect = selectedIndex == question.CorrectAnswerIndex;

            if (isCorrect)
                _score++;

            _currentQuestionIndex++;

            if (_currentQuestionIndex >= _questions.Count)
                _isQuizActive = false;

            return isCorrect;
        }

        public void StartQuiz()
        {
            _questions = _questions.OrderBy(q => _random.Next()).ToList();
            _currentQuestionIndex = 0;
            _score = 0;
            _isQuizActive = true;
        }

        public bool IsQuizActive()
        {
            return _isQuizActive;
        }

        public int GetScore()
        {
            return _score;
        }

        public int GetTotalQuestions()
        {
            return _questions.Count;
        }

        public int GetCurrentQuestionNumber()
        {
            return _currentQuestionIndex + 1;
        }

        public bool IsQuizComplete()
        {
            return !_isQuizActive && _currentQuestionIndex >= _questions.Count;
        }

        public string GetFeedback(int score)
        {
            int percentage = (score * 100) / _questions.Count;

            if (percentage >= 90)
                return "🌟 Excellent! You're a Cybersecurity Pro! 🏆";
            else if (percentage >= 70)
                return "👏 Good job! You know your cybersecurity well!";
            else if (percentage >= 50)
                return "📚 Not bad! Keep learning to stay safe online!";
            else
                return "🔒 Keep learning! Cybersecurity is important for everyone!";
        }
    }
}