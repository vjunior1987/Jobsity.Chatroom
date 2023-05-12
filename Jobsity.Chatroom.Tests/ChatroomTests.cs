using AutoMapper;
using Jobsity.Chatroom.Data.Entities;
using Jobsity.Chatroom.Data.Interfaces;
using Jobsity.Chatroom.Data.MappingProfiles;
using Jobsity.Chatroom.Models;
using Jobsity.Chatroom.Services;
using Jobsity.Chatroom.Services.Interfaces;
using NSubstitute;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace Jobsity.Chatroom.Tests
{
    public class ChatroomTests
    {
        private const int chatId = 1;
        private const string userName = "TestUser001";
        private const string content = "Test 001";
        private readonly IMessageService _messageService;
        private readonly IMessageRepository _messageRepository;
        private readonly IMapper _mapper;

        public ChatroomTests()
        {
            _mapper = new MapperConfiguration(cfg => cfg.AddProfile<MessageProfile>()).CreateMapper();
            _messageRepository = Substitute.For<IMessageRepository>();
            _messageService = new MessageService(_messageRepository, _mapper);
        }
         
        [Fact, Description("Happy path for sending message")]
        public async Task MessageService_SendMessage_HappyPath()
        {
            // Arrange
            MessageViewModel message = GetMessageViewModel();

            _messageRepository.SendMessage(Arg.Any<Message>()).Returns(_ => Task.CompletedTask);

            // Act
            await _messageService.SendMessage(message);

            // Assert
            await _messageRepository.Received().SendMessage(Arg.Any<Message>());
        }

        [Fact, Description("Happy path for sending bot commands")]
        public async Task MessageService_SendBotMessage_HappyPath()
        {
            // Arrange
            var messages = GetMessageViewModels();
            _messageRepository.RetrieveMessages().Returns(_mapper.Map<IEnumerable<Message>>(messages).AsQueryable());

            // Act
            var result = await _messageService.ReceiveMessages(1);

            // Assert
            Assert.Equal(messages, result.ToList());
        }

        //[Fact, Description("Happy path for sending bot help command")]
        //public void MessageService_SendBotMessageWithHelp_HappyPath()
        //{
        //    // Arrange
        //    // Act
        //    // Assert
        //}

        //[Fact, Description("Happy path for retrieving message")]
        //public void MessageService_RetrieveMessage_HappyPath()
        //{
        //    // Arrange
        //    // Act
        //    // Assert
        //}

        //[Fact, Description("Exception path for sending message with no user logged in")]
        //public void MessageService_SendMessageWithNoUserSession_Fail()
        //{
        //    // Arrange
        //    // Act
        //    // Assert
        //}

        //[Fact, Description("Exception path for sending message with no message")]
        //public void MessageService_SendMessageWithNoMessage_Fail()
        //{
        //    // Arrange
        //    // Act
        //    // Assert
        //}

        //[Fact, Description("Exception path for sending bot invalid command")]
        //public void MessageService_SendBotMessageWithInvalidCommand_Fail()
        //{
        //    // Arrange
        //    // Act
        //    // Assert
        //}

        private MessageViewModel GetMessageViewModel()
        {
            return new MessageViewModel
            {
                ChatroomId = chatId,
                TimeStamp = DateTime.Now,
                UserName = userName,
                Content = content
            };
        }


        private IEnumerable<MessageViewModel> GetMessageViewModels()
        {
            yield return GetMessageViewModel();
            yield return GetMessageViewModel();
            yield return GetMessageViewModel();
        }
    }
}
