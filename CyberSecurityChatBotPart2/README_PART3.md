# CyberSecurity Awareness Chatbot - Part 3 Add-On

This version keeps the original Part 2 WPF chatbot structure and adds the Part 3 requirements without replacing the Part 2 features.

## Preserved Part 2 Features
- Existing WPF/XAML chat interface
- Voice greeting button
- ASCII art display
- Keyword recognition for password, scam, privacy, phishing, malware, encryption, backup and 2FA
- Memory/personalised responses
- Sentiment-aware responses
- Follow-up responses

## Added Part 3 Features

### 1. Task Assistant with Reminders
Use commands such as:
- `add task - Enable two-factor authentication in 3 days`
- `remind me to update my password tomorrow`
- `show tasks`
- `complete task 1`
- `delete task 1`

### 2. Cybersecurity Mini-Game Quiz
Use:
- `start quiz`
- `play game`
- `test me`

The quiz includes 15 mixed multiple-choice and true/false cybersecurity questions, immediate feedback, explanations and a final score.

### 3. NLP Simulation
The chatbot recognises different phrases for tasks, reminders, quizzes and logs using keyword detection and regular expressions.

### 4. Activity Log
Use:
- `show activity log`
- `what have you done`
- `history`
- `recent actions`

The log records task actions, quiz attempts, topic responses and log views with timestamps.

## MySQL Storage Script
A MySQL script is included as `database_setup.sql`. The current classroom build stores tasks in memory so it can run immediately in Visual Studio without requiring a live MySQL server. The script shows the database structure required by the POE and can be connected later through a DatabaseManager class.
