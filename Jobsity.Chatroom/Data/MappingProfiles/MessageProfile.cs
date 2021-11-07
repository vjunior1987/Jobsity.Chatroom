using AutoMapper;
using Jobsity.Chatroom.Data.Entities;
using Jobsity.Chatroom.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Jobsity.Chatroom.Data.MappingProfiles
{
    public class MessageProfile : Profile
    {
        /// <summary>
        /// Mapping Profile for converting DTO Message to ViewModel
        /// </summary>
        public MessageProfile()
        {
            CreateMap<Message, MessageViewModel>().ReverseMap();
        }
    }
}
