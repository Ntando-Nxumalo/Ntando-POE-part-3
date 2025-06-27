using System.Collections.Generic;
using System.Windows;
using PoePart_3.ActivityLog;

namespace PoePart_3.Controls
{
    public partial class ActivityLogWindow : Window
    {
        private readonly ActivityLogger logger;

        public ActivityLogWindow(ActivityLogger loggerInstance)
        {
            InitializeComponent();
            logger = loggerInstance;

            LoadLogs();
        }

        private void LoadLogs()
        {
            logListBox.Items.Clear();
            var recentLogs = logger.GetRecentLogs();
            foreach (var log in recentLogs)
            {
                logListBox.Items.Add("• " + log);
            }

            if (recentLogs.Count == 0)
            {
                logListBox.Items.Add("No activity recorded yet.");
            }
        }

        private void ClearLog_Click(object sender, RoutedEventArgs e)
        {
            logger.ClearLog();
            LoadLogs(); // Refresh list
            MessageBox.Show("Activity log cleared.", "Log Cleared", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void Close_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
