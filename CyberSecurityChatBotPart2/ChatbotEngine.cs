using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace CyberSecurityChatBotPart2
{
    public class ChatbotEngine
    {
        private readonly Dictionary<string, List<string>> keywordResponses;
        private readonly Dictionary<string, List<string>> randomResponses;
        private readonly Dictionary<string, string> userMemory;
        private readonly Dictionary<string, List<string>> sentimentKeywords;
        private readonly List<CyberTask> tasks;
        private readonly List<ActivityEntry> activityLog;
        private readonly List<QuizQuestion> quizQuestions;
        private readonly Random random;

        private string currentTopic = "";
        private string lastResponse = "";
        private string currentSentiment = "neutral";
        private bool quizInProgress;
        private int quizIndex;
        private int quizScore;

        public ChatbotEngine()
        {
            keywordResponses = InitializeKeywordResponses();
            randomResponses = InitializeRandomResponses();
            sentimentKeywords = InitializeSentimentKeywords();
            userMemory = new Dictionary<string, string>();
            tasks = new List<CyberTask>();
            activityLog = new List<ActivityEntry>();
            quizQuestions = InitializeQuizQuestions();
            random = new Random();
            AddActivity("Application started", "Chatbot loaded with Part 1, Part 2 and Part 3 features.");
        }

        public string GetResponse(string userInput)
        {
            if (string.IsNullOrWhiteSpace(userInput))
                return "Please type a message so I can help you with cybersecurity tips!";

            string lowerInput = userInput.ToLower().Trim();
            DetectSentiment(userInput);
            StoreInMemory(userInput);

            if (quizInProgress)
                return HandleQuizAnswer(userInput);

            string part3Response = HandlePart3Commands(userInput, lowerInput);
            if (!string.IsNullOrWhiteSpace(part3Response))
                return part3Response;

            string? followUpResponse = HandleFollowUp(userInput);
            if (followUpResponse != null)
            {
                lastResponse = followUpResponse;
                return GetSentimentAdjustedResponse(GetPersonalizedResponse(followUpResponse));
            }

            foreach (var keyword in keywordResponses.Keys)
            {
                if (lowerInput.Contains(keyword))
                {
                    currentTopic = keyword;
                    var responses = keywordResponses[keyword];
                    string response = responses[random.Next(responses.Count)];

                    if (keyword == "phishing" && randomResponses.ContainsKey("phishing_tips"))
                    {
                        var additionalTips = randomResponses["phishing_tips"];
                        response += "\n\n" + additionalTips[random.Next(additionalTips.Count)];
                    }

                    lastResponse = response;
                    AddActivity("Cybersecurity topic answered", $"Answered a question about {keyword}.");
                    return GetSentimentAdjustedResponse(GetPersonalizedResponse(response));
                }
            }

            foreach (var topic in randomResponses.Keys)
            {
                if (lowerInput.Contains(topic.Replace("_tips", "")))
                {
                    var responses = randomResponses[topic];
                    string response = responses[random.Next(responses.Count)];
                    lastResponse = response;
                    AddActivity("Cybersecurity tip given", $"Shared a {topic.Replace('_', ' ')} response.");
                    return GetSentimentAdjustedResponse(GetPersonalizedResponse(response));
                }
            }

            if (ContainsAny(lowerInput, "hello", "hi", "hey", "good day"))
            {
                string greeting = "Hello! I'm your cybersecurity assistant. " +
                                 (userMemory.ContainsKey("name") ? $"Nice to talk to you again, {userMemory["name"]}! " : "") +
                                 "How can I help you stay safe online today?";
                return GetSentimentAdjustedResponse(greeting);
            }

            if (lowerInput.Contains("thank") || lowerInput.Contains("thanks"))
                return "You're welcome! Stay safe online! 😊 Is there anything else about cybersecurity I can help you with?";

            return "🤔 I'm not sure I understand. Could you rephrase that?\n\n" +
                   "Try commands like:\n" +
                   "• add task - Review privacy settings\n" +
                   "• remind me to update my password tomorrow\n" +
                   "• start quiz\n" +
                   "• show activity log\n" +
                   "• show tasks\n\n" +
                   "You can also ask about password safety, scams, privacy, phishing, malware, encryption, backup, or 2FA.";
        }

        private string HandlePart3Commands(string userInput, string lowerInput)
        {
            if (IsActivityCommand(lowerInput))
                return ShowActivityLog();

            if (IsQuizCommand(lowerInput))
                return StartQuiz();

            if (IsShowTasksCommand(lowerInput))
                return ShowTasks();

            if (IsCompleteTaskCommand(lowerInput))
                return CompleteTask(lowerInput);

            if (IsDeleteTaskCommand(lowerInput))
                return DeleteTask(lowerInput);

            if (IsTaskCommand(lowerInput))
                return AddTaskFromInput(userInput, lowerInput);

            return string.Empty;
        }

        private bool IsTaskCommand(string input)
        {
            return ContainsAny(input, "add task", "create task", "new task", "task added", "remind me", "set reminder", "make a reminder", "remember to", "i need to remember");
        }

        private bool IsShowTasksCommand(string input)
        {
            return ContainsAny(input, "show tasks", "view tasks", "list tasks", "my tasks", "display tasks", "what tasks");
        }

        private bool IsCompleteTaskCommand(string input)
        {
            return ContainsAny(input, "complete task", "mark task", "done task", "finish task", "completed task");
        }

        private bool IsDeleteTaskCommand(string input)
        {
            return ContainsAny(input, "delete task", "remove task", "clear task");
        }

        private bool IsQuizCommand(string input)
        {
            return ContainsAny(input, "start quiz", "quiz", "mini game", "minigame", "play game", "test me", "ask me questions", "cybersecurity quiz");
        }

        private bool IsActivityCommand(string input)
        {
            return ContainsAny(input, "activity log", "show activity", "what have you done", "recent actions", "history", "show log", "log");
        }

        private static bool ContainsAny(string input, params string[] keywords)
        {
            return keywords.Any(input.Contains);
        }

        private string AddTaskFromInput(string userInput, string lowerInput)
        {
            string title = ExtractTaskTitle(userInput);
            DateTime? reminder = ExtractReminderDate(lowerInput);

            var task = new CyberTask
            {
                Id = tasks.Count == 0 ? 1 : tasks.Max(t => t.Id) + 1,
                Title = string.IsNullOrWhiteSpace(title) ? "Cybersecurity task" : title,
                Description = BuildTaskDescription(title),
                ReminderDate = reminder,
                Completed = false,
                CreatedDate = DateTime.Now
            };

            tasks.Add(task);
            AddActivity("Task added", $"Task #{task.Id}: {task.Title}" + (reminder.HasValue ? $". Reminder set for {reminder.Value:dd MMM yyyy}." : ". No reminder set."));

            var response = new StringBuilder();
            response.AppendLine($"✅ Task added: {task.Title}");
            response.AppendLine($"Description: {task.Description}");
            response.AppendLine(reminder.HasValue ? $"Reminder: {reminder.Value:dd MMM yyyy}" : "Reminder: none set");
            response.AppendLine();
            response.AppendLine("You can say 'show tasks', 'complete task 1', or 'delete task 1'.");
            return response.ToString();
        }

        private string ExtractTaskTitle(string input)
        {
            string title = input.Trim();
            string[] phrases = { "add task", "create task", "new task", "task added", "remind me to", "set reminder to", "make a reminder to", "remember to", "i need to remember to" };
            foreach (string phrase in phrases)
            {
                title = Regex.Replace(title, Regex.Escape(phrase), "", RegexOptions.IgnoreCase).Trim(' ', ':', '-', '.');
            }

            title = Regex.Replace(title, "\\b(today|tomorrow|next week|in \\d+ days?|in \\d+ weeks?|on \\d{1,2}/\\d{1,2}/\\d{2,4})\\b", "", RegexOptions.IgnoreCase).Trim(' ', ':', '-', '.');
            return string.IsNullOrWhiteSpace(title) ? "Review cybersecurity settings" : title;
        }

        private string BuildTaskDescription(string title)
        {
            string lower = title.ToLower();
            if (lower.Contains("password")) return "Update your password and make sure it is strong, unique, and stored safely.";
            if (lower.Contains("two-factor") || lower.Contains("2fa") || lower.Contains("authentication")) return "Enable two-factor authentication on important accounts to reduce account takeover risk.";
            if (lower.Contains("privacy")) return "Review account privacy settings to limit unnecessary sharing of personal information.";
            if (lower.Contains("backup")) return "Create or verify backups so important files can be recovered after malware or device failure.";
            if (lower.Contains("antivirus") || lower.Contains("malware")) return "Check that malware protection is enabled and updated.";
            return "Cybersecurity-related task added by the user.";
        }

        private DateTime? ExtractReminderDate(string input)
        {
            if (input.Contains("tomorrow")) return DateTime.Today.AddDays(1);
            if (input.Contains("next week")) return DateTime.Today.AddDays(7);
            if (input.Contains("today")) return DateTime.Today;

            Match days = Regex.Match(input, @"in (\d+) day");
            if (days.Success && int.TryParse(days.Groups[1].Value, out int d)) return DateTime.Today.AddDays(d);

            Match weeks = Regex.Match(input, @"in (\d+) week");
            if (weeks.Success && int.TryParse(weeks.Groups[1].Value, out int w)) return DateTime.Today.AddDays(w * 7);

            Match date = Regex.Match(input, @"on (\d{1,2})/(\d{1,2})/(\d{2,4})");
            if (date.Success && int.TryParse(date.Groups[1].Value, out int day) && int.TryParse(date.Groups[2].Value, out int month) && int.TryParse(date.Groups[3].Value, out int year))
            {
                if (year < 100) year += 2000;
                try { return new DateTime(year, month, day); } catch { return null; }
            }

            return null;
        }

        private string ShowTasks()
        {
            AddActivity("Tasks viewed", "User requested the task list.");
            if (tasks.Count == 0)
                return "📋 You have no tasks yet. Try: 'Add task - Enable two-factor authentication in 3 days'.";

            var sb = new StringBuilder();
            sb.AppendLine("📋 Your cybersecurity tasks:");
            foreach (var task in tasks)
            {
                sb.AppendLine($"{task.Id}. {(task.Completed ? "✅" : "⬜")} {task.Title}");
                sb.AppendLine($"   {task.Description}");
                sb.AppendLine(task.ReminderDate.HasValue ? $"   Reminder: {task.ReminderDate.Value:dd MMM yyyy}" : "   Reminder: none");
            }
            return sb.ToString();
        }

        private string CompleteTask(string input)
        {
            int id = ExtractFirstNumber(input);
            var task = tasks.FirstOrDefault(t => t.Id == id);
            if (task == null) return "I could not find that task. Use 'show tasks' to see task numbers.";
            task.Completed = true;
            AddActivity("Task completed", $"Task #{task.Id}: {task.Title}");
            return $"✅ Task marked as complete: {task.Title}";
        }

        private string DeleteTask(string input)
        {
            int id = ExtractFirstNumber(input);
            var task = tasks.FirstOrDefault(t => t.Id == id);
            if (task == null) return "I could not find that task. Use 'show tasks' to see task numbers.";
            tasks.Remove(task);
            AddActivity("Task deleted", $"Task #{task.Id}: {task.Title}");
            return $"🗑️ Task deleted: {task.Title}";
        }

        private int ExtractFirstNumber(string input)
        {
            Match match = Regex.Match(input, @"\d+");
            return match.Success && int.TryParse(match.Value, out int number) ? number : -1;
        }

        private string StartQuiz()
        {
            quizInProgress = true;
            quizIndex = 0;
            quizScore = 0;
            AddActivity("Quiz started", "Cybersecurity mini-game started.");
            return "🎮 Cybersecurity Mini-Game started! Type A, B, C, D, True, or False to answer.\n\n" + FormatCurrentQuestion();
        }

        private string FormatCurrentQuestion()
        {
            var q = quizQuestions[quizIndex];
            var sb = new StringBuilder();
            sb.AppendLine($"Question {quizIndex + 1}/{quizQuestions.Count}: {q.Question}");
            foreach (var option in q.Options) sb.AppendLine(option);
            return sb.ToString();
        }

        private string HandleQuizAnswer(string input)
        {
            string answer = NormalizeAnswer(input);
            var q = quizQuestions[quizIndex];
            bool correct = answer == q.CorrectAnswer;
            if (correct) quizScore++;

            var sb = new StringBuilder();
            sb.AppendLine(correct ? "✅ Correct!" : $"❌ Incorrect. Correct answer: {q.CorrectAnswer}");
            sb.AppendLine("Explanation: " + q.Explanation);
            sb.AppendLine();

            quizIndex++;
            if (quizIndex >= quizQuestions.Count)
            {
                quizInProgress = false;
                string message = quizScore >= 10 ? "Great job! You're a cybersecurity pro!" : quizScore >= 7 ? "Good work! Keep learning to stay safer online." : "Keep learning to stay safe online.";
                sb.AppendLine($"🏁 Quiz complete! Final score: {quizScore}/{quizQuestions.Count}");
                sb.AppendLine(message);
                AddActivity("Quiz completed", $"Final score: {quizScore}/{quizQuestions.Count}.");
            }
            else
            {
                sb.AppendLine(FormatCurrentQuestion());
            }

            return sb.ToString();
        }

        private string NormalizeAnswer(string input)
        {
            string value = input.Trim().ToLower();
            if (value.StartsWith("a")) return "A";
            if (value.StartsWith("b")) return "B";
            if (value.StartsWith("c")) return "C";
            if (value.StartsWith("d")) return "D";
            if (value.StartsWith("true") || value == "t") return "True";
            if (value.StartsWith("false") || value == "f") return "False";
            return value.ToUpper();
        }

        private string ShowActivityLog()
        {
            AddActivity("Activity log viewed", "User requested recent actions.");
            var recent = activityLog.TakeLast(10).ToList();
            var sb = new StringBuilder();
            sb.AppendLine("📝 Here's a summary of recent actions:");
            for (int i = 0; i < recent.Count; i++)
                sb.AppendLine($"{i + 1}. [{recent[i].TimeStamp:HH:mm}] {recent[i].Action} - {recent[i].Description}");
            return sb.ToString();
        }

        private void AddActivity(string action, string description)
        {
            activityLog.Add(new ActivityEntry { TimeStamp = DateTime.Now, Action = action, Description = description });
        }

        private Dictionary<string, List<string>> InitializeKeywordResponses()
        {
            return new Dictionary<string, List<string>>
            {
                ["password"] = new List<string>
                {
                    "🔑 Password Safety: Use strong, unique passwords for each account. Aim for at least 12 characters with uppercase, lowercase, numbers, and symbols. Consider using a password manager!",
                    "🔒 Pro tip: Enable two-factor authentication (2FA) whenever possible. It adds an extra layer of security to your accounts.",
                    "⚠️ Never share your passwords with anyone, and avoid using personal information like birthdays or pet names in your passwords."
                },
                ["scam"] = new List<string>
                {
                    "🎣 Scam Prevention: Always verify the sender's email address before clicking links or opening attachments.",
                    "🚨 Be suspicious of messages that create urgency, ask for money, or request personal details.",
                    "📞 If a company contacts you unexpectedly, use the official website or app to contact them back."
                },
                ["privacy"] = new List<string>
                {
                    "🛡️ Privacy Protection: Review privacy settings on social media and limit who can see your personal information.",
                    "🔍 Be careful about what you share online. Information such as your location, school, ID number, or birthday can be misused.",
                    "📱 Only give apps the permissions they truly need, such as camera, contacts, or location access."
                },
                ["phish"] = new List<string>
                {
                    "📧 Phishing Warning: Check for spelling mistakes, generic greetings, suspicious attachments, and links that do not match the real company website.",
                    "🎣 Hover over links before clicking to see the actual URL. Phishing sites often use misspelled domain names.",
                    "⚠️ Legitimate companies will not ask for your password by email or SMS."
                },
                ["help"] = new List<string>
                {
                    "Available topics: password, scam, privacy, phishing, malware, encryption, backup, 2FA. Part 3 commands: add task, show tasks, complete task 1, delete task 1, start quiz, show activity log."
                },
                ["malware"] = new List<string>
                {
                    "🦠 Malware Protection: Keep your antivirus software updated and run regular scans.",
                    "💾 Avoid downloading software from untrusted sources. Stick to official app stores and official websites."
                },
                ["encryption"] = new List<string>
                {
                    "🔐 Encryption converts your data into unreadable code to prevent unauthorized access. Use HTTPS websites and encrypted messaging apps."
                },
                ["backup"] = new List<string>
                {
                    "💾 Regular backups are crucial! Follow the 3-2-1 rule: 3 copies, 2 different media, 1 offsite backup."
                },
                ["2fa"] = new List<string>
                {
                    "🔑 Two-Factor Authentication adds a second verification step. Use authenticator apps instead of SMS when possible."
                }
            };
        }

        private Dictionary<string, List<string>> InitializeRandomResponses()
        {
            return new Dictionary<string, List<string>>
            {
                ["phishing_tips"] = new List<string>
                {
                    "🎣 Be cautious of urgent emails asking for immediate action - this is a common phishing tactic.",
                    "📧 Never click on links in unsolicited emails. Type the website address directly into your browser.",
                    "🔍 Check the sender's email address carefully - phishers often use addresses similar to legitimate ones.",
                    "📱 Scammers also use SMS (smishing) and phone calls (vishing). Be vigilant across all channels!"
                },
                ["password_tips"] = new List<string>
                {
                    "🔑 Use a passphrase of 4-6 random words for better security and memorability.",
                    "🔄 Change passwords immediately if you suspect any account has been compromised.",
                    "📝 Never reuse passwords across different accounts - each account needs its own unique password."
                },
                ["scam_warnings"] = new List<string>
                {
                    "🚨 If something seems too good to be true, it probably is a scam.",
                    "📞 Legitimate organizations won't demand immediate payment or personal info over the phone.",
                    "💳 Never share your credit card details in response to an unsolicited email or call."
                }
            };
        }

        private Dictionary<string, List<string>> InitializeSentimentKeywords()
        {
            return new Dictionary<string, List<string>>
            {
                ["worried"] = new List<string> { "worried", "anxious", "nervous", "scared", "concerned", "afraid", "panic" },
                ["frustrated"] = new List<string> { "frustrated", "annoyed", "angry", "upset", "mad", "irritated" },
                ["curious"] = new List<string> { "curious", "interested", "excited", "eager", "want to learn" },
                ["confused"] = new List<string> { "confused", "don't understand", "unsure", "unclear", "lost" }
            };
        }

        private List<QuizQuestion> InitializeQuizQuestions()
        {
            return new List<QuizQuestion>
            {
                new QuizQuestion("What should you do if you receive an email asking for your password?", new[] { "A) Reply with your password", "B) Delete the email", "C) Report the email as phishing", "D) Ignore it" }, "C", "Reporting phishing helps prevent scams and protects other users."),
                new QuizQuestion("True or False: You should use the same password for all accounts so you do not forget it.", new[] { "True", "False" }, "False", "Reusing passwords is risky because one breached account can expose all accounts."),
                new QuizQuestion("Which option is the strongest password?", new[] { "A) password123", "B) Sihle2005", "C) Blue!Tiger#River92", "D) 12345678" }, "C", "Long mixed passwords or passphrases are harder to guess."),
                new QuizQuestion("What does 2FA add to an account?", new[] { "A) A second verification step", "B) Faster internet", "C) Free storage", "D) Automatic backups" }, "A", "2FA requires another proof, such as an authenticator code."),
                new QuizQuestion("True or False: Public Wi-Fi can expose your data if you use unsafe websites.", new[] { "True", "False" }, "True", "Attackers can monitor insecure public networks. Use HTTPS and a trusted VPN when necessary."),
                new QuizQuestion("What is phishing?", new[] { "A) A fake message trying to steal information", "B) A computer backup", "C) A software update", "D) A password manager" }, "A", "Phishing tricks users into giving away passwords, money, or personal data."),
                new QuizQuestion("Which file attachment should you be most cautious about?", new[] { "A) Unexpected invoice.exe", "B) A photo from your camera", "C) A PDF you requested", "D) A school document from your teacher" }, "A", "Executable files from unknown sources can install malware."),
                new QuizQuestion("True or False: Software updates often include security fixes.", new[] { "True", "False" }, "True", "Updates patch vulnerabilities that attackers may exploit."),
                new QuizQuestion("What should you do before entering card details online?", new[] { "A) Check for HTTPS and the correct domain", "B) Use any link from social media", "C) Turn off antivirus", "D) Share the page publicly" }, "A", "HTTPS and the correct website address help confirm you are on the real site."),
                new QuizQuestion("What is a backup?", new[] { "A) A copy of data for recovery", "B) A type of virus", "C) A weak password", "D) A phishing link" }, "A", "Backups help recover files after accidental deletion, theft, ransomware, or device failure."),
                new QuizQuestion("True or False: You should install apps only from trusted sources.", new[] { "True", "False" }, "True", "Trusted stores and official websites reduce the risk of malware."),
                new QuizQuestion("Which personal detail should you avoid posting publicly?", new[] { "A) Your ID number", "B) Your favourite movie", "C) A general hobby", "D) A cybersecurity tip" }, "A", "Identity numbers and sensitive personal data can be used for fraud."),
                new QuizQuestion("What should you do if an account may be compromised?", new[] { "A) Ignore it", "B) Change the password and enable 2FA", "C) Share the password", "D) Disable updates" }, "B", "Changing the password and enabling 2FA helps regain control and prevent further access."),
                new QuizQuestion("True or False: A password manager can help create and store unique passwords.", new[] { "True", "False" }, "True", "Password managers reduce password reuse and help generate strong passwords."),
                new QuizQuestion("What is social engineering?", new[] { "A) Manipulating people into revealing information", "B) Building social media apps", "C) Updating Windows", "D) Encrypting files" }, "A", "Social engineering targets human trust rather than only technical systems.")
            };
        }

        private string DetectSentiment(string input)
        {
            string lowerInput = input.ToLower();
            foreach (var sentiment in sentimentKeywords)
            {
                foreach (var keyword in sentiment.Value)
                {
                    if (lowerInput.Contains(keyword))
                    {
                        currentSentiment = sentiment.Key;
                        return currentSentiment;
                    }
                }
            }
            currentSentiment = "neutral";
            return currentSentiment;
        }

        private string GetSentimentAdjustedResponse(string baseResponse)
        {
            switch (currentSentiment)
            {
                case "worried":
                    return "😟 It's completely normal to feel worried about cybersecurity threats. " + baseResponse + "\n\nTaking small steps can significantly improve your online safety.";
                case "frustrated":
                    return "😤 I understand cybersecurity can feel frustrating sometimes. Let me simplify it: " + baseResponse;
                case "curious":
                    return "🤔 Great question! I'm glad you're curious about staying safe online. " + baseResponse;
                case "confused":
                    return "🤔 Let me clarify that for you: " + baseResponse;
                default:
                    return baseResponse;
            }
        }

        private void StoreInMemory(string input)
        {
            string lowerInput = input.ToLower();
            Match nameMatch = Regex.Match(input, @"(?:my name is|i'm|i am)\s+([A-Za-z]+)", RegexOptions.IgnoreCase);
            if (nameMatch.Success) userMemory["name"] = nameMatch.Groups[1].Value;

            foreach (var topic in keywordResponses.Keys)
            {
                if (lowerInput.Contains(topic))
                {
                    userMemory["interest"] = topic;
                    break;
                }
            }
        }

        private string GetPersonalizedResponse(string response)
        {
            if (userMemory.ContainsKey("name") && !string.IsNullOrEmpty(userMemory["name"]) && random.Next(3) == 0)
                response = $"Thanks for asking, {userMemory["name"]}! " + response;

            if (userMemory.ContainsKey("interest") && !string.IsNullOrEmpty(userMemory["interest"]) && response.Contains("security") && random.Next(2) == 0)
                response += $"\n\nSince you're interested in {userMemory["interest"]}, would you like more specific tips on this topic?";

            return response;
        }

        private string? HandleFollowUp(string input)
        {
            string lowerInput = input.ToLower();
            if (lowerInput.Contains("another") || lowerInput.Contains("more tip") || lowerInput.Contains("tell me another") || lowerInput.Contains("another one"))
            {
                if (!string.IsNullOrEmpty(currentTopic) && randomResponses.ContainsKey(currentTopic + "_tips"))
                    return randomResponses[currentTopic + "_tips"][random.Next(randomResponses[currentTopic + "_tips"].Count)];
                if (!string.IsNullOrEmpty(currentTopic) && keywordResponses.ContainsKey(currentTopic))
                    return keywordResponses[currentTopic][random.Next(keywordResponses[currentTopic].Count)];
            }
            else if (lowerInput.Contains("explain more") || lowerInput.Contains("tell me more") || lowerInput.Contains("elaborate") || lowerInput.Contains("more details"))
            {
                if (!string.IsNullOrEmpty(currentTopic) && keywordResponses.ContainsKey(currentTopic))
                    return keywordResponses[currentTopic].First();
            }
            return null;
        }

        public string GetCurrentTopic()
        {
            return string.IsNullOrEmpty(currentTopic) ? "cybersecurity" : currentTopic;
        }
    }

    public class CyberTask
    {
        public int Id { get; set; }
        public string Title { get; set; } = "";
        public string Description { get; set; } = "";
        public DateTime? ReminderDate { get; set; }
        public bool Completed { get; set; }
        public DateTime CreatedDate { get; set; }
    }

    public class ActivityEntry
    {
        public DateTime TimeStamp { get; set; }
        public string Action { get; set; } = "";
        public string Description { get; set; } = "";
    }

    public class QuizQuestion
    {
        public string Question { get; }
        public List<string> Options { get; }
        public string CorrectAnswer { get; }
        public string Explanation { get; }

        public QuizQuestion(string question, IEnumerable<string> options, string correctAnswer, string explanation)
        {
            Question = question;
            Options = options.ToList();
            CorrectAnswer = correctAnswer;
            Explanation = explanation;
        }
    }
}
