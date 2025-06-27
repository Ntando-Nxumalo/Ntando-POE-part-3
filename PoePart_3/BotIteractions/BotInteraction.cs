using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;

namespace PoePart_3.BotInteractions
{
    public class BotInteraction
    {
        private string username = "";
        private string currentTopic = "";
        private bool inConversation = false;
        private readonly Random random = new Random();

        // User preference and history
        private readonly Dictionary<string, string> userPreferences = new Dictionary<string, string>();
        private readonly List<string> discussedTopics = new List<string>();
        private readonly Dictionary<string, List<int>> shownResponses = new Dictionary<string, List<int>>();

        // Output handler for GUI
        private Action<string> _outputHandler;
        public void SetOutputHandler(Action<string> handler) => _outputHandler = handler;

        private readonly Dictionary<string, List<string>> responseLibrary = new Dictionary<string, List<string>>()
        {
            {"phishing", new List<string>
                {
                    "Phishing attacks trick users into revealing sensitive information by impersonating trusted entities.",
                    "Look out for urgent language, unfamiliar senders, and requests for sensitive details in emails.",
                    "Always verify links by hovering over them and never enter login credentials on suspicious websites.",
                    "Be cautious of emails asking for personal information. Scammers often disguise themselves as trusted organisations.",
                    "Check the sender's email address carefully - phishing emails often use addresses that look similar to legitimate ones."
                }
            },
            {"passwords", new List<string>
                {
                    "Create passwords that are at least 14 characters long for better security.",
                    "Use a mix of uppercase and lowercase letters, numbers, and special characters to strengthen passwords.",
                    "Avoid reusing passwords across multiple accounts to prevent credential leaks.",
                    "Consider using passphrases, which are longer and easier to remember than random passwords.",
                    "Make sure to use strong, unique passwords for each account. Avoid using personal details in your passwords."
                }
            },
            {"security", new List<string>
                {
                    "Cybersecurity is about protecting your digital identity, data, and devices from malicious threats.",
                    "Enable multi-factor authentication (MFA) to add an extra layer of security.",
                    "Regularly update your software and security patches to guard against new threats.",
                    "Use a combination of strong passwords and biometric authentication where possible.",
                    "Be wary of public Wi-Fi networks. Use a VPN when accessing sensitive information on public networks."
                }
            },
            {"scams", new List<string>
                {
                    "Scams often try to create a sense of urgency to make you act without thinking.",
                    "If an offer seems too good to be true, it probably is. Be skeptical of unexpected prizes or winnings.",
                    "Never give out personal or financial information to someone who contacts you unexpectedly.",
                    "Tech support scams often call claiming your computer has a virus. Legitimate companies don't operate this way.",
                    "Romance scams target people on dating sites, often asking for money for emergencies or travel expenses."
                }
            },
            {"privacy", new List<string>
                {
                    "Protect your privacy by reviewing app permissions and only granting access to what's necessary.",
                    "Use privacy-focused browsers and search engines to minimize tracking of your online activities.",
                    "Regularly check your social media privacy settings to control who can see your information.",
                    "Be cautious about what personal information you share online - once it's out there, it's hard to take back.",
                    "Consider using encrypted messaging apps for sensitive communications to protect your privacy."
                }
            },
            {"general", new List<string>
                {
                    "Enable multi-factor authentication (MFA) on all important accounts for added security.",
                    "Use a password manager to generate and store strong, unique passwords for different accounts.",
                    "Keep your operating system, software, and antivirus programs updated to protect against vulnerabilities.",
                    "Backup your important data regularly to protect against ransomware attacks.",
                    "Be careful what you share on social media - attackers can use personal information to guess passwords or answers to security questions."
                }
            }
        };

        // Rest of your dictionaries and arrays remain the same...
        private readonly Dictionary<string, string> spellingCorrections = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
        {
            {"fishing", "phishing"},
            {"phisihng", "phishing"},
            {"phising", "phishing"},
            {"passsword", "password"},
            {"pasword", "password"},
            {"scamm", "scam"},
            {"privasy", "privacy"},
            {"securty", "security"},
            {"cybersec", "cyber security"}
        };

