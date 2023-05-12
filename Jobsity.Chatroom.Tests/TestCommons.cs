using Jobsity.Chatroom.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Jobsity.Chatroom.Tests
{
    public static class TestCommons
    {
        public const int chatId = 1;
        public const string userName = "TestUser001";
        public const string content = "Test 001";
        public static DateTime timeStamp = DateTime.Now;

        public static MessageViewModel GetMessageViewModel(int chatroomId, string message = content)
        {
            return new MessageViewModel
            {
                ChatroomId = chatroomId,
                TimeStamp = timeStamp,
                UserName = userName,
                Content = message
            };
        }
    }
}
