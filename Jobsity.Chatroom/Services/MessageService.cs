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
using RabbitMQ.Client;
using System.Text;
using RabbitMQ.Client.Events;
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
            ConsumerRabbitMQ(chatId);
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
                if (message.Content[0] == '/')
                {
                    using (var client = new HttpClient())
                    {
                        await SendBotMessage(message.Content, message.ChatroomId);
                        Thread.Sleep(2000);
                        ConsumerRabbitMQ(message.ChatroomId);
                    }
                }
                else
                {
                    await _repository.SendMessage(_mapper.Map<Message>(message));
                    await _hubcontext.Clients.All.SendAsync("ReceiveMessage", message);
                }


            }
            else
            {
                throw new ArgumentException();
            }
        }

        #region Send Message through RabbiMQ
        private async Task SendBotMessage(string command, int chatId)
        {
            var factory = new ConnectionFactory() { HostName = "localhost" };
            using (var connection = factory.CreateConnection())
            using (var channel = connection.CreateModel())
            {
                channel.QueueDeclare(queue: "chat1",
                                     durable: false,
                                     exclusive: false,
                                     autoDelete: false,
                                     arguments: null);

                string message = chatId.ToString() + "|" + command;
                var body = Encoding.UTF8.GetBytes(message);

                channel.BasicPublish(exchange: "",
                                     routingKey: "chat1",
                                     basicProperties: null,
                                     body: body);
            }
        }
        #endregion

        #region Retrieve bot messages

        public void ConsumerRabbitMQ(int chatId)
        {
            var factory = new ConnectionFactory() { HostName = "localhost" };
            using (var connection = factory.CreateConnection())
            using (var channel = connection.CreateModel())
            {
                channel.QueueDeclare(queue: "bot1", durable: false, exclusive: false, autoDelete: false, arguments: null);

                Console.WriteLine(" [*] Waiting for messages.");

                var consumer = new EventingBasicConsumer(channel);
                consumer.Received += (model, ea) =>
                {
                    var body = ea.Body.ToArray();
                    var message = Encoding.UTF8.GetString(body).Split('|');
                    if (Int32.TryParse(message[0], out int chatroomId))
                    {
                        var botResponse = new Message
                        {
                            TimeStamp = DateTime.Now,
                            Content = message[1],
                            UserName = "Stock Bot",
                            ChatroomId = chatroomId
                        };
                        _hubcontext.Clients.All.SendAsync("ReceiveBotMessage", botResponse);
                    }
                };
                channel.BasicConsume(queue: "bot1", autoAck: true, consumer: consumer);
            };
        }
        #endregion
    }
}

