using Microsoft.AspNetCore.Identity;
using System;

namespace FinancialChatApi.Models
{
    public class Account : IdentityUser<Guid>
    {
        public Account(string userName) : base(userName) { }
    }
}
