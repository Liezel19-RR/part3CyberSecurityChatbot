using part3.classes;
using System;
using System.Collections.Generic;
using System.Text;

namespace cyber_security_bottttt
{
    /*
     * ================================================================
     * CHATBOT CLASS
     * ================================================================
     * This is the main chatbot class that handles all user interactions.
     * It uses ResponseHandler for processing and delegates all
     * functionality to the appropriate components.
     * 
     * Part 2 Features:
     * - Topic-based responses (Password, Phishing, Safe Browsing)
     * - Emotion detection
     * - Voice playback
     * 
     * Part 3 Features:
     * - Task Management (Add, View, Delete, Complete)
     * - Cybersecurity Quiz
     * - NLP Simulation
     * - Activity Logging
     * - Memory
     */
    public class ChatBot
    {
        // Core components
        private ResponseHandler responseHandler;
        private Emotion emotion;
        private VoicePlayer voicePlayer;
        private DatabaseHelper db;
        private ActivityLogService activityLog;
        private NLPSimulator nlp;
        private QuizData quiz;
        private Memory memory;

        // Quiz state
        private bool isQuizActive = false;
        private QuizQuestion currentQuestion = null;

        // Properties
        public string BotName { get; set; } = "CyberGuard";
        public string Version { get; set; } = "3.0";

        // Constructor
        public ChatBot()
        {
            // Initialize all components
            responseHandler = new ResponseHandler();
            emotion = new Emotion();
            voicePlayer = new VoicePlayer();
            db = new DatabaseHelper();
            activityLog = new ActivityLogService();
            nlp = new NLPSimulator();
            quiz = new QuizData();
            memory = new Memory();

            // Log initialization
            activityLog.LogAction("ChatBot initialized.");
            activityLog.LogAction($"ChatBot Version: {Version}");

            System.Diagnostics.Debug.WriteLine($"🤖 {BotName} v{Version} initialized.");
        }

        // ============================================================
        // MAIN METHOD: Get Response
        // ============================================================
        public string GetResponse(string input)
        {
            if (string.IsNullOrWhiteSpace(input))
            {
                return "I didn't catch that. Could you please say something? 🤔";
            }

            try
            {
                // Process input through ResponseHandler (includes NLP)
                string response = responseHandler.ProcessInput(input);

                // Add emotion detection if applicable
                string emotionResponse = emotion.GetEmotion(input);
                if (!string.IsNullOrEmpty(emotionResponse))
                {
                    response += "\n" + emotionResponse;
                }

                // Log the interaction
                activityLog.LogAction($"User input: '{input}'");

                return response;
            }
            catch (Exception ex)
            {
                activityLog.LogAction($"ERROR: {ex.Message}");
                return $"⚠️ I encountered an error: {ex.Message}\nPlease try again or type 'help' for assistance.";
            }
        }

        // ============================================================
        // PART 2: Topic-Based Responses
        // ============================================================
        public string GetTopicResponse(string topic)
        {
            topic = topic.ToLower().Trim();

            switch (topic)
            {
                case "how are you":
                    activityLog.LogAction("User asked: How are you");
                    return "I'm functioning perfectly 😊 How can I help you with cybersecurity today?";

                case "purpose":
                    activityLog.LogAction("User asked: Purpose");
                    return "My purpose is to teach cyber security awareness and help you manage your security tasks. I'm your personal cybersecurity assistant! 🔒";

                case "topics":
                    activityLog.LogAction("User asked: Topics");
                    return "📚 I can help you with:\n" +
                           "   🔐 Password Safety\n" +
                           "   🎣 Phishing Detection\n" +
                           "   🌐 Safe Browsing\n" +
                           "   📋 Task Management\n" +
                           "   🧠 Cybersecurity Quiz\n" +
                           "   📊 Activity Log\n" +
                           "   🧠 Memory\n\n" +
                           "Type 'help' for all commands!";

                case "password safety":
                    activityLog.LogAction("User asked: Password Safety");
                    return GetPasswordInfo();

                case "phishing":
                    activityLog.LogAction("User asked: Phishing");
                    return GetPhishingInfo();

                case "safe browsing":
                    activityLog.LogAction("User asked: Safe Browsing");
                    return GetSafeBrowsingInfo();

                case "exit":
                    activityLog.LogAction("User selected: Exit");
                    return "Goodbye 👋 Stay safe online!";

                default:
                    return "I don't have information on that specific topic. Try: password safety, phishing, or safe browsing.";
            }
        }

