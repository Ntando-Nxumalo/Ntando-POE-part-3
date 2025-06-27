using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Reminder;

namespace PoePart_3.Controls
{
    /// <summary>
    /// ReminderControl: Handles NLP-driven task creation, reminder setup, and user interaction.
    /// </summary>
    public partial class ReminderControl : UserControl
    {
        private readonly get_reminder reminderLogic = new get_reminder();

        private bool awaitingReminder = false;
        private string lastTask = "";

        public ReminderControl()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Detects and handles user input using NLP simulation.
        /// </summary>
        private void Ask_Question(object sender, RoutedEventArgs e)
        {
            string input = user_task.Text.Trim();

            if (string.IsNullOrWhiteSpace(input))
            {
                MessageBox.Show("Please enter a task or response.");
                return;
            }

            chat_append.Items.Add("User: " + input);
            chat_append.ScrollIntoView(chat_append.Items[chat_append.Items.Count - 1]);

            if (awaitingReminder)
            {
                string validation = reminderLogic.validate_input(input);
                if (validation != "found")
                {
                    chat_append.Items.Add("Bot: " + validation);
                }
                else
                {
                    string day = reminderLogic.get_days(input);
                    if (day == "Invalid input")
                    {
                        chat_append.Items.Add("Bot: Please specify the number of days (e.g., 'in 3 days').");
                    }
                    else
                    {
                        string result = reminderLogic.today_date(lastTask, day);
                        chat_append.Items.Add("Bot: Got it! " + result);
                        chat_append.Items.Add($"[Task] {lastTask} | Reminder: {result}");
                        awaitingReminder = false;
                        lastTask = "";
                    }
                }

                user_task.Clear();
                return;
            }

            string intent = DetectIntent(input);

            switch (intent)
            {
                case "add_task":
                    lastTask = ExtractTaskDescription(input);
                    chat_append.Items.Add($"Bot: Task added: \"{lastTask}\". Would you like to set a reminder?");
                    awaitingReminder = true;
                    break;

                case "reminder":
                    string valid = reminderLogic.validate_input(input);
                    if (valid == "found")
                    {
                        string days = reminderLogic.get_days(input);
                        if (days != "Invalid input")
                        {
                            string output = reminderLogic.today_date(input, days);
                            chat_append.Items.Add("Bot: " + output);
                        }
                        else
                        {
                            chat_append.Items.Add("Bot: Please include a number of days (e.g., 'in 3 days').");
                        }
                    }
                    else
                    {
                        chat_append.Items.Add("Bot: " + valid);
                    }
                    break;

                case "cyber_task":
                    chat_append.Items.Add($"Bot: Cybersecurity task logged: \"{input}\".");
                    break;

                case "summary":
                    chat_append.Items.Add("Bot: Here's a summary of recent actions (coming soon).");
                    break;

                default:
                    chat_append.Items.Add("Bot: I'm still learning. Try saying 'add a task', 'remind me', or ask about cybersecurity.");
                    break;
            }

            user_task.Clear();
        }

        /// <summary>
        /// Determines user intent using basic NLP simulation (keywords).
        /// </summary>
        private string DetectIntent(string input)
        {
            input = input.ToLower();

            if (input.Contains("add task") || input.Contains("create a task") || input.Contains("set a task") ||
                input.Contains("log a task") || input.Contains("note down") || input.Contains("add to do"))
                return "add_task";

            if (input.Contains("remind") || input.Contains("set reminder") || input.Contains("reminder"))
                return "reminder";

            if (input.Contains("password") || input.Contains("phishing") || input.Contains("2fa") ||
                input.Contains("authentication") || input.Contains("privacy") || input.Contains("security"))
                return "cyber_task";

            if (input.Contains("what have you done") || input.Contains("summary"))
                return "summary";

            return "unknown";
        }

        /// <summary>
        /// Extracts a user task from a freeform sentence.
        /// </summary>
        private string ExtractTaskDescription(string input)
        {
            input = input.ToLower();
            int start = input.IndexOf("task");
            if (start >= 0 && start + 4 < input.Length)
            {
                return input.Substring(start + 4).Trim(new char[] { ' ', '-', ':' });
            }
            return input;
        }

        /// <summary>
        /// Manually sets a reminder via button.
        /// </summary>
        private void Set_Reminder(object sender, RoutedEventArgs e)
        {
            string input = user_task.Text.Trim();
            string validation = reminderLogic.validate_input(input);

            if (validation != "found")
            {
                MessageBox.Show(validation);
                return;
            }

            string day = reminderLogic.get_days(input);
            if (day == "Invalid input")
            {
                MessageBox.Show("Please include a valid number of days.");
                return;
            }

            string result = reminderLogic.today_date(input, day);
            chat_append.Items.Add("Bot: " + result);
            user_task.Clear();
        }

        /// <summary>
        /// Marks a task as done or deletes it on double-click.
        /// </summary>
        private void chat_append_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (chat_append.SelectedItem is string selected_task)
            {
                if (!selected_task.Contains("[status done]"))
                {
                    int index = chat_append.Items.IndexOf(selected_task);
                    chat_append.Items[index] = selected_task + " [status done]";
                }
                else
                {
                    chat_append.Items.Remove(selected_task);
                }
            }
        }
    }
}
