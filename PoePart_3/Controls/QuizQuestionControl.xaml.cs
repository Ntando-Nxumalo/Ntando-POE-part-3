using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace PoePart_3.Controls
{
    public partial class QuizzQuestionControl : UserControl
    {
        private List<QuizQuestion> quizData = new List<QuizQuestion>();
        private int questionIndex = 0;
        private int currentScore = 0;
        private Button? selectedButton = null;
        private List<Button> optionButtons = new List<Button>();

        public QuizzQuestionControl()
        {
            InitializeComponent();
            optionButtons = new List<Button> { OptionA, OptionB, OptionC, OptionD };
            LoadQuizData();
            ShowQuiz();
        }

        private void LoadQuizData()
        {
            quizData = new List<QuizQuestion> {
                new QuizQuestion
                {
                    Question = "What is a strong password characteristic?",
                    CorrectChoice = "At least 14 characters long",
                    Choices = new List<string> { "Only lowercase letters", "Your pet's name", "12345678" }
                },
                new QuizQuestion
                {
                    Question = "What is phishing?",
                    CorrectChoice = "Tricking users into revealing sensitive information",
                    Choices = new List<string> { "A type of malware", "A password manager", "A secure website" }
                },
                new QuizQuestion
                {
                    Question = "Which is a sign of a phishing email?",
                    CorrectChoice = "Urgent language and unfamiliar sender",
                    Choices = new List<string> { "Personalized greeting", "No links", "Sent from your own address" }
                },
                new QuizQuestion
                {
                    Question = "What should you do before clicking a link in an email?",
                    CorrectChoice = "Hover over the link to check the URL",
                    Choices = new List<string> { "Click immediately", "Reply to the sender", "Forward to a friend" }
                },
                new QuizQuestion
                {
                    Question = "What is a good way to manage multiple passwords?",
                    CorrectChoice = "Use a password manager",
                    Choices = new List<string> { "Write them on paper", "Use the same password everywhere", "Share with friends" }
                },
                new QuizQuestion
                {
                    Question = "What is multi-factor authentication (MFA)?",
                    CorrectChoice = "Using two or more ways to verify your identity",
                    Choices = new List<string> { "A type of virus", "A password reset", "A browser extension" }
                },
                new QuizQuestion
                {
                    Question = "What should you avoid in your passwords?",
                    CorrectChoice = "Personal details like your birthday",
                    Choices = new List<string> { "Special characters", "Uppercase letters", "Numbers" }
                },
                new QuizQuestion
                {
                    Question = "What is a common scam tactic?",
                    CorrectChoice = "Creating a sense of urgency",
                    Choices = new List<string> { "Offering free antivirus", "Sending newsletters", "Providing tech support" }
                },
                new QuizQuestion
                {
                    Question = "How can you protect your privacy online?",
                    CorrectChoice = "Review app permissions regularly",
                    Choices = new List<string> { "Share your location always", "Post personal info on social media", "Use public Wi-Fi for banking" }
                },
                new QuizQuestion
                {
                    Question = "What is the safest way to use public Wi-Fi?",
                    CorrectChoice = "Use a VPN",
                    Choices = new List<string> { "Disable antivirus", "Turn off your firewall", "Share files with strangers" }
                }
            };
        }

        private void ShowQuiz()
        {
            if (questionIndex >= quizData.Count)
            {
                MessageBox.Show($"Quiz complete! Your score: {currentScore}/{quizData.Count}");
                questionIndex = 0;
                currentScore = 0;
                ShowQuiz();
                return;
            }

            var currentQuiz = quizData[questionIndex];
            QuestionText.Text = currentQuiz.Question;

            // Shuffle choices
            var allChoices = new List<string>(currentQuiz.Choices) { currentQuiz.CorrectChoice };
            allChoices = allChoices.OrderBy(_ => System.Guid.NewGuid()).ToList();

            OptionA.Content = allChoices[0];
            OptionB.Content = allChoices[1];
            OptionC.Content = allChoices[2];
            OptionD.Content = allChoices[3];

            foreach (var btn in optionButtons)
            {
                btn.IsEnabled = true;
                btn.Background = Brushes.LightGray;
            }
            selectedButton = null;
            NextButton.IsEnabled = false;
            ScoreText.Text = $"Score: {currentScore}";
        }

        public void Option_Click(object sender, RoutedEventArgs e)
        {
            if (selectedButton != null)
                return; // Prevent multiple selections

            selectedButton = sender as Button;
            if (selectedButton == null)
                return;

            string chosen = selectedButton.Content?.ToString() ?? "";
            string correct = quizData[questionIndex].CorrectChoice;

            if (chosen == correct)
            {
                selectedButton.Background = Brushes.Green;
                currentScore++;
            }
            else
            {
                selectedButton.Background = Brushes.Red;
                // Highlight the correct answer
                foreach (var btn in optionButtons)
                {
                    if (btn.Content?.ToString() == correct)
                    {
                        btn.Background = Brushes.Green;
                        break;
                    }
                }
            }

            foreach (var btn in optionButtons)
                btn.IsEnabled = false;

            NextButton.IsEnabled = true;
            ScoreText.Text = $"Score: {currentScore}";
        }

        public void NextButton_Click(object sender, RoutedEventArgs e)
        {
            questionIndex++;
            ShowQuiz();
        }
    }

    public class QuizQuestion
    {
        public string Question { get; set; } = "";
        public string CorrectChoice { get; set; } = "";
        public List<string> Choices { get; set; } = new List<string>();
    }
}