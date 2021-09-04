using AutoMapper;
using FinancialChatApi.Dtos.Messages;
using FinancialChatApi.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FinancialChatApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class MessagesController : ControllerBase
    {
        private readonly IMessageService _messageService;
        private readonly IMapper _mapper;

        public MessagesController(IMessageService messageService, IMapper mapper)
        {
            _messageService = messageService;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var messages = await _messageService.GetLatestMessagesAsync();

            return Ok(_mapper.Map<IEnumerable<MessageDto>>(messages));
        }

        [HttpDelete]
        public async Task<IActionResult> DeleteAll()
        {
            await _messageService.DeleteAllAsync();

            return NoContent();
        }
    }
}
