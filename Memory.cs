using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace part3
{
    internal class Memory
    {
    }
}

namespace cyber_security_bottttt
{
    /*
     * ================================================================
     * MEMORY CLASS
     * ================================================================
     * This class simulates short-term memory for the chatbot.
     * It stores user inputs and can recall them when prompted.
     * This adds a more human-like interaction experience.
     */
    public class Memory
    {
        private List<string> memories = new List<string>();
        private int maxMemories = 20;

        public Memory()
        {
            // Initialize with some default cybersecurity tips
            memories.Add("Welcome to Cybersecurity Chatbot!");
        }

        public void StoreMemory(string input)
        {
            if (string.IsNullOrEmpty(input))
                return;

            // Don't store commands or common phrases
            string[] excludeTerms = { "help", "exit", "quit", "hello", "hi", "hey" };
            foreach (string term in excludeTerms)
            {
                if (input.ToLower().Trim() == term)
                    return;
            }

            // Add to memories
            memories.Add(input);

            // Keep only last maxMemories entries
            if (memories.Count > maxMemories)
            {
                memories.RemoveAt(0);
            }
        }

        public string RecallMemory(string input)
        {
            // Check if user is asking for memory recall
            if (input.Contains("what did i say") ||
                input.Contains("remember when") ||
                input.Contains("recall"))
            {
                return ShowMemory();
            }

            // Check if user wants to remember something
            if (input.StartsWith("remember ") || input.Contains(" i want you to remember "))
            {
                string text = input.Replace("remember", "").Trim();
                text = text.Replace("i want you to", "").Trim();
                if (!string.IsNullOrEmpty(text))
                {
                    StoreMemory(text);
                    return $"🧠 I'll remember that: '{text}'";
                }
            }

            return "";
        }

        public string ShowMemory()
        {
            if (memories.Count == 0 || (memories.Count == 1 && memories[0] == "Welcome to Cybersecurity Chatbot!"))
                return "🧠 I don't have any memories stored yet. Tell me something to remember!";

            StringBuilder sb = new StringBuilder();
            sb.AppendLine("🧠 WHAT I REMEMBER:");
            sb.AppendLine($"📊 {memories.Count} memories stored\n");

            for (int i = 0; i < memories.Count; i++)
            {
                sb.AppendLine($"   {i + 1}. {memories[i]}");
            }

            sb.AppendLine("\n💡 To store a memory, say 'remember [something]'");
            return sb.ToString();
        }

        public void ClearMemory()
        {
            memories.Clear();
            memories.Add("Memory cleared.");
        }

        public int GetMemoryCount()
        {
            return memories.Count;
        }

        public List<string> GetAllMemories()
        {
            return new List<string>(memories);
        }
    }
}