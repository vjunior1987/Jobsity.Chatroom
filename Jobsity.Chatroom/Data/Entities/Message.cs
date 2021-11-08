using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Jobsity.Chatroom.Data.Entities
{
    public class Message
    {
        [Key]
        public Guid Id { get; set; }
        public string Content { get; set; }
        public DateTime TimeStamp { get; set; }
        public string UserName { get; set; }
        public int ChatroomId { get; set; }
    }
}
