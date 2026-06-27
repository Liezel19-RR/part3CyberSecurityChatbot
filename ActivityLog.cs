using part3.classes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace part3
{
    internal class ActivityLog
    {
    }
}


namespace cyber_security_bottttt
{
    /*
     * ================================================================
     * ACTIVITY LOG (Legacy Compatibility)
     * ================================================================
     * This class now uses ActivityLogService for all functionality.
     * It remains for backward compatibility with existing code.
     */
    public class ActivityLog
    {
        private ActivityLogService logService = new ActivityLogService();

        public void LogAction(string action)
        {
            logService.LogAction(action);
        }

        public List<string> GetRecentLogs(int count = 10)
        {
            var entries = logService.GetRecentLogs(count);
            List<string> strings = new List<string>();
            foreach (var entry in entries)
            {
                strings.Add(entry.ToString());
            }
            return strings;
        }

        public string GetLogSummary()
        {
            return logService.GetLogSummary();
        }

        public string GetFullLogSummary()
        {
            return logService.GetFullLogSummary();
        }

        public void ClearLog()
        {
            logService.ClearLogs();
            LogAction("Activity log cleared.");
        }

        public int GetLogCount()
        {
            return logService.GetLogCount();
        }

        public string ExportLogsToFile()
        {
            return logService.ExportLogsToFile();
        }
    }
}