        private readonly Dictionary<string, string> keywordToTopic = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
        {
            {"password", "passwords"},
            {"passwords", "passwords"},
            {"strong password", "passwords"},
            {"login", "passwords"},
            {"credentials", "passwords"},
            {"phishing", "phishing"},
            {"phish", "phishing"},
            {"email scam", "phishing"},
            {"scam", "scams"},
            {"scams", "scams"},
            {"fraud", "scams"},
            {"con", "scams"},
            {"privacy", "privacy"},
            {"private", "privacy"},
            {"data protection", "privacy"},
            {"personal data", "privacy"},
            {"security", "security"},
            {"cyber", "security"},
            {"cybersecurity", "security"},
            {"safe browsing", "security"},
            {"hack", "security"},
            {"malware", "security"},
            {"virus", "security"}
        };

        private readonly string[] positiveWords = { "great", "good", "awesome", "happy", "thanks", "thank you", "cool", "excellent", "love", "like", "fantastic", "wonderful", "perfect" };
        private readonly string[] negativeWords = { "bad", "sad", "angry", "frustrated", "worried", "scared", "afraid", "hate", "annoyed", "terrible", "awful", "horrible", "stupid" };

        public void StartConversation(string user)
        {
            username = user?.Trim() ?? string.Empty;
            if (string.IsNullOrWhiteSpace(username))
            {
                _outputHandler?.Invoke("Please enter your name.");
                return;
            }
            LoadPreferencesFromFile();
            _outputHandler?.Invoke($"\nHello, {username}! Welcome to Cyber Security Bot.\n");
            if (userPreferences.ContainsKey("favoriteTopic"))
                _outputHandler?.Invoke($"Last time, you were interested in: {userPreferences["favoriteTopic"]}");
            if (discussedTopics.Count > 0)
                _outputHandler?.Invoke($"Previously discussed topics: {string.Join(", ", discussedTopics)}");
            DisplayPrompt();
        }

        public void ProcessUserInput(string input)
        {
            if (string.IsNullOrWhiteSpace(input))
            {
                _outputHandler?.Invoke("I didn't hear anything. Could you repeat that?");
                return;
            }

            if (input.Equals("exit", StringComparison.OrdinalIgnoreCase))
            {
                DisplayExitMessage();
                return;
            }

            input = CorrectMisspellings(input);

            var sentimentAnalysis = AnalyzeSentiment(input);

            // Handle context-aware follow-ups
            if (input.Contains("what should i know") || input.Contains("what else should i know"))
            {
                if (!string.IsNullOrEmpty(currentTopic))
                {
                    DisplayRandomResponse(currentTopic, sentimentAnalysis, true);
                    return;
                }
                else if (userPreferences.ContainsKey("favoriteTopic"))
                {
                    currentTopic = userPreferences["favoriteTopic"];
                    DisplayRandomResponse(currentTopic, sentimentAnalysis, true);
                    return;
                }
                else
                {
                    _outputHandler?.Invoke("What topic would you like to know more about?");
                    return;
                }
            }

            if (ProcessInterestStatement(input)) return;
            if (ProcessSpecialCommands(input)) return;

            string detectedTopic = DetectTopic(input);
            if (!string.IsNullOrEmpty(detectedTopic))
            {
                currentTopic = detectedTopic;
                inConversation = true;
                if (!discussedTopics.Contains(currentTopic)) discussedTopics.Add(currentTopic);
                DisplayRandomResponse(currentTopic, sentimentAnalysis, false);
            }
            else
            {
                HandleUnknownInput(input);
            }
        }

        private string GetUserPreferencesFile()
        {
            var safeName = string.Concat(username.Where(char.IsLetterOrDigit));
            return $"userprefs_{safeName}.txt";
        }

