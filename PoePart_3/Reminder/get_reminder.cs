using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Reminder
{
    public class get_reminder
    {
        private List<string> descriptions = new List<string>();
        private List<string> dates = new List<string>();

        /// <summary>
        /// Validates if the input contains "remind me in" or "add task".
        /// </summary>
        public string validate_input(string input)
        {
            if (!string.IsNullOrWhiteSpace(input) &&
                (input.ToLower().Contains("remind me in") || input.ToLower().Contains("add task")))
            {
                return "found";
            }
            return "Invalid command. Try saying 'remind me in 3 days'.";
        }

        /// <summary>
        /// Extracts number of days from the user input.
        /// </summary>
        public string get_days(string input)
        {
            Match match = Regex.Match(input, @"\b(\d+)\s*days?\b");
            if (match.Success)
            {
                return match.Groups[1].Value;
            }
            return "Invalid input";
        }

        /// <summary>
        /// Calculates reminder date N days from now.
        /// </summary>
        public string today_date(string description, string days)
        {
            if (int.TryParse(days, out int numDays))
            {
                DateTime future = DateTime.Now.AddDays(numDays);
                string formatted = future.ToString("yyyy-MM-dd") + " at " + future.ToString("hh:mm tt");
                descriptions.Add(description);
                dates.Add(formatted);
                return $"I'll remind you on {formatted}";
            }
            return "Error setting reminder.";
        }

        /// <summary>
        /// Gets formatted list of reminders.
        /// </summary>
        public List<string> get_reminders()
        {
            var reminders = new List<string>();
            for (int i = 0; i < descriptions.Count; i++)
            {
                reminders.Add($"{descriptions[i]} - {dates[i]}");
            }
            return reminders;
        }
    }
}
