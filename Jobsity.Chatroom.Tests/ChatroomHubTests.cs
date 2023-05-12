using Jobsity.Chatroom.Hubs;
using Microsoft.AspNetCore.SignalR;
using NSubstitute;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.ComponentModel;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Jobsity.Chatroom.Tests
{
    /// <summary>
    /// Integral Tests for the chatroomHub and StockBot. Have stockbot and the rabbitMQ docker container running in the background for tests
    /// </summary>
    public class ChatroomHubTests
    {
        private ChatroomHub _hub;

        public ChatroomHubTests()
        {
            _hub = Substitute.For<ChatroomHub>();
        }

        [Fact, Description("Should successfully send bot command and receive the stock quote")]
        public async Task ChatroomHub_SendBotMessage_HappyPath()
        {
            // Arrange
            var message = TestCommons.GetMessageViewModel(1, "/stock=AVE");

            // Act
            await _hub.SendBotMessage(message.Content, message.ChatroomId);

            // Assert
            AssertConsumerMessageBus("AVE");
        }

        [Fact, Description("Should successfully send bot command and receive the help instructions")]
        public async Task ChatroomHub_SendBotMessageWithHelp_HappyPath()
        {
            // Arrange
            var message = TestCommons.GetMessageViewModel(1, "/help");

            // Act
            await _hub.SendBotMessage(message.Content, message.ChatroomId);

            // Assert
            AssertConsumerMessageBus(Commons.Constants.CHATBOT_HELP_MESSAGE);
        }


        [Fact, Description("Should receive error message when sending bot command with invalid code")]
        public async Task ChatroomHub_SendBotMessageCommandWithInvalidCode_Fail()
        {
            // Arrange
            var message = TestCommons.GetMessageViewModel(1, "/stock=test001");
            message.Content = null;

            // Act
            await _hub.SendBotMessage(message.Content, message.ChatroomId);

            // Assert
            AssertConsumerMessageBus(Commons.Constants.CHATBOT_STOCK_NOT_FOUND_MESSAGE);
        }


        [Fact, Description("Should receive error message when sending invalid bot command")]
        public async Task ChatroomHub_SendBotMessageWithInvalidCommand_Fail()
        {
            // Arrange
            var message = TestCommons.GetMessageViewModel(1, "/test");
            message.Content = null;

            // Act
            await _hub.SendBotMessage(message.Content, message.ChatroomId);

            // Assert
            AssertConsumerMessageBus(Commons.Constants.CHATBOT_COMMAND_NOT_RECOGNIZED_MESSAGE); ;
        }

        private void AssertConsumerMessageBus(string expected)
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
                        Assert.Contains(expected, message[1]);
                    }
                };
                channel.BasicConsume(queue: "bot1", autoAck: true, consumer: consumer);
            };
        }
    }
}
