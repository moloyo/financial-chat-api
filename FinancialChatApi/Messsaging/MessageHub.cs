using AutoMapper;
using FinancialChatApi.Dtos.Messages;
using FinancialChatApi.Models;
using FinancialChatApi.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Configuration;
using System.Threading.Tasks;

namespace FinancialChatApi.Messsaging
{
    public class MessageHub : Hub
    {
        private readonly IDataProtectionProvider _dataProtectionProvider;
        private readonly IConfiguration _configuration;
        private readonly IMessageService _messageService;
        private readonly UserManager<Account> _userManager;
        private readonly IMapper _mapper;

        public MessageHub(
            IMessageService messageService,
            UserManager<Account> userManager,
            IMapper mapper,
            IDataProtectionProvider dataProtectionProvider,
            IConfiguration configuration)
        {
            _messageService = messageService;
            _userManager = userManager;
            _mapper = mapper;
            _dataProtectionProvider = dataProtectionProvider;
            _configuration = configuration;
        }

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task Message(string content)
        {
            var user = await _userManager.FindByNameAsync(Context.User.Identity.Name);

            var message = await SaveMessage(content, user);

            await BroadcastMessage(message);
        }

        private async Task BroadcastMessage(Message message)
        {
            var messageDto = _mapper.Map<MessageDto>(message);

            if (messageDto.Content.StartsWith("/"))
            {
                await Clients.All.SendAsync("MessageBot", messageDto);
            }

            await Clients.All.SendAsync("BroadcastMessage", messageDto);
        }

        private async Task<Message> SaveMessage(string content, Account user)
        {
            var message = new Message(Encrypt(content), user);
            await _messageService.CreateMessageAsync(message);
            return message;
        }

        private string Encrypt(string input)
        {
            var protector = _dataProtectionProvider.CreateProtector(_configuration["EncryptionKey"]);
            return protector.Protect(input);
        }
    }
}
