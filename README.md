# Cybersecurity Awareness Chatbot
## PROG6221 Programming POE – Part 3

**Student:** Mkhaliphi Khomo  
**Module:** PROG6221 – Programming 2A  
**Institution:** The Independent Institute of Education (IIE)  
**Application Type:** Windows Presentation Foundation (WPF) Desktop Application  
**Language:** C# (.NET 10)  
**IDE:** Visual Studio 2022

---

# Project Overview

The Cybersecurity Awareness Chatbot is an intelligent desktop application developed using C# and Windows Presentation Foundation (WPF). The chatbot educates users about cybersecurity while providing interactive features that improve cybersecurity awareness.

The application was developed incrementally through three parts of the POE.

Part 3 extends the chatbot by introducing:

- Task Assistant
- Reminder System
- Cybersecurity Quiz
- Natural Language Processing (NLP)
- Activity Log
- Improved GUI
- Enhanced chatbot intelligence

---

# Features

## Part 1 Features

- Cybersecurity chatbot
- ASCII Art Welcome Banner
- Voice Greeting
- Cybersecurity Advice
- Keyword Detection
- Random Responses
- User-Friendly Interface

---

## Part 2 Features

- WPF Graphical User Interface
- Sentiment Detection
- Memory of User Information
- Dynamic Responses
- Follow-up Questions
- Improved Conversation Flow
- More Cybersecurity Topics

---

## Part 3 Features

### Task Assistant

Users can:

- Add cybersecurity tasks
- View tasks
- Delete tasks
- Mark tasks as completed
- Set reminders

Example:

```
Add task: Enable Two-Factor Authentication
```

---

### Cybersecurity Quiz

Features include:

- 15+ Questions
- Multiple Choice Questions
- True/False Questions
- Immediate Feedback
- Explanations
- Score Tracking
- Final Results

---

### Natural Language Processing (NLP)

The chatbot recognises different ways users ask the same question.

Examples:

```
Add task
Create task
Set reminder
Remember to
I need to remember
```

```
Start quiz
Play game
Test me
```

```
Show activity
History
What have you done?
```

---

### Activity Log

The chatbot records:

- Tasks Added
- Tasks Deleted
- Tasks Completed
- Quiz Started
- Quiz Completed
- User Commands
- Cybersecurity Advice Given

Users can view the log by typing:

```
Show activity
```

or

```
History
```

---

### Chatbot Intelligence

The chatbot recognises:

- Password questions
- Phishing
- Malware
- Encryption
- Backups
- Privacy
- Scams
- Two-Factor Authentication (2FA)

It also remembers:

- User Name
- Previous Topics
- Conversation Context

---

# Graphical User Interface

The application uses Windows Presentation Foundation (WPF) and includes:

- Modern interface
- RichTextBox Chat Window
- Text Input
- Send Button
- Voice Greeting Button
- Task Management
- Quiz Interface
- Activity Log
- Status Bar

---

# Technologies Used

- C#
- .NET 10
- Windows Presentation Foundation (WPF)
- XAML
- Visual Studio 2022
- Git
- GitHub

---

# Project Structure

```
CyberSecurityChatBot
│
├── Assets
│
├── Models
│     CyberTask.cs
│     QuizQuestion.cs
│     ActivityEntry.cs
│
├── Services
│     NLPService.cs
│     QuizService.cs
│
├── Database
│     DatabaseManager.cs
│
├── MainWindow.xaml
├── MainWindow.xaml.cs
├── ChatbotEngine.cs
├── MemoryStore.cs
├── AudioPlayer.cs
├── AsciiArt.cs
│
├── README.md
│
└── CyberSecurityChatBotPart2.csproj
```

---

# Installation

### Clone the Repository

```bash
git clone https://github.com/YourUsername/CyberSecurityChatBot.git
```

### Open the Project

Open the solution file in Visual Studio:

```
CyberSecurityChatBotPart2.slnx
```

### Restore Dependencies

From Visual Studio:

```
Build > Restore NuGet Packages
```

or using the .NET CLI:

```bash
dotnet restore
```

### Build the Project

```bash
dotnet build
```

### Run the Application

Press:

```
Ctrl + F5
```

or

```bash
dotnet run
```

---

# Example Commands

```
Hello
```

```
Password tips
```

```
Tell me about phishing
```

```
Add task Enable 2FA
```

```
Show tasks
```

```
Delete task 1
```

```
Complete task 2
```

```
Start quiz
```

```
History
```

```
Show activity
```

---

# Assessment Requirements Covered

- Task Assistant
- Reminder Feature
- Cybersecurity Quiz
- Natural Language Processing (NLP)
- Activity Log
- GUI Application
- Object-Oriented Programming
- Lists
- Dictionaries
- Classes
- Methods
- Exception Handling
- Modular Programming

---

# Cybersecurity Topics Covered

- Password Security
- Multi-Factor Authentication
- Phishing
- Social Engineering
- Malware
- Safe Browsing
- Online Privacy
- Data Encryption
- Secure Backups
- Email Scams

---

# Future Improvements

Possible future enhancements include:

- MySQL Database Integration
- User Login System
- Dark Mode
- AI-powered Responses
- Cloud Synchronisation
- Password Generator
- Password Strength Checker
- Export Activity Logs to PDF
- Notifications
- Mobile Version

---

# Author

**Mkhaliphi Khomo**

Programming 2A

The Independent Institute of Education (IIE)

---

# License

This project was developed for academic purposes as part of the PROG6221 Practical of Evidence (POE).

© 2026 Mkhaliphi Khomo

All Rights Reserved.
