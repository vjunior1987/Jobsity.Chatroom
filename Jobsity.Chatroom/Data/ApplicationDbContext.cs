using Jobsity.Chatroom.Data.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Jobsity.Chatroom.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public DbSet<Message> Message { get; set; }
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }
    }
}
