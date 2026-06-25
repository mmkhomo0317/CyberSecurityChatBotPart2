using System;
using System.Collections.Generic;
using System.Text;

namespace CyberSecurityChatBotPart2
{
    public class ResponseManager
    {
        private readonly Dictionary<string, string> _keywordResponses;
        private readonly List<string> _randomTips;
        private readonly Random _rng = new();

        public ResponseManager()
        {
            _keywordResponses = new Dictionary<string, string>
            {
                ["password"] = "🔐 Use strong, unique passwords for each account!",
                ["scam"] = "🚨 Never share personal info with unknown callers!",
                ["phishing"] = "🎣 Don't click suspicious links in emails!",
                ["privacy"] = "🛡️ Review your privacy settings regularly!"
            };

            _randomTips = new List<string>
            {
                "Use a password manager!",
                "Enable two-factor authentication!",
                "Back up your data regularly!"
            };
        }

        public string? GetKeywordResponse(string input)
        {
            string lower = input.ToLower();
            foreach (var keyword in _keywordResponses.Keys)
            {
                if (lower.Contains(keyword))
                    return _keywordResponses[keyword];
            }
            return null;
        }

        public string GetRandomTip()
        {
            return _randomTips[_rng.Next(_randomTips.Count)];
        }
    }
}