        // ============================================================
        // PART 2: Security Topic Information
        // ============================================================
        private string GetPasswordInfo()
        {
            return @"🔐 **PASSWORD SAFETY TIPS:**

✅ **DO's:**
   • Use at least 12 characters
   • Mix uppercase, lowercase, numbers, and symbols
   • Use unique passwords for each account
   • Consider using a password manager
   • Enable Two-Factor Authentication (2FA)

❌ **DON'Ts:**
   • Don't use common words or personal info
   • Don't use 'password' or '123456'
   • Don't reuse passwords across sites
   • Don't share passwords with anyone
   • Don't write passwords on sticky notes

💡 **Pro Tip:** Use passphrases like 'MyCatLovesTuna!2024' - they're strong AND memorable!";
        }

        private string GetPhishingInfo()
        {
            return @"🎣 **PHISHING DETECTION:**

⚠️ **Red Flags:**
   • Urgent or threatening language
   • Requests for personal information
   • Suspicious sender email addresses
   • Poor spelling and grammar
   • Unexpected attachments or links
   • Too-good-to-be-true offers

✅ **What to Do:**
   • Hover over links to see the real URL
   • Verify with the company directly
   • Report phishing emails
   • Never click suspicious links
   • Never reply with sensitive info

💡 **Remember:** If it looks suspicious, it probably is! When in doubt, throw it out!";
        }

        private string GetSafeBrowsingInfo()
        {
            return @"🌐 **SAFE BROWSING TIPS:**

✅ **DO's:**
   • Look for HTTPS and padlock icon
   • Use updated browsers
   • Enable private browsing for sensitive sites
   • Use a VPN on public Wi-Fi
   • Clear cookies and cache regularly

❌ **DON'Ts:**
   • Don't enter personal info on HTTP sites
   • Don't ignore security warnings
   • Don't download from untrusted sources
   • Don't save passwords in browsers
   • Don't click pop-up ads

💡 **Pro Tip:** Always check the URL carefully - cybercriminals use fake domains like 'amaz0n.com' instead of 'amazon.com'!";
        }

        // ============================================================
        // PART 3: Task Management
        // ============================================================
        public string AddTask(string title, string description = "")
        {
            if (string.IsNullOrWhiteSpace(title))
                return "Please provide a title for the task.";

            if (db.AddTask(title, description))
            {
                activityLog.LogAction($"Task added: '{title}'");
                return $"✅ Task added successfully!\n   Title: {title}\n" +
                       (string.IsNullOrEmpty(description) ? "" : $"   Description: {description}\n") +
                       "\n💡 Type 'remind me' to set a reminder.";
            }
            else
            {
                return "❌ Failed to add task. Please check MySQL connection.";
            }
        }

        public string ShowTasks()
        {
            var tasks = db.GetAllTasks();
            if (tasks.Count == 0)
                return "📭 You have no tasks yet. Add a task to get started!";

            string result = "📋 YOUR CYBERSECURITY TASKS:\n";
            result += $"📊 Total: {tasks.Count}\n\n";

            int completed = 0;
            foreach (var task in tasks)
            {
                if (task.IsCompleted) completed++;
                result += task.ToString() + "\n";
            }

            result += $"\n✅ Completed: {completed} | ⏳ Pending: {tasks.Count - completed}";
            result += "\n\n💡 Commands: 'delete task #' | 'complete task #'";
            return result;
        }

        public string DeleteTask(int id)
        {
            if (id <= 0)
                return "Please specify a valid task ID.";

            if (db.DeleteTask(id))
            {
                activityLog.LogAction($"Task #{id} deleted");
                return $"🗑️ Task #{id} has been deleted.";
            }
            else
            {
                return $"❌ Task #{id} not found or could not be deleted.";
            }
        }

        public string CompleteTask(int id)
        {
            if (id <= 0)
                return "Please specify a valid task ID.";

            if (db.MarkTaskCompleted(id))
            {
                activityLog.LogAction($"Task #{id} marked as completed");
                return $"✅ Task #{id} has been marked as completed! 🎉\nGood job!";
            }
            else
            {
                return $"❌ Task #{id} not found or could not be completed.";
            }
        }

