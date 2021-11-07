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

namespace Jobsity.Chatroom.Services
{
    /// <summary>
    /// Service class containing requirement specifications
    /// </summary>
    public class MessageService : IMessageService
    {
        private readonly IMessageRepository repository;
        private readonly IMapper mapper;
        public MessageService(IMessageRepository repository, IMapper mapper)
        {
            this.repository = repository;
            this.mapper = mapper;
        }
        public async Task<List<MessageViewModel>> ReceiveMessages()
        {
            var list = repository.RetrieveMessages().ToList();
            //using(var client = new HttpClient()){
            //    client.BaseAddress()
            //}
            return mapper.Map<List<MessageViewModel>>(list);
        }

        public async Task SendMessage(MessageViewModel message)
        {
            await repository.SetMessage(mapper.Map<Message>(message));
        }
    }
}
