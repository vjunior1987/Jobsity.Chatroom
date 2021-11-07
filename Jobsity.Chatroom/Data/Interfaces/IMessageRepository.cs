using Jobsity.Chatroom.Data.Entities;
using System.Linq;
using System.Threading.Tasks;

namespace Jobsity.Chatroom.Data.Interfaces
{
    public interface IMessageRepository
    {
        IQueryable<Message> RetrieveMessages();
        Task SetMessage(Message entity);
    }
}
