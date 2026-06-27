
using System;
using System.Collections.Generic;

namespace cyber_security_bottttt
{
    public class QuizQuestion
    {
        public string Question { get; set; }
        public List<string> Options { get; set; }
        public int CorrectAnswerIndex { get; set; }
        public string Explanation { get; set; }
        public bool IsTrueFalse { get; set; }

        public QuizQuestion(string question, List<string> options, int correctIndex, string explanation, bool isTrueFalse = false)
        {
            Question = question;
            Options = options;
            CorrectAnswerIndex = correctIndex;
            Explanation = explanation;
            IsTrueFalse = isTrueFalse;
        }
    }

    public class QuizData
    {
        private List<QuizQuestion> questions;
        private Random random = new Random();
        private List<int> questionOrder;
        private int currentIndex = 0;
        private int score = 0;

        public QuizData()
        {
            InitializeQuestions();
            ShuffleQuestions();
        }

        private void InitializeQuestions()
        {
            questions = new List<QuizQuestion>
            {
                // Multiple Choice Questions
                new QuizQuestion(
                    "What should you do if you receive an email asking for your password?",
                    new List<string> { "Reply with your password", "Delete the email", "Report the email as phishing", "Ignore it" },
                    2,
                    "Reporting phishing emails helps prevent scams and protects others from falling victim."
                ),
                new QuizQuestion(
                    "Which of the following is a strong password?",
                    new List<string> { "123456", "password", "P@ssw0rd!2024", "qwerty" },
                    2,
                    "A strong password includes a mix of uppercase, lowercase, numbers, and special characters."
                ),
                new QuizQuestion(
                    "What does HTTPS stand for?",
                    new List<string> { "Hyper Text Transfer Protocol Secure", "High Technology Transfer System", "Hyper Transfer Text Protocol", "None of the above" },
                    0,
                    "HTTPS ensures secure communication between your browser and the website."
                ),
                new QuizQuestion(
                    "What is phishing?",
                    new List<string> { "A type of fishing sport", "A cyber attack using fraudulent emails", "A type of computer virus", "A programming language" },
                    1,
                    "Phishing is a cyber attack where attackers send fraudulent emails to trick users into revealing sensitive information."
                ),
                new QuizQuestion(
                    "What is social engineering in cybersecurity?",
                    new List<string> { "Engineering social networks", "Manipulating people to reveal confidential information", "A type of malware", "Building secure social platforms" },
                    1,
                    "Social engineering involves psychological manipulation to trick people into divulging confidential information."
                ),
                new QuizQuestion(
                    "What should you check before entering personal information on a website?",
                    new List<string> { "The website's design", "The HTTPS and padlock icon", "The website's domain age", "The number of visitors" },
                    1,
                    "Always check for HTTPS and the padlock icon in the address bar to ensure the connection is secure."
                ),
                new QuizQuestion(
                    "What is MFA (Multi-Factor Authentication)?",
                    new List<string> { "One-time password", "Using multiple verification methods", "A type of firewall", "A password manager" },
                    1,
                    "MFA adds an extra layer of security by requiring two or more verification methods before granting access."
                ),
                new QuizQuestion(
                    "What is ransomware?",
                    new List<string> { "Software that encrypts your files for ransom", "A type of antivirus", "A password recovery tool", "A backup software" },
                    0,
                    "Ransomware is malware that encrypts your files and demands payment for their release."
                ),
                new QuizQuestion(
                    "What is the best way to create a memorable but strong password?",
                    new List<string> { "Use a password manager", "Use a common word with numbers", "Use a sentence or passphrase", "Use your pet's name" },
                    2,
                    "Using a passphrase (like 'MyCatLovesTuna!2024') is both strong and memorable."
                ),
                new QuizQuestion(
                    "What is a VPN used for?",
                    new List<string> { "To increase internet speed", "To hide your IP address and encrypt data", "To download files faster", "To block all websites" },
                    1,
                    "A VPN (Virtual Private Network) encrypts your internet connection and hides your IP address for privacy and security."
                ),
                new QuizQuestion(
                    "What should you do if you suspect your account has been hacked?",
                    new List<string> { "Ignore it", "Change your password immediately", "Tell your friends", "Post about it on social media" },
                    1,
                    "Immediately change your password and enable 2FA. Also contact the service provider."
                ),

                // True/False Questions
                new QuizQuestion(
                    "True or False: Using the same password for multiple accounts is safe.",
                    new List<string> { "True", "False" },
                    1,
                    "False! Using the same password across multiple accounts is risky. If one account is compromised, all accounts are vulnerable.",
                    true
                ),
                new QuizQuestion(
                    "True or False: Public Wi-Fi networks are always safe to use for banking.",
                    new List<string> { "True", "False" },
                    1,
                    "False! Public Wi-Fi networks are often unsecured and can be intercepted by attackers. Use a VPN or avoid sensitive transactions.",
                    true
                ),
                new QuizQuestion(
                    "True or False: Software updates are optional and can be ignored.",
                    new List<string> { "True", "False" },
                    1,
                    "False! Software updates often include security patches that protect against known vulnerabilities. Always update regularly.",
                    true
                ),
                new QuizQuestion(
                    "True or False: You should share your password with trusted colleagues.",
                    new List<string> { "True", "False" },
                    1,
                    "False! Never share your passwords with anyone, even trusted individuals. Use proper access control methods instead.",
                    true
                ),
                new QuizQuestion(
                    "True or False: Two-factor authentication adds an extra layer of security.",
                    new List<string> { "True", "False" },
                    0,
                    "True! 2FA adds a second verification step, significantly improving account security.",
                    true
                ),
                new QuizQuestion(
                    "True or False: Free public Wi-Fi is encrypted and safe to use.",
                    new List<string> { "True", "False" },
                    1,
                    "False! Most public Wi-Fi networks are not encrypted, making them vulnerable to attacks.",
                    true
                ),
                new QuizQuestion(
                    "True or False: Antivirus software can protect against all types of cyber attacks.",
                    new List<string> { "True", "False" },
                    1,
                    "False! Antivirus is important but doesn't protect against phishing, social engineering, or zero-day attacks. Use multiple security layers.",
                    true
                )
            };
        }