        private void SavePreferencesToFile()
        {
            if (string.IsNullOrWhiteSpace(username)) return;
            try
            {
                using var writer = new StreamWriter(GetUserPreferencesFile(), false);
                if (userPreferences.ContainsKey("favoriteTopic"))
                    writer.WriteLine($"favoriteTopic={userPreferences["favoriteTopic"]}");
                if (discussedTopics.Count > 0)
                    writer.WriteLine($"discussedTopics={string.Join(",", discussedTopics)}");
            }
            catch (Exception ex)
            {
                _outputHandler?.Invoke($"Failed to save preferences: {ex.Message}");
            }
        }

        private void LoadPreferencesFromFile()
        {
            userPreferences.Clear();
            discussedTopics.Clear();
            if (string.IsNullOrWhiteSpace(username)) return;
            var file = GetUserPreferencesFile();
            if (!File.Exists(file))
                return;

            try
            {
                foreach (var line in File.ReadAllLines(file))
                {
                    var parts = line.Split(new[] { '=' }, 2);
                    if (parts.Length == 2)
                    {
                        if (parts[0] == "favoriteTopic")
                            userPreferences["favoriteTopic"] = parts[1];
                        else if (parts[0] == "discussedTopics")
                            discussedTopics.AddRange(parts[1].Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries));
                    }
                }
            }
            catch (Exception ex)
            {
                _outputHandler?.Invoke($"Failed to load preferences: {ex.Message}");
            }
        }

        private string CorrectMisspellings(string input)
        {
            foreach (var correction in spellingCorrections)
            {
                if (input.Contains(correction.Key))
                {
                    _outputHandler?.Invoke($"Did you mean '{correction.Value}' instead of '{correction.Key}'?");
                    input = input.Replace(correction.Key, correction.Value);
                }
            }
            return input;
        }

        private void DisplayPrompt()
        {
            if (!inConversation)
            {
                _outputHandler?.Invoke("What would you like to ask me about cybersecurity?");
                _outputHandler?.Invoke("You can ask about: passwords, phishing, scams, privacy, or general security.");
            }
            else
            {
                _outputHandler?.Invoke($"What else would you like to know about {currentTopic}?");
                _outputHandler?.Invoke("(Or ask about a new topic, or type 'exit' to quit)");
            }
        }

        private bool ProcessInterestStatement(string input)
        {
            var interestPhrases = new List<string>
            {
                "i'm interested in",
                "i am interested in",
                "i like",
                "i love",
                "i enjoy",
                "i want to know about",
                "tell me about"
            };

            if (!interestPhrases.Any(phrase => input.Contains(phrase)))
                return false;

            string detectedTopic = DetectTopic(input);
            if (!string.IsNullOrEmpty(detectedTopic))
            {
                userPreferences["favoriteTopic"] = detectedTopic;
                _outputHandler?.Invoke($"Great! I'll remember that you're interested in {detectedTopic}.");
                currentTopic = detectedTopic;
                inConversation = true;
                DisplayRandomResponse(currentTopic, ("neutral", ""), false);
                return true;
            }

            _outputHandler?.Invoke("That sounds interesting! Could you tell me more about which cybersecurity topic you're interested in?");
            return true;
        }

        private bool ProcessSpecialCommands(string input)
        {
            var commands = new Dictionary<string, Action>
            {
                { "how are you", () => RespondToHowAreYou(AnalyzeSentiment(input)) },
                { "what's your purpose", DisplayPurpose },
                { "what is your purpose", DisplayPurpose },
                { "what can i ask you about", DisplayAvailableTopics },
                { "more", () => DisplayRandomResponse(currentTopic, AnalyzeSentiment(input), true) },
                { "another tip", () => DisplayRandomResponse(currentTopic, AnalyzeSentiment(input), true) },
                { "tell me more", () => DisplayRandomResponse(currentTopic, AnalyzeSentiment(input), true) },
                { "remember", RecallUserPreferences },
                { "recall", RecallUserPreferences },
                { "help", DisplayHelp },
                { "topics", DisplayAvailableTopics },
                { "what is my name", RecallUserName },
                { "what's my name", RecallUserName }
            };

            foreach (var command in commands)
            {
                if (input.Contains(command.Key))
                {
                    command.Value();
                    return true;
                }
            }

            return false;
        }

