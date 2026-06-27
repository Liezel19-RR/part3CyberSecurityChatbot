using System;
using System.Collections.Generic;
using MySql.Data.MySqlClient;

namespace cyber_security_bottttt
{
    public class DatabaseHelper
    {
        private string connectionString = "Server=localhost;Database=cybersecurity_tasks;Uid=root;Pwd=;";

        public DatabaseHelper()
        {
            try
            {
                CreateDatabaseIfNotExists();
                CreateTasksTableIfNotExists();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Database initialization error: " + ex.Message);
            }
        }

        // ============================================================
        // TEST CONNECTION
        // ============================================================
        public bool TestConnection()
        {
            try
            {
                using (MySqlConnection conn = new MySqlConnection(connectionString))
                {
                    conn.Open();
                    return true;
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Connection test failed: " + ex.Message);
                return false;
            }
        }

        // ============================================================
        // CREATE DATABASE
        // ============================================================
        private void CreateDatabaseIfNotExists()
        {
            try
            {
                string createDbQuery = "CREATE DATABASE IF NOT EXISTS cybersecurity_tasks;";
                using (MySqlConnection conn = new MySqlConnection("Server=localhost;Uid=root;Pwd=;"))
                {
                    conn.Open();
                    MySqlCommand cmd = new MySqlCommand(createDbQuery, conn);
                    cmd.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Database creation error: " + ex.Message);
            }
        }

        // ============================================================
        // CREATE TABLE
        // ============================================================
        private void CreateTasksTableIfNotExists()
        {
            try
            {
                string createTableQuery = @"
                    CREATE TABLE IF NOT EXISTS tasks (
                        id INT AUTO_INCREMENT PRIMARY KEY,
                        title VARCHAR(255) NOT NULL,
                        description TEXT,
                        reminder_date DATETIME,
                        is_completed BOOLEAN DEFAULT FALSE,
                        created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP
                    );";

                using (MySqlConnection conn = new MySqlConnection(connectionString))
                {
                    conn.Open();
                    MySqlCommand cmd = new MySqlCommand(createTableQuery, conn);
                    cmd.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Table creation error: " + ex.Message);
            }
        }

        // ============================================================
        // ADD TASK
        // ============================================================
        public bool AddTask(string title, string description, DateTime? reminderDate = null)
        {
            try
            {
                string query = "INSERT INTO tasks (title, description, reminder_date) VALUES (@title, @desc, @reminder);";
                using (MySqlConnection conn = new MySqlConnection(connectionString))
                {
                    conn.Open();
                    MySqlCommand cmd = new MySqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@title", title);
                    cmd.Parameters.AddWithValue("@desc", description ?? "");

                    if (reminderDate.HasValue)
                    {
                        cmd.Parameters.AddWithValue("@reminder", reminderDate.Value);
                    }
                    else
                    {
                        cmd.Parameters.AddWithValue("@reminder", DBNull.Value);
                    }

                    int rowsAffected = cmd.ExecuteNonQuery();
                    return rowsAffected > 0;
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Add task error: " + ex.Message);
                return false;
            }
        }

        // ============================================================
        // GET ALL TASKS
        // ============================================================
        public List<TaskItem> GetAllTasks()
        {
            List<TaskItem> tasks = new List<TaskItem>();
            try
            {
                string query = "SELECT * FROM tasks ORDER BY created_at DESC;";
                using (MySqlConnection conn = new MySqlConnection(connectionString))
                {
                    conn.Open();
                    MySqlCommand cmd = new MySqlCommand(query, conn);
                    using (MySqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            TaskItem task = new TaskItem();
                            task.Id = reader.GetInt32("id");
                            task.Title = reader.GetString("title");

                            // Handle potential null values
                            if (!reader.IsDBNull(reader.GetOrdinal("description")))
                                task.Description = reader.GetString("description");
                            else
                                task.Description = "";

                            if (!reader.IsDBNull(reader.GetOrdinal("reminder_date")))
                                task.ReminderDate = reader.GetDateTime("reminder_date");
                            else
                                task.ReminderDate = null;

                            task.IsCompleted = reader.GetBoolean("is_completed");
                            task.CreatedAt = reader.GetDateTime("created_at");

                            tasks.Add(task);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Get tasks error: " + ex.Message);
            }
            return tasks;
        }

        // ============================================================
        // DELETE TASK
        // ============================================================
        public bool DeleteTask(int id)
        {
            try
            {
                string query = "DELETE FROM tasks WHERE id = @id;";
                using (MySqlConnection conn = new MySqlConnection(connectionString))
                {
                    conn.Open();
                    MySqlCommand cmd = new MySqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@id", id);
                    int rowsAffected = cmd.ExecuteNonQuery();
                    return rowsAffected > 0;
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Delete task error: " + ex.Message);
                return false;
            }
        }

        // ============================================================
        // MARK TASK COMPLETED
        // ============================================================
        public bool MarkTaskCompleted(int id)
        {
            try
            {
                string query = "UPDATE tasks SET is_completed = TRUE WHERE id = @id;";
                using (MySqlConnection conn = new MySqlConnection(connectionString))
                {
                    conn.Open();
                    MySqlCommand cmd = new MySqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@id", id);
                    int rowsAffected = cmd.ExecuteNonQuery();
                    return rowsAffected > 0;
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Complete task error: " + ex.Message);
                return false;
            }
        }

        // ============================================================
        // UPDATE TASK REMINDER
        // ============================================================
        public bool UpdateTaskReminder(int id, DateTime reminderDate)
        {
            try
            {
                string query = "UPDATE tasks SET reminder_date = @reminder WHERE id = @id;";
                using (MySqlConnection conn = new MySqlConnection(connectionString))
                {
                    conn.Open();
                    MySqlCommand cmd = new MySqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@reminder", reminderDate);
                    cmd.Parameters.AddWithValue("@id", id);
                    int rowsAffected = cmd.ExecuteNonQuery();
                    return rowsAffected > 0;
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Update reminder error: " + ex.Message);
                return false;
            }
        }

        // ============================================================
        // UPDATE TASK DETAILS
        // ============================================================
        public bool UpdateTask(int id, string title, string description)
        {
            try
            {
                string query = "UPDATE tasks SET title = @title, description = @desc WHERE id = @id;";
                using (MySqlConnection conn = new MySqlConnection(connectionString))
                {
                    conn.Open();
                    MySqlCommand cmd = new MySqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@title", title);
                    cmd.Parameters.AddWithValue("@desc", description);
                    cmd.Parameters.AddWithValue("@id", id);
                    int rowsAffected = cmd.ExecuteNonQuery();
                    return rowsAffected > 0;
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Update task error: " + ex.Message);
                return false;
            }
        }

        // ============================================================
        // GET TASK BY ID
        // ============================================================
        public TaskItem GetTaskById(int id)
        {
            try
            {
                string query = "SELECT * FROM tasks WHERE id = @id;";
                using (MySqlConnection conn = new MySqlConnection(connectionString))
                {
                    conn.Open();
                    MySqlCommand cmd = new MySqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@id", id);
                    using (MySqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            TaskItem task = new TaskItem();
                            task.Id = reader.GetInt32("id");
                            task.Title = reader.GetString("title");

                            if (!reader.IsDBNull(reader.GetOrdinal("description")))
                                task.Description = reader.GetString("description");
                            else
                                task.Description = "";

                            if (!reader.IsDBNull(reader.GetOrdinal("reminder_date")))
                                task.ReminderDate = reader.GetDateTime("reminder_date");
                            else
                                task.ReminderDate = null;

                            task.IsCompleted = reader.GetBoolean("is_completed");
                            task.CreatedAt = reader.GetDateTime("created_at");

                            return task;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Get task by id error: " + ex.Message);
            }
            return null;
        }

        // ============================================================
        // GET TASKS BY STATUS
        // ============================================================
        public List<TaskItem> GetTasksByStatus(bool isCompleted)
        {
            List<TaskItem> tasks = new List<TaskItem>();
            try
            {
                string query = "SELECT * FROM tasks WHERE is_completed = @completed ORDER BY created_at DESC;";
                using (MySqlConnection conn = new MySqlConnection(connectionString))
                {
                    conn.Open();
                    MySqlCommand cmd = new MySqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@completed", isCompleted);
                    using (MySqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            TaskItem task = new TaskItem();
                            task.Id = reader.GetInt32("id");
                            task.Title = reader.GetString("title");

                            if (!reader.IsDBNull(reader.GetOrdinal("description")))
                                task.Description = reader.GetString("description");
                            else
                                task.Description = "";

                            if (!reader.IsDBNull(reader.GetOrdinal("reminder_date")))
                                task.ReminderDate = reader.GetDateTime("reminder_date");
                            else
                                task.ReminderDate = null;

                            task.IsCompleted = reader.GetBoolean("is_completed");
                            task.CreatedAt = reader.GetDateTime("created_at");

                            tasks.Add(task);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Get tasks by status error: " + ex.Message);
            }
            return tasks;
        }

        // ============================================================
        // DELETE ALL COMPLETED TASKS
        // ============================================================
        public bool DeleteCompletedTasks()
        {
            try
            {
                string query = "DELETE FROM tasks WHERE is_completed = TRUE;";
                using (MySqlConnection conn = new MySqlConnection(connectionString))
                {
                    conn.Open();
                    MySqlCommand cmd = new MySqlCommand(query, conn);
                    int rowsAffected = cmd.ExecuteNonQuery();
                    return rowsAffected > 0;
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Delete completed tasks error: " + ex.Message);
                return false;
            }
        }

        // ============================================================
        // GET TASK COUNT
        // ============================================================
        public int GetTaskCount()
        {
            try
            {
                string query = "SELECT COUNT(*) FROM tasks;";
                using (MySqlConnection conn = new MySqlConnection(connectionString))
                {
                    conn.Open();
                    MySqlCommand cmd = new MySqlCommand(query, conn);
                    return Convert.ToInt32(cmd.ExecuteScalar());
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Get task count error: " + ex.Message);
                return 0;
            }
        }

        // ============================================================
        // GET COMPLETED TASK COUNT
        // ============================================================
        public int GetCompletedTaskCount()
        {
            try
            {
                string query = "SELECT COUNT(*) FROM tasks WHERE is_completed = TRUE;";
                using (MySqlConnection conn = new MySqlConnection(connectionString))
                {
                    conn.Open();
                    MySqlCommand cmd = new MySqlCommand(query, conn);
                    return Convert.ToInt32(cmd.ExecuteScalar());
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Get completed task count error: " + ex.Message);
                return 0;
            }
        }

        // ============================================================
        // GET PENDING TASK COUNT
        // ============================================================
        public int GetPendingTaskCount()
        {
            try
            {
                string query = "SELECT COUNT(*) FROM tasks WHERE is_completed = FALSE;";
                using (MySqlConnection conn = new MySqlConnection(connectionString))
                {
                    conn.Open();
                    MySqlCommand cmd = new MySqlCommand(query, conn);
                    return Convert.ToInt32(cmd.ExecuteScalar());
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Get pending task count error: " + ex.Message);
                return 0;
            }
        }
    }

    // ================================================================
    // TASK ITEM CLASS
    // ================================================================
    public class TaskItem
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public DateTime? ReminderDate { get; set; }
        public bool IsCompleted { get; set; }
        public DateTime CreatedAt { get; set; }

        public override string ToString()
        {
            string status = IsCompleted ? "✅" : "⏳";
            string reminder = ReminderDate.HasValue ? $" (Reminder: {ReminderDate.Value:yyyy-MM-dd HH:mm})" : "";
            return $"{status} #{Id}: {Title} - {Description}{reminder}";
        }

        public string ToShortString()
        {
            string status = IsCompleted ? "✅" : "⏳";
            return $"{status} #{Id}: {Title}";
        }
    }
}