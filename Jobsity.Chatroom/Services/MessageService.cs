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

        /// <summary>
        /// Retrieves messages from database
        /// </summary>
        /// <returns></returns>
        public async Task<List<MessageViewModel>> ReceiveMessages(int chatId)
        {
            //Listen for Bot messages
            ConsumerRabbitMQ(chatId);
            var list = await repository.RetrieveMessages().Where(x => x.ChatroomId == chatId).ToListAsync();
            return mapper.Map<List<MessageViewModel>>(list);
        }

        /// <summary>
        ///  Post messages to database
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        public async Task SendMessage(MessageViewModel message)
        {
            //Handle command inputs
            if (!string.IsNullOrWhiteSpace(message.Content) && message.Content[0] == '/')
                using (var client = new HttpClient())
                {
                    await SendBotMessage(message.Content, message.ChatroomId);
                }
            else
                await repository.SendMessage(mapper.Map<Message>(message));
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

        private void ConsumerRabbitMQ(int chatId)
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
                    if(Int32.TryParse(message[0], out int chatroomId))
                    {
                        SendMessage(new MessageViewModel
                        {
                            TimeStamp = DateTime.Now,
                            Content = message[1],
                            UserName = "Stock Bot",
                            ChatroomId = chatroomId
                        });
                    }
                };
                channel.BasicConsume(queue: "bot1", autoAck: true, consumer: consumer);
            };
        }
    #endregion
    }
}

