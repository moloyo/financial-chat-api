using FinancialChatApi.Dtos.Messages;
using System.Threading.Tasks;

namespace FinancialChatApi.Messsaging
{
    public interface IHubClient
    {
        Task BroadcastMessage(MessageDto message);

        Task SendMessage(MessageDto message);
    }
}
