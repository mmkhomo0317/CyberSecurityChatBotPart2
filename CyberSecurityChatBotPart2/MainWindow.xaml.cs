using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows.Threading;

namespace CyberSecurityChatBotPart2
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly AudioPlayer _audio;
        private ChatbotEngine chatbot;
        private DispatcherTimer typingTimer;
        private int charIndex;
        private string currentResponse;

        public MainWindow()
        {
            InitializeComponent();
            _audio = new AudioPlayer();
            InitializeChatbot();
            Loaded += MainWindow_Loaded;
        }

        private async void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            AddWelcomeMessage();
            await _audio.PlayGreetingAsync();
            txtUserInput.Focus();
        }

        private void InitializeChatbot()
        {
            chatbot = new ChatbotEngine();
        }

        private void AddWelcomeMessage()
        {
            // Display the ASCII art logo from AsciiArt.cs class
            AppendToChat("Bot", AsciiArt.Logo, Colors.LightGreen);

            // Add a separator line
            AppendToChat("Bot", new string('═', 70), Colors.Gray);

            // Add welcome message with box art
            string welcomeArt = @"
╔══════════════════════════════════════════════════════════════╗
║                   🔐 CYBERSECURITY CHATBOT 🔐               ║
║                                                              ║
║  Welcome to your personal cybersecurity assistant!           ║
║  I can help you with:                                        ║
║  • Password safety 🔑                                        ║
║  • Scam prevention 🎣                                        ║
║  • Privacy protection 🛡️                                     ║
║  • Phishing tips 📧                                          ║
║  • Tasks, reminders, quiz, NLP and activity log              ║
║                                                              ║
║  Try: add task, show tasks, start quiz, show activity log     ║
╚══════════════════════════════════════════════════════════════╝";

            AppendToChat("Bot", welcomeArt, Colors.LightGreen);
        }

        private void AppendToChat(string sender, string message, Color color)
        {
            Dispatcher.Invoke(() =>
            {
                var paragraph = new Paragraph();

                // Add timestamp
                var timestampRun = new Run($"[{DateTime.Now.ToString("HH:mm:ss")}] ")
                {
                    Foreground = new SolidColorBrush(Colors.Gray),
                    FontSize = 10
                };
                paragraph.Inlines.Add(timestampRun);

                // Add sender
                var senderRun = new Run($"{sender}: ")
                {
                    Foreground = new SolidColorBrush(color),
                    FontWeight = FontWeights.Bold
                };
                paragraph.Inlines.Add(senderRun);

                // Add message with appropriate font for ASCII art
                var messageRun = new Run(message)
                {
                    Foreground = new SolidColorBrush(Colors.White)
                };

                // Use Consolas font for ASCII art (contains box drawing characters)
                if (message.Contains("╔") || message.Contains("█") || message.Contains("║") || message.Contains("═"))
                {
                    messageRun.FontFamily = new FontFamily("Consolas");
                    messageRun.FontSize = 10;
                }
                else
                {
                    messageRun.FontFamily = new FontFamily("Segoe UI");
                    messageRun.FontSize = 12;
                }

                paragraph.Inlines.Add(messageRun);

                rtxtChatHistory.Document.Blocks.Add(paragraph);

                // Add spacing
                rtxtChatHistory.Document.Blocks.Add(new Paragraph(new Run(" "))
                {
                    Margin = new Thickness(0, 0, 0, 5)
                });

                // Scroll to bottom
                chatScrollViewer.ScrollToBottom();
            });
        }

        private void SimulateTyping(string response)
        {
            currentResponse = response;
            charIndex = 0;

            Dispatcher.Invoke(() =>
            {
                var paragraph = new Paragraph();
                var timestampRun = new Run($"[{DateTime.Now.ToString("HH:mm:ss")}] ")
                {
                    Foreground = new SolidColorBrush(Colors.Gray),
                    FontSize = 10
                };
                paragraph.Inlines.Add(timestampRun);

                var senderRun = new Run($"Bot: ")
                {
                    Foreground = new SolidColorBrush(Colors.LightGreen),
                    FontWeight = FontWeights.Bold
                };
                paragraph.Inlines.Add(senderRun);

                rtxtChatHistory.Document.Blocks.Add(paragraph);

                typingTimer = new DispatcherTimer();
                typingTimer.Interval = TimeSpan.FromMilliseconds(30);
                typingTimer.Tick += (s, e) => TypingTimer_Tick(s, e, paragraph);
                typingTimer.Start();
            });
        }

        private void TypingTimer_Tick(object sender, EventArgs e, Paragraph paragraph)
        {
            if (charIndex < currentResponse.Length)
            {
                var run = paragraph.Inlines.LastOrDefault() as Run;
                if (run == null || run.Text == "Bot: ")
                {
                    run = new Run("");
                    paragraph.Inlines.Add(run);
                }

                run.Text += currentResponse[charIndex];
                charIndex++;

                // Scroll to bottom
                chatScrollViewer.ScrollToBottom();
            }
            else
            {
                typingTimer.Stop();
                // Add spacing after response
                Dispatcher.Invoke(() =>
                {
                    rtxtChatHistory.Document.Blocks.Add(new Paragraph(new Run(" "))
                    {
                        Margin = new Thickness(0, 0, 0, 5)
                    });
                    chatScrollViewer.ScrollToBottom();
                });
            }
        }

        private void ProcessUserInput()
        {
            string userInput = txtUserInput.Text.Trim();
            if (string.IsNullOrEmpty(userInput))
                return;

            AppendToChat("You", userInput, Colors.Gold);
            txtUserInput.Clear();

            string response = chatbot.GetResponse(userInput);
            SimulateTyping(response);
        }

        private void BtnSend_Click(object sender, RoutedEventArgs e)
        {
            ProcessUserInput();
        }

        private void TxtUserInput_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == System.Windows.Input.Key.Enter)
            {
                ProcessUserInput();
                e.Handled = true;
            }
        }

        private void BtnVoiceGreeting_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Play a simple system sound as greeting
                System.Media.SystemSounds.Asterisk.Play();
                AppendToChat("Bot", "🔊 Hello! Thanks for using the Cybersecurity Awareness Chatbot! Stay safe online! 🔒", Colors.LightGreen);
            }
            catch (Exception ex)
            {
                AppendToChat("System", $"Note: Audio system: {ex.Message}", Colors.Gray);
            }
        }

        private void BtnClear_Click(object sender, RoutedEventArgs e)
        {
            rtxtChatHistory.Document.Blocks.Clear();
            AddWelcomeMessage();
        }
    }
}