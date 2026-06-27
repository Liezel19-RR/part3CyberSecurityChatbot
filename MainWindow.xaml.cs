using part3;
using System;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Xml.Linq;

namespace cyber_security_bottttt
{
    public partial class MainWindow : Window
    {
        private Chatbot bot = new ChatBot();
        private ResponseHandler responseHandler = new ResponseHandler();
        private TypingEffect typer = new TypingEffect();
        private VoicePlayer voice = new VoicePlayer();
        private DatabaseHelper db = new DatabaseHelper();
        private bool isChatStarted = false;
        private string userName = "";

        public MainWindow()
        {
            InitializeComponent();

            // Play welcome voice
            try
            {
                voice.PlayVoice();
            }
            catch { }

            // Initial welcome message
            ChatBox.AppendText("🛡️ Welcome to Cybersecurity Chatbot! (Part 3)\n");
            ChatBox.AppendText("━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━\n\n");
            ChatBox.AppendText("🔒 This chatbot helps you with:\n");
            ChatBox.AppendText("   • Task Management with MySQL database\n");
            ChatBox.AppendText("   • Cybersecurity Quiz with 15+ questions\n");
            ChatBox.AppendText("   • Natural Language Processing simulation\n");
            ChatBox.AppendText("   • Activity Logging with history\n");
            ChatBox.AppendText("   • Memory for personalized interactions\n\n");
            ChatBox.AppendText("💡 Enter your name and click 'Start Chat' to begin!\n");
            ChatBox.ScrollToEnd();

            // Update stats
            UpdateStats();
        }

        private async void StartChat_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtName.Text))
            {
                MessageBox.Show("Please enter your name to start.", "Name Required",
                              MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            userName = txtName.Text.Trim();
            isChatStarted = true;
            ChatBox.Clear();

            // Update user label
            lblUser.Text = $"👤 User: {userName}";

            await typer.Type(
                ChatBox,
                $"👋 Hello {userName}! Welcome to your Cybersecurity Assistant.\n" +
                $"━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━\n\n" +
                $"🔐 I'm here to help you stay safe online.\n\n" +
                $"📋 You can:\n" +
                $"   • Add and manage cybersecurity tasks\n" +
                $"   • Take a cybersecurity quiz\n" +
                $"   • View your activity log\n" +
                $"   • Store and recall memories\n\n" +
                $"💡 Type 'help' to see all available commands!"
            );

            responseHandler.LogActivity($"User {userName} started chat");
            UpdateStats();
        }

        private async void TestDB_Click(object sender, RoutedEventArgs e)
        {
            if (db.TestConnection())
            {
                MessageBox.Show("✅ Database connection successful!\n\nMySQL is running and connected.",
                              "Database Status", MessageBoxButton.OK, MessageBoxImage.Information);

                // Show task count
                var tasks = db.GetAllTasks();
                lblTasks.Text = $"📋 Tasks: {tasks.Count}";
            }
            else
            {
                MessageBox.Show("❌ Database connection failed!\n\n" +
                               "Please check:\n" +
                               "   • MySQL is installed and running\n" +
                               "   • The service is started\n" +
                               "   • Your connection string is correct",
                               "Database Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private async void btnSend_Click(object sender, RoutedEventArgs e)
        {
            await ProcessUserInput();
        }

        private async void txtInput_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                e.Handled = true;
                await ProcessUserInput();
            }
        }

        private async Task ProcessUserInput()
        {
            if (!isChatStarted)
            {
                MessageBox.Show("Please click 'Start Chat' first.", "Not Started",
                              MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            string input = txtInput.Text.Trim();
            if (string.IsNullOrEmpty(input))
                return;

            txtInput.Clear();

            // Display user input
            ChatBox.AppendText($"🧑 **{userName}:** {input}\n");
            ChatBox.ScrollToEnd();

            // Process with ResponseHandler
            string response = responseHandler.ProcessInput(input, userName);

            // Display bot response with typing effect
            ChatBox.AppendText($"🤖 **Bot:** ");
            await typer.Type(ChatBox, response);
            ChatBox.AppendText("\n");
            ChatBox.ScrollToEnd();

            // Update stats after each interaction
            UpdateStats();
        }

        private async void QuickAction_Click(object sender, RoutedEventArgs e)
        {
            if (!isChatStarted)
            {
                MessageBox.Show("Please click 'Start Chat' first.", "Not Started",
                              MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            Button btn = sender as Button;
            string action = btn.Content.ToString();

            string command = action switch
            {
                "📋 Tasks" => "show tasks",
                "🧠 Quiz" => "quiz",
                "📊 Activity" => "activity log",
                "🧠 Memory" => "what do you remember",
                "❓ Help" => "help",
                _ => "help"
            };

            txtInput.Text = command;
            await ProcessUserInput();
        }

        private void ClearChat_Click(object sender, RoutedEventArgs e)
        {
            ChatBox.Clear();
            ChatBox.AppendText("💬 Chat cleared.\n");
            ChatBox.AppendText("━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━\n\n");
            ChatBox.AppendText($"👋 Welcome back, {userName}!\n");
            ChatBox.AppendText("💡 Type 'help' for commands or continue your conversation.\n\n");
            responseHandler.LogActivity("Chat cleared by user");
            ChatBox.ScrollToEnd();
        }

        private void Exit_Click(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show("Are you sure you want to exit?", "Confirm Exit",
                              MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                responseHandler.LogActivity($"User {userName} exited the application");
                Application.Current.Shutdown();
            }
        }

        private void UpdateStats()
        {
            try
            {
                // Update task count
                var tasks = db.GetAllTasks();
                lblTasks.Text = $"📋 Tasks: {tasks.Count}";

                // Update memory count
                var memory = responseHandler.GetMemory();
                lblMemory.Text = $"🧠 Memory: {memory.GetMemoryCount()}";

                // Update status
                lblStatus.Text = "💡 Status: Connected";

                // Update user name if set
                if (!string.IsNullOrEmpty(userName))
                {
                    lblUser.Text = $"👤 User: {userName}";
                }
            }
            catch
            {
                lblStatus.Text = "💡 Status: Error";
            }
        }

        protected override void OnClosing(System.ComponentModel.CancelEventArgs e)
        {
            responseHandler.LogActivity($"User {userName} closed the application");
            base.OnClosing(e);
        }
    }
}