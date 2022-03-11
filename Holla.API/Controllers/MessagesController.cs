using Holla.API.Extensions;
using Holla.BLL.DTOs;
using Holla.BLL.Features;
using Holla.BLL.Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Holla.API.Controllers
{
    [Authorize]
    public class MessagesController : BaseApiController
    {
        private readonly MessageManager _messageManager;

        public MessagesController(MessageManager messageManager)
        {
            _messageManager = messageManager;
        }

        [HttpPost]
        public async Task<ActionResult<MessageDto>> CreateMessage(CreateMessageDto createMessageDto)
        {
            var result = await _messageManager.CreateMessage(User.GetUsername(), createMessageDto);
            if (result == null)
            {
                Ok(result);
            }
            return BadRequest("Failed to send message");
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<MessageDto>>> GetMessagesForUser([FromQuery] MessageParams messageParams)
        {
            var messages = await _messageManager.GetMessagesForUser(User.GetUsername(), messageParams);
            Response.AddPaginationHeader(messages.CurrentPage, messages.PageSize, messages.TotalCount, messages.TotalPages);
            return messages;
        }

        [HttpGet("thread/{username}")]
        public async Task<ActionResult<IEnumerable<MessageDto>>> GetMessageThread(string username)
        {
            string currentUsername = User.GetUsername();
            return Ok(await _messageManager.GetMessageThread(currentUsername, username));
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteMessage(int id)
        {
            if (await _messageManager.DeleteMessage(User.GetUsername(), id))
            { 
                return Ok();
            }
            return BadRequest("Problem deleting the message");
        }

    }
}