        public string SetReminder(string taskTitle, int days)
        {
            if (string.IsNullOrWhiteSpace(taskTitle))
                return "Please specify which task to set a reminder for.";

            if (days <= 0)
                days = 7;

            DateTime reminderDate = DateTime.Now.AddDays(days);

            var tasks = db.GetAllTasks();
            TaskItem matchingTask = null;

            foreach (var task in tasks)
            {
                if (!task.IsCompleted && task.Title.ToLower().Contains(taskTitle.ToLower()))
                {
                    matchingTask = task;
                    break;
                }
            }

            if (matchingTask != null)
            {
                if (db.UpdateTaskReminder(matchingTask.Id, reminderDate))
                {
                    activityLog.LogAction($"Reminder set for task '{matchingTask.Title}' in {days} days");
                    return $"📅 Reminder set for '{matchingTask.Title}'\n" +
                           $"   ⏰ {reminderDate:yyyy-MM-dd HH:mm}";
                }
            }
            else
            {
                // Create new task with reminder
                if (db.AddTask(taskTitle, "", reminderDate))
                {
                    activityLog.LogAction($"Task with reminder added: '{taskTitle}'");
                    return $"✅ New task added with reminder: '{taskTitle}'\n" +
                           $"   📅 {reminderDate:yyyy-MM-dd HH:mm}";
                }
            }

            return "❌ Could not set reminder. Please try again.";
        }

        // ============================================================
        // PART 3: Quiz
        // ============================================================
        public string StartQuiz()
        {
            quiz.ResetQuiz();
            isQuizActive = true;
            activityLog.LogAction("Quiz started");

            if (!quiz.HasNextQuestion())
                return "I don't have enough questions ready. Try again later.";

            currentQuestion = quiz.GetNextQuestion();
            return DisplayQuizQuestion();
        }

        public string AnswerQuiz(string input)
        {
            if (!isQuizActive || currentQuestion == null)
                return "No quiz is currently active. Type 'quiz' to start one!";

            // Allow quitting
            if (input.ToLower() == "quit" || input.ToLower() == "stop" || input.ToLower() == "exit")
            {
                isQuizActive = false;
                int answered = quiz.GetQuestionsAnswered();
                int score = quiz.GetCurrentScore();
                currentQuestion = null;
                activityLog.LogAction($"Quiz ended early - {score}/{answered} correct");

                if (answered > 0)
                {
                    return $"🛑 Quiz ended.\n" +
                           $"📊 You answered {answered} questions with {score} correct.\n" +
                           $"💡 Come back anytime to try again! 🧠";
                }
                else
                {
                    return "🛑 Quiz ended. No questions were answered.";
                }
            }

            if (int.TryParse(input, out int answerIndex))
            {
                answerIndex--;
                if (answerIndex >= 0 && answerIndex < currentQuestion.Options.Count)
                {
                    bool isCorrect = quiz.CheckAnswer(currentQuestion, answerIndex);
                    string feedback = isCorrect ? "✅ Correct!" :
                        $"❌ Incorrect. The correct answer is: {currentQuestion.Options[currentQuestion.CorrectAnswerIndex]}";
                    feedback += $"\n📖 {currentQuestion.Explanation}";

                    string answerType = isCorrect ? "Correct" : "Incorrect";
                    activityLog.LogAction($"Quiz answer: {answerType} - {currentQuestion.Question}");

                    if (!quiz.HasNextQuestion())
                    {
                        isQuizActive = false;
                        currentQuestion = null;
                        string finalMessage = quiz.GetFinalMessage();
                        activityLog.LogAction($"Quiz completed with {quiz.GetCurrentScore()}/{quiz.GetQuestionsAnswered()} correct");
                        return feedback + "\n\n🏁 QUIZ COMPLETE!\n" + finalMessage;
                    }
                    else
                    {
                        currentQuestion = quiz.GetNextQuestion();
                        return feedback + "\n\n" + DisplayQuizQuestion();
                    }
                }
                else
                {
                    return "Please enter a valid number from the options above.";
                }
            }
            else
            {
                return "Please enter the number of your answer (1, 2, 3, etc.) or type 'quit' to stop the quiz.";
            }
        }

        private string DisplayQuizQuestion()
        {
            if (currentQuestion == null)
                return "No question available.";

            string result = $"🧠 CYBERSECURITY QUIZ\n";
            result += $"📝 Question {quiz.GetQuestionsAnswered()} of 10\n\n";
            result += $"{currentQuestion.Question}\n\n";

            if (currentQuestion.IsTrueFalse)
            {
                result += "   Type '1' for True or '2' for False\n";
            }
            else
            {
                for (int i = 0; i < currentQuestion.Options.Count; i++)
                {
                    result += $"   {i + 1}. {currentQuestion.Options[i]}\n";
                }
            }
            result += "\n💡 Type your answer (1, 2, etc.) or 'quit' to stop";
            return result;
        }

        public bool IsQuizActive()
        {
            return isQuizActive;
        }

        public int GetQuizScore()
        {
            return quiz.GetCurrentScore();
        }

