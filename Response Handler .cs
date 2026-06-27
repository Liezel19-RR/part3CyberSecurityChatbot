using part3.classes;
using System;
using System.Collections.Generic;
using System.Text;

namespace cyber_security_bottttt
{
    /*
     * ================================================================
     * RESPONSE HANDLER
     * ================================================================
     * This class handles the processing of user input and generates
     * appropriate responses. It acts as a mediator between the
     * ChatBot and other components.
     */
    public class ResponseHandler
    {
        private Password password = new Password();
        private Phishing phishing = new Phishing();
        private SafeBrowsing safeBrowsing = new SafeBrowsing();
        private Emotion emotion = new Emotion();
        private DatabaseHelper db;
        private NLPSimulator nlp;
        private QuizData quiz;
        private ActivityLogService activityLog;
        private Memory memory;

        private bool quizActive = false;
        private QuizQuestion currentQuizQuestion = null;

        public ResponseHandler()
        {
            db = new DatabaseHelper();
            nlp = new NLPSimulator();
            quiz = new QuizData();
            activityLog = new ActivityLogService();  // NEW: Using ActivityLogService
            memory = new Memory();

            activityLog.LogAction("ResponseHandler initialized.");
        }

        public string ProcessInput(string input, string userName = "")
        {
            input = input.ToLower().Trim();

            // Check for memory recall first
            string memoryResponse = memory.RecallMemory(input);
            if (!string.IsNullOrEmpty(memoryResponse))
            {
                activityLog.LogAction($"Memory recalled: '{input}'");
                return memoryResponse;
            }

            // Store user input in memory
            if (!string.IsNullOrEmpty(input))
            {
                memory.StoreMemory(input);
            }

            // Check for quiz responses first
            if (quizActive && currentQuizQuestion != null)
            {
                return ProcessQuizAnswer(input);
            }

            // Check for exit
            if (IsExitCommand(input))
            {
                activityLog.LogAction($"User {userName} exited the chatbot.");
                return "Goodbye 👋 Stay safe online!";
            }

            // Check for help
            if (IsHelpCommand(input))
            {
                return GetHelpMessage();
            }

            // Check for memory commands
            if (IsMemoryCommand(input))
            {
                return HandleMemoryCommand(input);
            }

            // Process with NLP
            var nlpResult = nlp.ProcessInput(input);

            switch (nlpResult.Intent)
            {
                case NLPSimulator.Intent.AddTask:
                    return HandleAddTask(nlpResult);
                case NLPSimulator.Intent.AddReminder:
                    return HandleAddReminder(nlpResult);
                case NLPSimulator.Intent.ShowTasks:
                    return HandleShowTasks();
                case NLPSimulator.Intent.DeleteTask:
                    return HandleDeleteTask(nlpResult);
                case NLPSimulator.Intent.CompleteTask:
                    return HandleCompleteTask(nlpResult);
                case NLPSimulator.Intent.StartQuiz:
                    return HandleStartQuiz();
                case NLPSimulator.Intent.ShowActivityLog:
                    return activityLog.GetLogSummary();
                case NLPSimulator.Intent.ShowFullLog:
                    return activityLog.GetFullLogSummary();
                case NLPSimulator.Intent.Help:
                    return GetHelpMessage();
                default:
                    return GetBasicResponse(input);
            }
        }

        private bool IsExitCommand(string input)
        {
            return input == "exit" || input == "quit" || input == "goodbye" ||
                   input == "bye" || input == "close";
        }

        private bool IsHelpCommand(string input)
        {
            return input == "help" || input == "what can you do" ||
                   input == "commands" || input == "menu";
        }

        private bool IsMemoryCommand(string input)
        {
            return input.Contains("remember") || input.Contains("recall") ||
                   input.Contains("memory") || input.Contains("what do you remember");
        }

        private string HandleMemoryCommand(string input)
        {
            if (input.Contains("what do you remember") || input.Contains("show memory"))
            {
                return memory.ShowMemory();
            }
            else if (input.Contains("clear memory"))
            {
                memory.ClearMemory();
                activityLog.LogAction("Memory cleared by user");
                return "🧠 Memory has been cleared.";
            }
            else if (input.Contains("remember"))
            {
                string text = input.Replace("remember", "").Replace("i want you to", "").Trim();
                if (!string.IsNullOrEmpty(text))
                {
                    memory.StoreMemory(text);
                    activityLog.LogAction($"Memory stored: '{text}'");
                    return $"🧠 I'll remember that: '{text}'";
                }
            }
            return "I can remember things you tell me. Just say 'remember [something]' and I'll store it.";
        }

