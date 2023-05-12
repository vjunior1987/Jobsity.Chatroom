using Jobsity.Chatroom.Data.Entities;
using Jobsity.Chatroom.Data.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Jobsity.Chatroom.Data.Repositories
{
    /// <summary>
    /// Repository Class for handling Database queries
    /// </summary>
    public class MessageRepository : IMessageRepository
    {
        private readonly ApplicationDbContext context;
        public MessageRepository(ApplicationDbContext context)
        {
            this.context = context;
        }

        public IQueryable<Message> RetrieveMessages()
        {
            return context.Message.OrderByDescending(x => x.TimeStamp).Take(50).OrderBy(x => x.TimeStamp);
        }

        public async Task SendMessage(Message entity)
        {
            await context.Message.AddAsync(entity);
            context.SaveChanges();
        }
    }
}
