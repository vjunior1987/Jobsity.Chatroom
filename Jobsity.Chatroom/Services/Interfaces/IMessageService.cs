using Jobsity.Chatroom.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Jobsity.Chatroom.Services.Interfaces
{
    public interface IMessageService
    {
        Task<List<MessageViewModel>> RetrieveMessages(int chatId);
        Task SendMessage(MessageViewModel message);
    }
}
