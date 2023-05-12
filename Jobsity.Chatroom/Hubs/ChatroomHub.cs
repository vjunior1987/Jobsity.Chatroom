using Jobsity.Chatroom.Models;
using Microsoft.AspNetCore.SignalR;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Jobsity.Chatroom.Hubs
{
    public class ChatroomHub : Hub
    {
        #region Send Message through RabbiMQ
        public async Task SendBotMessage(string command, int chatId)
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
            Thread.Sleep(3000);
            if (Clients?.Caller != null)
            {
                await Clients.Caller.SendAsync("RequestBotMessage", chatId);
            }
        }
        #endregion

        #region Retrieve bot messages

        public void ConsumerMessageBus()
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
                        var botResponse = new MessageViewModel
                        {
                            TimeStamp = DateTime.Now,
                            Content = message[1],
                            UserName = "Stock Bot",
                            ChatroomId = chatroomId
                        };
                        Clients.Caller.SendAsync("PublishBotMessage", botResponse);
                    }
                };
                channel.BasicConsume(queue: "bot1", autoAck: true, consumer: consumer);
            };
        }
        #endregion
    }
}