        private string HandleAddTask(NLPSimulator.NLPResult result)
        {
            string title = result.ExtractedTitle;
            string description = result.ExtractedDescription;

            if (string.IsNullOrEmpty(title))
                return "I couldn't understand the task. Please specify what task you'd like to add.\nExample: 'add task Enable 2FA'";

            if (db.AddTask(title, description))
            {
                activityLog.LogAction($"Task added: '{title}'");
                return $"✅ Task added: '{title}'\n" +
                       (string.IsNullOrEmpty(description) ? "" : $"📝 Description: {description}\n") +
                       "\n💡 Would you like to set a reminder for this task? (Type 'remind me')";
            }
            else
            {
                return "❌ Sorry, I couldn't add the task. Please check MySQL is running and try again.";
            }
        }

        private string HandleAddReminder(NLPSimulator.NLPResult result)
        {
            string title = result.ExtractedTitle;
            int days = result.ExtractedDays;

            if (string.IsNullOrEmpty(title))
                return "I couldn't understand which task you want to set a reminder for.\nExample: 'remind me Enable 2FA in 3 days'";

            if (days <= 0)
                days = 7;

            DateTime reminderDate = DateTime.Now.AddDays(days);

            var tasks = db.GetAllTasks();
            TaskItem matchingTask = null;

            foreach (var task in tasks)
            {
                if (!task.IsCompleted && task.Title.ToLower().Contains(title.ToLower()))
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
                           $"   ⏰ Will remind you on {reminderDate:yyyy-MM-dd HH:mm}";
                }
                else
                {
                    return "❌ Sorry, I couldn't set the reminder. Please try again.";
                }
            }
            else
            {
                if (db.AddTask(title, "", reminderDate))
                {
                    activityLog.LogAction($"Task with reminder added: '{title}' (reminder in {days} days)");
                    return $"✅ New task added with reminder: '{title}'\n" +
                           $"   📅 Reminder set for {reminderDate:yyyy-MM-dd HH:mm}";
                }
                else
                {
                    return "❌ Sorry, I couldn't add the task with reminder. Please try again.";
                }
            }
        }

        private string HandleShowTasks()
        {
            var tasks = db.GetAllTasks();
            if (tasks.Count == 0)
            {
                return "📭 You have no tasks yet. Add a task to get started!";
            }

            string result = "📋 YOUR CYBERSECURITY TASKS:\n";
            result += $"📊 {tasks.Count} total tasks\n\n";

            int completed = 0;
            foreach (var task in tasks)
            {
                if (task.IsCompleted) completed++;
                result += task.ToString() + "\n";
            }

            result += $"\n✅ Completed: {completed} | ⏳ Pending: {tasks.Count - completed}";
            result += "\n\n💡 To delete: 'delete task #' | To complete: 'complete task #'";
            return result;
        }

        private string HandleDeleteTask(NLPSimulator.NLPResult result)
        {
            int id = result.ExtractedTaskId;
            if (id <= 0)
                return "Please specify which task to delete.\nExample: 'delete task 3'";

            var tasks = db.GetAllTasks();
            TaskItem task = tasks.Find(t => t.Id == id);

            if (task == null)
                return $"❌ Task #{id} not found. Use 'show tasks' to see all tasks.";

            if (db.DeleteTask(id))
            {
                activityLog.LogAction($"Task #{id} deleted: '{task.Title}'");
                return $"🗑️ Task #{id} has been deleted.\n   Title: {task.Title}";
            }
            else
            {
                return "❌ Could not delete the task. Please try again.";
            }
        }

        private string HandleCompleteTask(NLPSimulator.NLPResult result)
        {
            int id = result.ExtractedTaskId;
            if (id <= 0)
                return "Please specify which task to complete.\nExample: 'complete task 3'";

            var tasks = db.GetAllTasks();
            TaskItem task = tasks.Find(t => t.Id == id);

            if (task == null)
                return $"❌ Task #{id} not found. Use 'show tasks' to see all tasks.";

            if (task.IsCompleted)
                return $"⏳ Task #{id} is already marked as completed!\n   Title: {task.Title}";

            if (db.MarkTaskCompleted(id))
            {
                activityLog.LogAction($"Task #{id} marked as completed: '{task.Title}'");
                return $"✅ Task #{id} has been marked as completed!\n" +
                       $"   Title: {task.Title}\n" +
                       $"   🎉 Good job! Keep up the great work!\n\n" +
                       $"💡 Would you like to add another task? (Type 'add task')";
            }
            else
            {
                return "❌ Could not mark task as completed. Please try again.";
            }
        }

        private string HandleStartQuiz()
        {
            quiz.ResetQuiz();
            quizActive = true;
            activityLog.LogAction("Quiz started");

            if (!quiz.HasNextQuestion())
            {
                return "I don't have enough questions ready. Try again later.";
            }

            currentQuizQuestion = quiz.GetNextQuestion();
            return DisplayQuizQuestion(currentQuizQuestion);
        }

        private string DisplayQuizQuestion(QuizQuestion question)
        {
            string result = $"🧠 CYBERSECURITY QUIZ\n";
            result += $"📝 Question {quiz.GetQuestionsAnswered()} of 10\n\n";
            result += $"{question.Question}\n\n";

            if (question.IsTrueFalse)
            {
                result += "   Type '1' for True or '2' for False\n";
            }
            else
            {
                for (int i = 0; i < question.Options.Count; i++)
                {
                    result += $"   {i + 1}. {question.Options[i]}\n";
                }
            }
            result += "\n💡 Type your answer (1, 2, etc.) or 'quit' to stop";
            return result;
        }

