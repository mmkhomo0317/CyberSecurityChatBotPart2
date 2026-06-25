using System;
using System.Collections.Generic;
using System.Text;

namespace CyberSecurityChatBotPart2
{
    public class MemoryStore
    {
        private string? _userName;
        private string? _favouriteTopic;

        public void RememberUserName(string name)
        {
            _userName = name;
        }

        public string GetUserName()
        {
            return _userName ?? "Friend";
        }

        public void RememberFavouriteTopic(string topic)
        {
            _favouriteTopic = topic;
        }

        public string GetFavouriteTopic()
        {
            return _favouriteTopic ?? "";
        }

        public bool HasUserName => _userName != null;
        public bool HasFavouriteTopic => _favouriteTopic != null;
    }
}
