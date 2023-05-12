using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Jobsity.Chatroom.Models;
using Microsoft.AspNetCore.Authorization;
using Jobsity.Chatroom.Services.Interfaces;

namespace Jobsity.Chatroom.Controllers
{
    public class HomeController : Controller
    {
        private readonly IMessageService service;
        public HomeController(IMessageService service)
        {
            this.service = service;
        }
        [Authorize]
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> SendMessage(string message, int chatId)
        {
            try
            {
                await service.SendMessage(new MessageViewModel
                {
                    Content = message,
                    TimeStamp = DateTime.Now,
                    UserName = User.Identity.Name,
                    ChatroomId = chatId
                });
                return Ok();
            }
            catch
            {
                return BadRequest();
            }
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> PublishBotMessage(MessageViewModel message)
        {
            try
            {
                await service.SendMessage(message);
                return Ok();
            }
            catch
            {
                return BadRequest();
            }
        }

        /// <summary>
        /// Retrieves messages from database and returns partial chatbox
        /// </summary>
        /// <param name="chatId"></param>
        /// <returns></returns>
        [Authorize]
        [HttpGet]
        public async Task<IActionResult> RetrieveMessage(int chatId)
        {
            try
            {
                var messageList = await service.RetrieveMessages(chatId);
                return PartialView("_ChatBox", messageList);
            }
            catch (Exception ex)
            {

                return BadRequest(ex.Message);
            }
        }
    }
}
