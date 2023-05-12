using Jobsity.Chatroom.Models;
using Jobsity.Chatroom.Services.Interfaces;
using Microsoft.AspNetCore.SignalR;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Jobsity.Chatroom.Hubs
{
    public class ChatroomHub : Hub
    {
        //public async Task SendMessage()
        //{
        //    await Clients.All.SendAsync("ReceiveMessage");
        //}
    }
}
