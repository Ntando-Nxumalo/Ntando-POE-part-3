ST10456704_CyberSecurityBot
Project Name: ST10456704_CyberSecurityBot
Framework: .NET Framework 4.8
Template: Console App (Part 1 & 2), WPF App (Part 3)
Author: Ntando Nxumalo
Student Number: ST10456704

Part 1 Features
(Console App)

Audio Greeting: Plays Greetings.wav on startup with auto and manual path detection.

ASCII Logo: Converts CyberSafetyBot.bmp to ASCII with error handling.

User Interaction Flow: Basic input/output with greeting.

Cybersecurity Topics: Passwords, phishing, online safety.

Error Handling: Graceful feedback for unknown or invalid input.

Exit Command: Recognizes 'exit'.

Part 2 Enhancements
(Console App)

Dynamic Keyword Recognition: Detects keywords like "phishing", "malware", "password".

Randomized Responses: Natural variation in replies.

Memory & Personalization: Bot remembers user interests (e.g., privacy).

Sentiment Detection: Changes tone based on user emotion (worried, frustrated, curious).

Enhanced Error Handling: Offers suggestions and context-aware feedback.

Smart Recognition: Fixes typos like "fishing" → "phishing".

Persistent User Memory: Saves user preferences to userprefs_{username}.txt.

✅ Part 3: WPF GUI Application & Advanced Interaction
(Graphical WPF-based Upgrade)

🔹 Project Structure
Migrated the bot into a WPF GUI Application.

Introduced modular UserControls:

ChatbotControl

ReminderControl

QuizzQuestionControl

ActivityLogWindow

🔹 New Functional Features
✅ Task & Reminder System (NLP-Based)
Users can add tasks or say things like “Remind me to update antivirus in 3 days”.

The bot parses days from input using simple NLP logic and schedules reminders.

Dynamic prompt if the reminder duration is missing or unclear.

Double-click interaction: marks tasks as complete or deletes them.

✅ Cybersecurity Quiz Game
Interactive WPF-based quiz using QuizzQuestionControl.

20 multiple-choice questions (4 options each).

Score displayed at the end.

Optionally expandable for leaderboard or categories.

✅ Activity Log System
All major actions are now logged automatically:

Task creation

Reminder setting

Chatbot interactions (NLP)

Activity logs are timestamped and retrievable from a dedicated Activity Log Window.

Logs are limited to the last 100 entries for performance.

Includes a “Clear Log” button with confirmation.

✅ Global Singleton Logger
Implemented ActivityLogger as a singleton for consistent logging across controls.

Accessible via ActivityLogger.Instance throughout the app.

✅ Chatbot Enhancements
Full NLP interaction ported from console to GUI.

Every user input and bot response is logged.

The bot remembers and greets the user by name.

🛠️ Technical Enhancements
.NET 4.8 WPF GUI with sidebar navigation (Chats, Reminders, Quiz, Activity Log, Exit).

Reusable UserControls with clean content-switching inside the main window.

Centralized logging system using in-memory list and timestamped entries.

Refactored code for error handling and modularity (especially Reminder NLP logic).

Fixes for constructor initialization issues, null safety, and cleaner interface feedback.

Example Activity Log Output:
makefile
Copy
Edit
12:02:15 [Task] Enable two-factor authentication
12:02:18 [Reminder] Review privacy settings in 3 days
12:02:20 [NLP] "User said: remind me to change password"
12:03:05 [NLP] "User: How do I protect myself from phishing?"
12:03:07 [NLP] "Bot: Avoid clicking on suspicious links in emails."
👤 Author
Developed by: Ntando Nxumalo
Student Number: ST10456704
