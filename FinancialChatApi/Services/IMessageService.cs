using FinancialChatApi.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FinancialChatApi.Services
{
    public interface IMessageService
    {
        Task CreateMessageAsync(Message message);
        Task<IEnumerable<Message>> GetLatestMessagesAsync(int count = 50);
        Task DeleteAllAsync();
    }
}