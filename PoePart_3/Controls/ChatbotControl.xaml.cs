using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using PoePart_3.BotLogic;

namespace PoePart_3.Controls
{
    public partial class ChatbotControl : UserControl
    {
        private readonly BotInteraction bot = new BotInteraction();
        private bool isNameEntered = false;
        private string userName = "You";

        public ChatbotControl()
        {
            InitializeComponent();
            bot.SetOutputHandler(AppendBotMessage);

            if (!isNameEntered)
            {
                AskForName();
            }
        }

        private void AskForName()
        {
            AppendBotMessage("Welcome! Please enter your name to start:");
            UserInput.Text = "";
            UserInput.Focus();
        }

        private void Send_Click(object sender, RoutedEventArgs e)
        {
            string input = UserInput.Text.Trim();
            if (string.IsNullOrEmpty(input)) return;

            AppendUserMessage(input);

            if (!isNameEntered)
            {
                userName = input;
                bot.StartConversation(userName);
                isNameEntered = true;
            }
            else
            {
                bot.ProcessUserInput(input);
            }

            UserInput.Text = "";
            UserInput.Focus();
        }

        private void UserInput_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
                Send_Click(sender, e);
        }

        private void AppendBotMessage(string message)
        {
            ChatHistory.Items.Add($"Bot: {message}");
        }

        private void AppendUserMessage(string message)
        {
            ChatHistory.Items.Add($"{userName}: {message}");
        }
    }
}