        public int GetQuizQuestionsAnswered()
        {
            return quiz.GetQuestionsAnswered();
        }

        public string GetQuizFinalMessage()
        {
            return quiz.GetFinalMessage();
        }

        // ============================================================
        // PART 3: Activity Log
        // ============================================================
        public string GetActivityLogSummary()
        {
            return activityLog.GetLogSummary();
        }

        public string GetFullActivityLog()
        {
            return activityLog.GetFullLogSummary();
        }

        public string ExportActivityLog()
        {
            return activityLog.ExportLogsToFile();
        }

        public string ClearActivityLog()
        {
            activityLog.ClearLogs();
            return "🗑️ Activity log cleared.";
        }

        public int GetLogCount()
        {
            return activityLog.GetLogCount();
        }

        // ============================================================
        // PART 3: Memory
        // ============================================================
        public string StoreMemory(string text)
        {
            if (string.IsNullOrWhiteSpace(text))
                return "Please tell me something to remember.";

            memory.StoreMemory(text);
            activityLog.LogAction($"Memory stored: '{text}'");
            return $"🧠 I'll remember that: '{text}'";
        }

        public string ShowMemory()
        {
            return memory.ShowMemory();
        }

        public string ClearMemory()
        {
            memory.ClearMemory();
            activityLog.LogAction("Memory cleared");
            return "🧠 Memory has been cleared.";
        }

        public int GetMemoryCount()
        {
            return memory.GetMemoryCount();
        }

        public List<string> GetAllMemories()
        {
            return memory.GetAllMemories();
        }

        // ============================================================
        // PART 2: Voice
        // ============================================================
        public string PlayVoice()
        {
            try
            {
                voicePlayer.PlayVoice();
                activityLog.LogAction("Voice played");
                return "🔊 Voice played!";
            }
            catch (Exception ex)
            {
                activityLog.LogAction($"Voice error: {ex.Message}");
                return $"❌ Could not play voice: {ex.Message}";
            }
        }

        // ============================================================
        // PART 3: Database
        // ============================================================
        public bool TestDatabaseConnection()
        {
            return db.TestConnection();
        }

        public int GetTaskCount()
        {
            return db.GetAllTasks().Count;
        }

        public List<TaskItem> GetAllTasks()
        {
            return db.GetAllTasks();
        }

        // ============================================================
        // PART 2: Emotion
        // ============================================================
        public string DetectEmotion(string input)
        {
            return emotion.GetEmotion(input);
        }

        // ============================================================
        // PART 3: NLP
        // ============================================================
        public NLPSimulator.NLPResult ProcessNLP(string input)
        {
            return nlp.ProcessInput(input);
        }

        // ============================================================
        // Utility Methods
        // ============================================================
        public string GetHelp()
        {
            return @"🤖 **AVAILABLE COMMANDS**

📋 **TASK MANAGEMENT:**
   • `add task [title]` - Add a cybersecurity task
   • `add task [title] - [description]` - Add task with description
   • `remind me [task] in [days] days` - Set a reminder
   • `show tasks` - View all your tasks
   • `delete task #` - Delete a task by ID
   • `complete task #` - Mark a task as done

🧠 **QUIZ:**
   • `quiz` or `start quiz` - Take a cybersecurity quiz
   • `quit` - Exit the quiz

📊 **ACTIVITY LOG:**
   • `activity log` - View recent activity (last 10)
   • `show more` or `full log` - View complete history
   • `export logs` - Export logs to file
   • `clear logs` - Clear all logs

🧠 **MEMORY:**
   • `remember [something]` - Store a memory
   • `what do you remember` - Show all memories
   • `clear memory` - Clear all memories

❓ **OTHER:**
   • `help` - Show this help menu
   • `exit` - Quit the chatbot

📚 **TOPICS:**
   • Ask about: password safety, phishing, safe browsing

💡 **TIP:** Try typing naturally - I understand variations!";
        }

        public string GetBotInfo()
        {
            return $"🤖 {BotName} v{Version}\n" +
                   $"📅 Running since: {DateTime.Now:yyyy-MM-dd HH:mm}\n" +
                   $"📋 Tasks: {GetTaskCount()}\n" +
                   $"🧠 Memory: {GetMemoryCount()}\n" +
                   $"📊 Log Entries: {GetLogCount()}";
        }

        // ============================================================
        // ResponseHandler Access
        // ============================================================
        public ResponseHandler GetResponseHandler()
        {
            return responseHandler;
        }

        public void LogActivity(string action)
        {
            activityLog.LogAction(action);
        }
    }
}