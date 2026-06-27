using CybersecurityAwarenessBot.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace part3.classes
{
    internal class TaskService
    {
    }
}


namespace CybersecurityAwarenessBot.Services
{
    /// <summary>
    /// Handles all task-related database operations (CRUD)
    /// </summary>
    public class TaskService
    {
        private readonly DatabaseService _database;

        public TaskService()
        {
            _database = new DatabaseService();
        }

        public bool AddTask(TaskItem task)
        {
            string query = @"INSERT INTO tasks (title, description, reminder_date, is_completed) 
                            VALUES (@title, @description, @reminderDate, @isCompleted)";

            var parameters = new Dictionary<string, object>
            {
                { "@title", task.Title },
                { "@description", task.Description },
                { "@reminderDate", task.ReminderDate.HasValue ? (object)task.ReminderDate.Value : DBNull.Value },
                { "@isCompleted", task.IsCompleted }
            };

            try
            {
                _database.ExecuteNonQuery(query, parameters);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public List<TaskItem> GetAllTasks()
        {
            List<TaskItem> tasks = new List<TaskItem>();
            string query = "SELECT * FROM tasks ORDER BY is_completed ASC, reminder_date ASC";

            try
            {
                DataTable dt = _database.ExecuteQuery(query);

                foreach (DataRow row in dt.Rows)
                {
                    tasks.Add(new TaskItem
                    {
                        Id = Convert.ToInt32(row["id"]),
                        Title = row["title"].ToString(),
                        Description = row["description"].ToString(),
                        ReminderDate = row["reminder_date"] != DBNull.Value ? Convert.ToDateTime(row["reminder_date"]) : (DateTime?)null,
                        IsCompleted = Convert.ToBoolean(row["is_completed"]),
                        CreatedAt = Convert.ToDateTime(row["created_at"])
                    });
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error getting tasks: {ex.Message}");
            }

            return tasks;
        }

        public bool UpdateTask(TaskItem task)
        {
            string query = @"UPDATE tasks SET 
                            title = @title, 
                            description = @description, 
                            reminder_date = @reminderDate, 
                            is_completed = @isCompleted 
                            WHERE id = @id";

            var parameters = new Dictionary<string, object>
            {
                { "@id", task.Id },
                { "@title", task.Title },
                { "@description", task.Description },
                { "@reminderDate", task.ReminderDate.HasValue ? (object)task.ReminderDate.Value : DBNull.Value },
                { "@isCompleted", task.IsCompleted }
            };

            try
            {
                _database.ExecuteNonQuery(query, parameters);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public bool DeleteTask(int id)
        {
            string query = "DELETE FROM tasks WHERE id = @id";
            var parameters = new Dictionary<string, object> { { "@id", id } };

            try
            {
                _database.ExecuteNonQuery(query, parameters);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public bool MarkComplete(int id)
        {
            string query = "UPDATE tasks SET is_completed = TRUE WHERE id = @id";
            var parameters = new Dictionary<string, object> { { "@id", id } };

            try
            {
                _database.ExecuteNonQuery(query, parameters);
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
