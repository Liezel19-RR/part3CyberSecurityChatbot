using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace part3.classes
{
    internal class Program
    {
    }
}

namespace cyber_security_bottttt
{
    class Program
    {
        [STAThread]
        static void Main(string[] args)
        {
            try
            {
                Application app = new Application();
                MainWindow mainWindow = new MainWindow();
                app.Run(mainWindow);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Application Error: {ex.Message}", "Error",
                              MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