        private string ProcessQuizAnswer(string input)
        {
            // Allow quitting the quiz
            if (input == "quit" || input == "exit quiz" || input == "stop")
            {
                quizActive = false;
                int answered = quiz.GetQuestionsAnswered();
                int score = quiz.GetCurrentScore();
                currentQuizQuestion = null;
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

            if (currentQuizQuestion == null)
                return HandleStartQuiz();

            if (int.TryParse(input, out int answerIndex))
            {
                answerIndex--;
                if (answerIndex >= 0 && answerIndex < currentQuizQuestion.Options.Count)
                {
                    bool isCorrect = quiz.CheckAnswer(currentQuizQuestion, answerIndex);
                    string feedback = isCorrect ? "✅ Correct!" : $"❌ Incorrect. The correct answer is: {currentQuizQuestion.Options[currentQuizQuestion.CorrectAnswerIndex]}";
                    feedback += $"\n📖 {currentQuizQuestion.Explanation}";

                    string answerType = isCorrect ? "Correct" : "Incorrect";
                    activityLog.LogAction($"Quiz answer: {answerType} - {currentQuizQuestion.Question}");

                    if (!quiz.HasNextQuestion())
                    {
                        quizActive = false;
                        currentQuizQuestion = null;
                        string finalMessage = quiz.GetFinalMessage();
                        activityLog.LogAction($"Quiz completed with {quiz.GetCurrentScore()}/{quiz.GetQuestionsAnswered()} correct");
                        return feedback + "\n\n🏁 QUIZ COMPLETE!\n" + finalMessage;
                    }
                    else
                    {
                        currentQuizQuestion = quiz.GetNextQuestion();
                        return feedback + "\n\n" + DisplayQuizQuestion(currentQuizQuestion);
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

        private string GetBasicResponse(string input)
        {
            if (input.Contains("how are you") || input.Contains("how are you doing"))
            {
                activityLog.LogAction("User asked 'how are you'");
                return "I'm functioning perfectly 😊 How can I help you with cybersecurity today?";
            }
            else if (input.Contains("purpose") || input.Contains("what are you") || input.Contains("who are you"))
            {
                return "I'm your Cybersecurity Assistant! 🔒\n" +
                       "My purpose is to teach cyber security awareness and help you manage your security tasks.\n" +
                       "I can help with:\n" +
                       "   • Password safety\n" +
                       "   • Phishing detection\n" +
                       "   • Safe browsing\n" +
                       "   • Task management\n" +
                       "   • Cybersecurity quiz\n" +
                       "   • And much more!";
            }
            else if (input.Contains("password") || input.Contains("passwords"))
            {
                return password.GetInfo();
            }
            else if (input.Contains("phishing"))
            {
                return phishing.GetInfo();
            }
            else if (input.Contains("browsing") || input.Contains("browser") || input.Contains("https"))
            {
                return safeBrowsing.GetInfo();
            }
            else if (input.Contains("topic") || input.Contains("about") || input.Contains("features"))
            {
                return "I can help you with:\n" +
                       "   🔐 Password Safety\n" +
                       "   🎣 Phishing Detection\n" +
                       "   🌐 Safe Browsing\n" +
                       "   📋 Task Management\n" +
                       "   🧠 Cybersecurity Quiz\n" +
                       "   📊 Activity Log\n" +
                       "   🧠 Memory\n\n" +
                       "Type 'help' for all commands!";
            }
            else if (input.Contains("thank") || input.Contains("thanks"))
            {
                activityLog.LogAction("User said thanks");
                return "You're welcome! 😊 Stay safe online!";
            }
            else
            {
                return "I'm not quite sure what you mean. 🤔\n" +
                       "You can ask about:\n" +
                       "   • password safety\n" +
                       "   • phishing\n" +
                       "   • safe browsing\n" +
                       "   • tasks\n" +
                       "   • quiz\n\n" +
                       "Type 'help' to see all available commands.";
            }
        }

        private string GetHelpMessage()
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
   • Ask about: password safety, phishing, safe browsing, cybersecurity tips

💡 **TIP:** Try typing naturally - I understand variations like 'add a task' or 'create new task'!";
        }

        public string GetActivityLogSummary()
        {
            return activityLog.GetLogSummary();
        }

        public void LogActivity(string action)
        {
            activityLog.LogAction(action);
        }

        public Memory GetMemory()
        {
            return memory;
        }

        // New: Export logs
        public string ExportLogs()
        {
            return activityLog.ExportLogsToFile();
        }

        // New: Clear logs
        public string ClearLogs()
        {
            activityLog.ClearLogs();
            return "🗑️ Activity log has been cleared.";
        }
    }
}