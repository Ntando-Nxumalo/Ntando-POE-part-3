using System;
using System.IO;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Controls;
using PoePart_3.Controls;

namespace PoePart_3
{
    public partial class MainWindow : Window
    {
        private TextBox UserInput;
        private ListBox ChatHistory;

        public MainWindow()
        {
            InitializeComponent();
            UserInput = (TextBox)FindName("UserInput");
            ChatHistory = (ListBox)FindName("ChatHistory");
        }

        private void Chats_Click(object sender, RoutedEventArgs e)
        {
            MainContentArea.Children.Clear();
            MainContentArea.Children.Add(new ChatbotControl());
        }

        private void Reminder_Click(object sender, RoutedEventArgs e)
        {
            MainContentArea.Children.Clear();
            MainContentArea.Children.Add(new ReminderControl());
        }

        private void Quiz_Click(object sender, RoutedEventArgs e)
        {
            MainContentArea.Children.Clear();
            MainContentArea.Children.Add(new PoePart_3.Controls.QuizzQuestionControl());

        }

        private void ActivityLog_Click(object sender, RoutedEventArgs e)
        {
            var recent = PoePart_3.ActivityLogService.GetRecent(10);
            string message = "Here's a summary of recent actions:\n\n";
            int i = 1;
            foreach (var entry in recent)
                message += $"{i++}. {entry}\n";
            MessageBox.Show(message, "Activity Log");
        }

        private void Exit_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }
    }
}
