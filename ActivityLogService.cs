using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace part3.classes
{
    internal class ActivityLogService
    {
    }
}


namespace cyber_security_bottttt
{
    /*
     * ================================================================
     * ACTIVITY LOG SERVICE
     * ================================================================
     * This service handles logging of all chatbot activities to both
     * in-memory storage and a file for persistence.
     * 
     * Features:
     * - Logs all significant actions with timestamps
     * - Stores last 100 entries in memory
     * - Saves to file for persistence
     * - Retrieves recent logs (last 10)
     * - Full log retrieval
     */
    public class ActivityLogService
    {
        private List<LogEntry> logEntries = new List<LogEntry>();
        private string logFilePath;
        private int maxMemoryEntries = 100;
        private int defaultDisplayCount = 10;

        public ActivityLogService()
        {
            // Set up log file path in Application Data folder
            string appDataFolder = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                "CybersecurityChatbot");

            if (!Directory.Exists(appDataFolder))
            {
                Directory.CreateDirectory(appDataFolder);
            }

            logFilePath = Path.Combine(appDataFolder, "activity_log.txt");

            // Load existing logs from file
            LoadLogsFromFile();

            LogAction("Activity Log Service initialized.");
        }

        // Log entry structure
        public class LogEntry
        {
            public DateTime Timestamp { get; set; }
            public string Action { get; set; }

            public override string ToString()
            {
                return $"[{Timestamp:yyyy-MM-dd HH:mm:ss}] {Action}";
            }
        }

        // Log an action
        public void LogAction(string action)
        {
            var entry = new LogEntry
            {
                Timestamp = DateTime.Now,
                Action = action
            };

            // Add to memory
            logEntries.Insert(0, entry);

            // Keep only last maxMemoryEntries
            if (logEntries.Count > maxMemoryEntries)
            {
                logEntries.RemoveRange(maxMemoryEntries, logEntries.Count - maxMemoryEntries);
            }

            // Save to file
            SaveLogToFile(entry);
        }

        // Get recent logs (default 10)
        public List<LogEntry> GetRecentLogs(int count = 10)
        {
            int take = Math.Min(count, logEntries.Count);
            return logEntries.GetRange(0, take);
        }

        // Get all logs
        public List<LogEntry> GetAllLogs()
        {
            return new List<LogEntry>(logEntries);
        }

        // Get log summary for display (formatted text)
        public string GetLogSummary()
        {
            if (logEntries.Count == 0)
                return "📭 No activity logged yet.";

            var recent = GetRecentLogs(defaultDisplayCount);
            string result = "📋 HERE'S A SUMMARY OF RECENT ACTIONS:\n";
            result += $"📅 Showing {recent.Count} of {logEntries.Count} total entries\n\n";

            for (int i = 0; i < recent.Count; i++)
            {
                result += $"   {i + 1}. {recent[i]}\n";
            }

            if (logEntries.Count > defaultDisplayCount)
            {
                result += $"\n💡 Type 'show more' or 'full log' to see all {logEntries.Count} entries.";
            }

            return result;
        }

        // Get full log summary (all entries)
        public string GetFullLogSummary()
        {
            if (logEntries.Count == 0)
                return "📭 No activity logged yet.";

            string result = "📋 COMPLETE ACTIVITY HISTORY:\n";
            result += $"📅 Total entries: {logEntries.Count}\n";
            result += $"━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━\n\n";

            for (int i = 0; i < logEntries.Count; i++)
            {
                result += $"   {i + 1}. {logEntries[i]}\n";
            }

            return result;
        }

        // Get log by category (filtered)
        public string GetLogsByCategory(string category)
        {
            category = category.ToLower();
            var filtered = logEntries.FindAll(e =>
                e.Action.ToLower().Contains(category) ||
                e.Action.ToLower().Contains(category.ToLower()));

            if (filtered.Count == 0)
                return $"📭 No logs found for category: '{category}'";

            string result = $"📋 LOGS FOR CATEGORY: '{category}'\n";
            result += $"━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━\n\n";

            for (int i = 0; i < Math.Min(filtered.Count, 20); i++)
            {
                result += $"   {i + 1}. {filtered[i]}\n";
            }

            if (filtered.Count > 20)
            {
                result += $"\n💡 Showing 20 of {filtered.Count} entries.";
            }

            return result;
        }

        // Clear all logs
        public void ClearLogs()
        {
            logEntries.Clear();

            // Clear the file
            if (File.Exists(logFilePath))
            {
                File.WriteAllText(logFilePath, "");
            }

            LogAction("Activity log cleared.");
        }

        // Get total log count
        public int GetLogCount()
        {
            return logEntries.Count;
        }

        // Export logs to file
        public string ExportLogsToFile()
        {
            try
            {
                string exportPath = Path.Combine(
                    Environment.GetFolderPath(Environment.SpecialFolder.Desktop),
                    $"ActivityLog_{DateTime.Now:yyyyMMdd_HHmmss}.txt");

                using (StreamWriter writer = new StreamWriter(exportPath))
                {
                    writer.WriteLine("CYBERSECURITY CHATBOT - ACTIVITY LOG EXPORT");
                    writer.WriteLine($"Exported: {DateTime.Now:yyyy-MM-dd HH:mm:ss}");
                    writer.WriteLine($"Total Entries: {logEntries.Count}");
                    writer.WriteLine(new string('=', 50));
                    writer.WriteLine();

                    foreach (var entry in logEntries)
                    {
                        writer.WriteLine($"[{entry.Timestamp:yyyy-MM-dd HH:mm:ss}] {entry.Action}");
                    }
                }

                return $"✅ Logs exported successfully to:\n{exportPath}";
            }
            catch (Exception ex)
            {
                return $"❌ Error exporting logs: {ex.Message}";
            }
        }

        // Private: Save log to file
        private void SaveLogToFile(LogEntry entry)
        {
            try
            {
                using (StreamWriter writer = new StreamWriter(logFilePath, true))
                {
                    writer.WriteLine(entry.ToString());
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error saving log to file: {ex.Message}");
            }
        }

        // Private: Load logs from file
        private void LoadLogsFromFile()
        {
            if (!File.Exists(logFilePath))
                return;

            try
            {
                string[] lines = File.ReadAllLines(logFilePath);

                // Load only the last maxMemoryEntries entries
                int start = Math.Max(0, lines.Length - maxMemoryEntries);

                for (int i = lines.Length - 1; i >= start; i--)
                {
                    string line = lines[i].Trim();
                    if (string.IsNullOrEmpty(line))
                        continue;

                    // Parse the line format: "[timestamp] action"
                    if (line.StartsWith("[") && line.Contains("] "))
                    {
                        int endBracket = line.IndexOf(']');
                        string timestampStr = line.Substring(1, endBracket - 1);
                        string action = line.Substring(endBracket + 2);

                        if (DateTime.TryParse(timestampStr, out DateTime timestamp))
                        {
                            logEntries.Add(new LogEntry
                            {
                                Timestamp = timestamp,
                                Action = action
                            });
                        }
                    }
                }

                // Sort by timestamp (newest first)
                logEntries.Sort((a, b) => b.Timestamp.CompareTo(a.Timestamp));
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error loading logs from file: {ex.Message}");
            }
        }
    }
}