        private void ShuffleQuestions()
        {
            questionOrder = new List<int>();
            for (int i = 0; i < questions.Count; i++)
                questionOrder.Add(i);

            for (int i = questionOrder.Count - 1; i > 0; i--)
            {
                int j = random.Next(i + 1);
                int temp = questionOrder[i];
                questionOrder[i] = questionOrder[j];
                questionOrder[j] = temp;
            }
        }

        public bool HasNextQuestion()
        {
            return currentIndex < questionOrder.Count && currentIndex < 10;
        }

        public QuizQuestion GetNextQuestion()
        {
            if (!HasNextQuestion())
                return null;

            int questionIndex = questionOrder[currentIndex];
            currentIndex++;
            return questions[questionIndex];
        }

        public bool CheckAnswer(QuizQuestion question, int selectedAnswer)
        {
            bool isCorrect = selectedAnswer == question.CorrectAnswerIndex;
            if (isCorrect)
                score++;
            return isCorrect;
        }

        public string GetFinalMessage()
        {
            int total = Math.Min(currentIndex, 10);
            double percentage = total > 0 ? (double)score / total * 100 : 0;

            if (percentage >= 80)
                return $"🎉 Great job! You scored {score}/{total} ({percentage:F0}%)! You're a cybersecurity pro!";
            else if (percentage >= 60)
                return $"👍 Good effort! You scored {score}/{total} ({percentage:F0}%). Keep learning to improve!";
            else if (percentage >= 40)
                return $"📚 You scored {score}/{total} ({percentage:F0}%). Review the basics to strengthen your cybersecurity knowledge.";
            else
                return $"🤔 You scored {score}/{total} ({percentage:F0}%). Consider learning more about cybersecurity to stay safe online!";
        }

        public void ResetQuiz()
        {
            currentIndex = 0;
            score = 0;
            ShuffleQuestions();
        }

        public int GetCurrentScore()
        {
            return score;
        }

        public int GetQuestionsAnswered()
        {
            return Math.Min(currentIndex, 10);
        }
    }
}