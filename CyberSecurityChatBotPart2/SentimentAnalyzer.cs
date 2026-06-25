using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CyberSecurityChatBotPart2
{
    public enum Sentiment { Neutral, Worried, Frustrated, Curious }

    public class SentimentAnalyzer
    {
        private readonly List<string> _worriedWords = new() { "worried", "scared", "anxious" };
        private readonly List<string> _frustratedWords = new() { "frustrated", "annoyed", "confused" };
        private readonly List<string> _curiousWords = new() { "curious", "interested", "tell me more" };

        public Sentiment DetectSentiment(string input)
        {
            string lower = input.ToLower();

            if (_worriedWords.Any(w => lower.Contains(w))) return Sentiment.Worried;
            if (_frustratedWords.Any(w => lower.Contains(w))) return Sentiment.Frustrated;
            if (_curiousWords.Any(w => lower.Contains(w))) return Sentiment.Curious;

            return Sentiment.Neutral;
        }

        public string GetEmpathyPrefix(Sentiment sentiment)
        {
            return sentiment switch
            {
                Sentiment.Worried => "💙 It's normal to feel worried. ",
                Sentiment.Frustrated => "🤝 I understand this can be frustrating. ",
                Sentiment.Curious => "✨ Great question! ",
                _ => ""
            };
        }
    }
}