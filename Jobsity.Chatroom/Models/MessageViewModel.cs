﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Jobsity.Chatroom.Models
{
    public class MessageViewModel
    {
        public string Content { get; set; }
        public DateTime TimeStamp { get; set; }
        public string UserName { get; set; }
        public int ChatroomId { get; set; }

        public bool ValidObject()
        {
            return !string.IsNullOrWhiteSpace(Content)
                && TimeStamp > DateTime.MinValue
                && !string.IsNullOrWhiteSpace(UserName)
                && ChatroomId != 0;
        }
    }
}
