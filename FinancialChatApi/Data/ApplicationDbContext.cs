using FinancialChatApi.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;

namespace FinancialChatApi.Data
{
    public class ApplicationDbContext : IdentityDbContext<Account, IdentityRole<Guid>, Guid>
    {
        protected ApplicationDbContext () { }

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Message> Messages { get; set; }
    }
}
