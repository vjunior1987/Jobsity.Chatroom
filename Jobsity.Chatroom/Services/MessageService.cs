using Jobsity.Chatroom.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Jobsity.Chatroom.Models;
using Jobsity.Chatroom.Data.Interfaces;
using System.Net.Http;
using AutoMapper;
using Jobsity.Chatroom.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.SignalR;
using Jobsity.Chatroom.Hubs;
using System.Threading;

namespace Jobsity.Chatroom.Services
{
    /// <summary>
    /// Service class containing requirement specifications
    /// </summary>
    public class MessageService : IMessageService
    {
        private readonly IMessageRepository _repository;
        private readonly IMapper _mapper;
        private readonly IHubContext<ChatroomHub> _hubcontext;

        public MessageService(IMessageRepository repository, IMapper mapper, IHubContext<ChatroomHub> hubContext)
        {
            _repository = repository;
            _mapper = mapper;
            _hubcontext = hubContext;
        }

        /// <summary>
        /// Retrieves messages from database
        /// </summary>
        /// <returns></returns>
        public async Task<List<MessageViewModel>> RetrieveMessages(int chatId)
        {
            //Listen for Bot messages
            var list = await _repository.RetrieveMessages().Where(x => x.ChatroomId == chatId).ToListAsync();
            return _mapper.Map<List<MessageViewModel>>(list);
        }

        /// <summary>
        ///  Post messages to database
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        public async Task SendMessage(MessageViewModel message)
        {

            //Handle command inputs
            if (message.ValidObject())
            {
                await _repository.SendMessage(_mapper.Map<Message>(message));
                await _hubcontext.Clients.All.SendAsync("ReceiveMessage", message);
            }
            else
            {
                throw new ArgumentException();
            }
        }
    }
}