        private void RecallUserName()
        {
            if (!string.IsNullOrWhiteSpace(username))
                _outputHandler?.Invoke($"Your name is {username}.");
            else
                _outputHandler?.Invoke("I don't seem to have your name saved. Please tell me your name!");
        }

        private string DetectTopic(string input)
        {
            foreach (var kvp in keywordToTopic)
            {
                if (input.Contains(kvp.Key))
                    return kvp.Value;
            }
            return string.Empty;
        }

        private void DisplayRandomResponse(string topic, (string sentiment, string emotion) sentimentAnalysis, bool isFollowUp)
        {
            if (string.IsNullOrEmpty(topic) || !responseLibrary.ContainsKey(topic) || responseLibrary[topic].Count == 0)
            {
                _outputHandler?.Invoke("I seem to be missing information on that topic. Let's try something else.");
                return;
            }

            if (!shownResponses.ContainsKey(topic))
                shownResponses[topic] = new List<int>();

            var availableIndexes = Enumerable.Range(0, responseLibrary[topic].Count).Except(shownResponses[topic]).ToList();
            if (availableIndexes.Count == 0)
            {
                shownResponses[topic].Clear();
                availableIndexes = Enumerable.Range(0, responseLibrary[topic].Count).ToList();
            }

            int responseIndex = availableIndexes[random.Next(availableIndexes.Count)];
            shownResponses[topic].Add(responseIndex);

            string response = responseLibrary[topic][responseIndex];
            response = AdjustResponseBySentiment(response, sentimentAnalysis);

            if (isFollowUp)
                _outputHandler?.Invoke($"About {topic}, here's another tip: {response}");
            else
                _outputHandler?.Invoke(response);
        }

        private string AdjustResponseBySentiment(string response, (string sentiment, string emotion) sentimentAnalysis)
        {
            if (sentimentAnalysis.sentiment == "positive")
                response += " Great to see you're interested in staying safe!";
            else if (sentimentAnalysis.sentiment == "negative")
            {
                if (sentimentAnalysis.emotion == "worried")
                    response = "I understand this can be worrying. " + response + " Remember, you're taking the right step by learning about this.";
                else if (sentimentAnalysis.emotion == "frustrated")
                    response = "I know this can be frustrating. " + response + " Let's break it down to make it easier.";
                else
                    response = "I understand cybersecurity can be concerning. " + response + " Let me know if you'd like more help with this.";
            }
            return response;
        }

        private void RespondToHowAreYou((string sentiment, string emotion) sentimentAnalysis)
        {
            if (sentimentAnalysis.sentiment == "positive")
                _outputHandler?.Invoke($"I'm doing great, thank you for asking, {username}! I'm glad you're in a good mood. How about we talk about cybersecurity?");
            else if (sentimentAnalysis.sentiment == "negative")
            {
                if (sentimentAnalysis.emotion == "worried")
                    _outputHandler?.Invoke($"I'm here to help, {username}. I sense you might be feeling worried. Cybersecurity can be overwhelming, but I'll help you navigate it safely.");
                else if (sentimentAnalysis.emotion == "frustrated")
                    _outputHandler?.Invoke($"I understand cybersecurity can be frustrating, {username}. Let's take it one step at a time. What's troubling you?");
                else
                    _outputHandler?.Invoke($"I'm here to help, {username}. I sense you might be feeling down. Remember, staying safe online can help reduce stress and problems.");
            }
            else
                _outputHandler?.Invoke($"I'm doing well, {username}! Ready to help with any cybersecurity questions you have.");
        }

        private void DisplayPurpose()
        {
            _outputHandler?.Invoke("My purpose is to assist you with cybersecurity knowledge and keep you safe online. I can help with:");
            _outputHandler?.Invoke("- Password safety and management");
            _outputHandler?.Invoke("- Recognizing and preventing phishing attempts");
            _outputHandler?.Invoke("- Identifying and avoiding scams");
            _outputHandler?.Invoke("- Protecting your privacy online");
            _outputHandler?.Invoke("- General security best practices");
        }

