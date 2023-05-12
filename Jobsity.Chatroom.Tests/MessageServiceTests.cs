using AutoMapper;
using Jobsity.Chatroom.Data.Entities;
using Jobsity.Chatroom.Data.Interfaces;
using Jobsity.Chatroom.Data.MappingProfiles;
using Jobsity.Chatroom.Hubs;
using Jobsity.Chatroom.Models;
using Jobsity.Chatroom.Services;
using Jobsity.Chatroom.Services.Interfaces;
using Microsoft.AspNetCore.SignalR;
using NSubstitute;
using Shouldly;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Jobsity.Chatroom.Tests
{
    public class MessageServiceTests
    {
        private readonly IMessageService _messageService;
        private readonly IMessageRepository _messageRepository;
        private readonly IMapper _mapper;
        private readonly IHubContext<ChatroomHub> _contextHub;

        public MessageServiceTests()
        {
            _contextHub = Substitute.For<IHubContext<ChatroomHub>>();
            _mapper = new MapperConfiguration(cfg => cfg.AddProfile<MessageProfile>()).CreateMapper();
            _messageRepository = Substitute.For<IMessageRepository>();
            _messageService = new MessageService(_messageRepository, _mapper, _contextHub);
        }

        [Fact, Description("Happy path for sending message")]
        public async Task MessageService_SendMessage_HappyPath()
        {
            // Arrange
            MessageViewModel message = TestCommons.GetMessageViewModel(1);

            _messageRepository.SendMessage(Arg.Any<Message>()).Returns(_ => Task.CompletedTask);

            // Act
            await _messageService.SendMessage(message);

            // Assert
            await _messageRepository.Received().SendMessage(Arg.Any<Message>());
        }

        [Fact, Description("Happy path for retrieving messages")]
        public async Task MessageService_ReceiveMessages_HappyPath()
        {
            // Arrange
            var messages = GetMessageViewModels(1);
            _messageRepository.RetrieveMessages().Returns(new TestAsyncEnumerable<Message>(_mapper.Map<IEnumerable<Message>>(messages)).AsQueryable());

            // Act
            var result = await _messageService.RetrieveMessages(1);

            // Assert
            messages.ToList().SequenceEqual(result);
        }

        [Fact, Description("Exception path for sending message with no user logged in")]
        public void MessageService_SendMessageWithNoUserSession_Fail()
        {
            // Arrange
            var message = TestCommons.GetMessageViewModel(1);
            message.UserName = null;

            // Act
            async Task SendMessage() => await _messageService.SendMessage(message);

            // Assert
            Assert.ThrowsAsync<ArgumentException>(SendMessage);
        }

        [Fact, Description("exception path for sending message with no message")]
        public void messageservice_sendmessagewithnomessage_fail()
        {
            // Arrange
            var message = TestCommons.GetMessageViewModel(1);
            message.Content = null;

            // Act
            async Task SendMessage() => await _messageService.SendMessage(message);

            // Assert
            Assert.ThrowsAsync<ArgumentException>(SendMessage);
        }

        private IEnumerable<MessageViewModel> GetMessageViewModels(int chatroomId)
        {
            yield return TestCommons.GetMessageViewModel(chatroomId);
            yield return TestCommons.GetMessageViewModel(chatroomId);
            yield return TestCommons.GetMessageViewModel(chatroomId);
        }
    }

    internal class TestAsyncQueryProvider<TEntity> : Microsoft.EntityFrameworkCore.Query.Internal.IAsyncQueryProvider
    {
        private readonly IQueryProvider _inner;

        internal TestAsyncQueryProvider(IQueryProvider inner)
        {
            _inner = inner;
        }

        public IQueryable CreateQuery(Expression expression)
        {
            return new TestAsyncEnumerable<TEntity>(expression);
        }

        public IQueryable<TElement> CreateQuery<TElement>(Expression expression)
        {
            return new TestAsyncEnumerable<TElement>(expression);
        }

        public object Execute(Expression expression)
        {
            return _inner.Execute(expression);
        }

        public TResult Execute<TResult>(Expression expression)
        {
            return _inner.Execute<TResult>(expression);
        }

        public IAsyncEnumerable<TResult> ExecuteAsync<TResult>(Expression expression)
        {
            return new TestAsyncEnumerable<TResult>(expression);
        }

        public Task<TResult> ExecuteAsync<TResult>(Expression expression, CancellationToken cancellationToken)
        {
            return Task.FromResult(Execute<TResult>(expression));
        }
    }

    internal class TestAsyncEnumerator<T> : IAsyncEnumerator<T>
    {
        private readonly IEnumerator<T> _inner;

        public TestAsyncEnumerator(IEnumerator<T> inner)
        {
            _inner = inner;
        }

        public void Dispose()
        {
            _inner.Dispose();
        }

        public T Current
        {
            get
            {
                return _inner.Current;
            }
        }

        public Task<bool> MoveNext(CancellationToken cancellationToken)
        {
            return Task.FromResult(_inner.MoveNext());
        }
    }

    internal class TestAsyncEnumerable<T> : EnumerableQuery<T>, IAsyncEnumerable<T>, IQueryable<T>
    {
        public TestAsyncEnumerable(IEnumerable<T> enumerable)
            : base(enumerable)
        { }

        public TestAsyncEnumerable(Expression expression)
            : base(expression)
        { }

        public IAsyncEnumerator<T> GetEnumerator()
        {
            return new TestAsyncEnumerator<T>(this.AsEnumerable().GetEnumerator());
        }

        IQueryProvider IQueryable.Provider
        {
            get { return new TestAsyncQueryProvider<T>(this); }
        }
    }
}