        private void DisplayAvailableTopics()
        {
            _outputHandler?.Invoke("You can ask me about these cybersecurity topics:");
            _outputHandler?.Invoke("- Passwords (creating strong passwords, password managers)");
            _outputHandler?.Invoke("- Phishing (recognizing scam emails, protecting yourself)");
            _outputHandler?.Invoke("- Scams (common online scams, how to avoid them)");
            _outputHandler?.Invoke("- Privacy (protecting your personal information online)");
            _outputHandler?.Invoke("- Security (general cybersecurity best practices)");
            _outputHandler?.Invoke("- Malware (viruses, ransomware, protection)");
        }

        private void DisplayHelp()
        {
            _outputHandler?.Invoke("I can help you with cybersecurity information. Here's how to interact with me:");
            _outputHandler?.Invoke("- Ask about specific topics (e.g., 'tell me about phishing')");
            _outputHandler?.Invoke("- Say 'more' or 'what else should I know' to get additional information on the current topic");
            _outputHandler?.Invoke("- Say 'remember' to recall your preferences");
            _outputHandler?.Invoke("- Express your feelings and I'll adapt my responses");
            _outputHandler?.Invoke("- Type 'exit' to end our conversation");
        }

        private void RecallUserPreferences()
        {
            if (userPreferences.Count == 0)
            {
                _outputHandler?.Invoke("I don't have any preferences saved for you yet. You can tell me things like:");
                _outputHandler?.Invoke("- 'I'm interested in privacy'");
                _outputHandler?.Invoke("- 'I love learning about passwords'");
                _outputHandler?.Invoke("- 'Remember that I use a password manager'");
                return;
            }

            _outputHandler?.Invoke("Here's what I remember about you:");
            foreach (var pref in userPreferences)
                _outputHandler?.Invoke($"- {pref.Key.Replace("favorite", "favorite ")}: {pref.Value}");

            if (userPreferences.ContainsKey("favoriteTopic"))
                _outputHandler?.Invoke($"Since you're interested in {userPreferences["favoriteTopic"]}, would you like to discuss that now?");
        }

        private void HandleUnknownInput(string input)
        {
            if (input.EndsWith("?") || input.StartsWith("what") || input.StartsWith("how") || input.StartsWith("why"))
                _outputHandler?.Invoke("I'm not sure I understand your question. Could you try rephrasing it or ask about a specific cybersecurity topic?");
            else
            {
                _outputHandler?.Invoke("I didn't quite understand that. Could you rephrase or ask about a cybersecurity topic?");
                _outputHandler?.Invoke("Try asking about: passwords, phishing, scams, privacy, or general security.");
                _outputHandler?.Invoke("Or type 'help' for assistance.");
            }
            inConversation = false;
            currentTopic = "";
        }

        private (string sentiment, string emotion) AnalyzeSentiment(string input)
        {
            string sentiment = "neutral";
            string emotion = "";

            if (positiveWords.Any(word => input.Contains(word)))
                sentiment = "positive";
            else if (negativeWords.Any(word => input.Contains(word)))
            {
                sentiment = "negative";

                if (input.Contains("worried") || input.Contains("scared") || input.Contains("afraid"))
                    emotion = "worried";
                else if (input.Contains("frustrated") || input.Contains("angry") || input.Contains("annoyed"))
                    emotion = "frustrated";
            }
            return (sentiment, emotion);
        }

        private void DisplayExitMessage()
        {
            _outputHandler?.Invoke($"Goodbye, {username}! Remember to stay safe online.");
            if (userPreferences.ContainsKey("favoriteTopic"))
                _outputHandler?.Invoke($"Don't forget to practice what we discussed about {userPreferences["favoriteTopic"]}!");
            if (discussedTopics.Count > 0)
            {
                _outputHandler?.Invoke($"We discussed these topics today: {string.Join(", ", discussedTopics)}.");
                _outputHandler?.Invoke("Here's a quick summary of what we covered:");
                foreach (var topic in discussedTopics)
                {
                    if (responseLibrary.ContainsKey(topic) && responseLibrary[topic].Count > 0)
                        _outputHandler?.Invoke($"- {topic}: {responseLibrary[topic][0]}");
                }
            }
            SavePreferencesToFile();
        }
    }
}