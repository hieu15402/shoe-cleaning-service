using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using TP4SCS.Library.Models.Request.Chat;
using TP4SCS.Services.Interfaces;

namespace TP4SCS.API.Controllers
{
    [Route("api/chats")]
    [ApiController]
    public class ChatController : ControllerBase
    {
        private readonly IChatService _chatService;

        public ChatController(IChatService chatService)
        {
            _chatService = chatService;
        }

        [HttpPost("create-room")]
        public async Task<IActionResult> CreateChatRoomAsync([FromBody] ChatRoomRequest room)
        {
            var result = await _chatService.CreateChatRoomAsync(room);

            if (result.StatusCode != 200)
            {
                return StatusCode(result.StatusCode, result);
            }

            return Ok(result);
        }

        [Authorize]
        [HttpPost("send-message")]
        public async Task<IActionResult> SendMessageAsync([FromBody] MessageRequest messageRequest)
        {
            string? userIdClaim = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            var userId = int.TryParse(userIdClaim, out int id);

            var result = await _chatService.SendMessageAsync(id, messageRequest);

            return StatusCode(result.StatusCode, result);
        }

        [Authorize]
        [HttpPost("send-order-message")]
        public async Task<IActionResult> SendOrderMessageAsync([FromBody] OrderMessageRequest orderMessageRequest)
        {
            string? userIdClaim = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            var userId = int.TryParse(userIdClaim, out int id);

            var result = await _chatService.SendOrderMessageAsync(id, orderMessageRequest);

            return StatusCode(result.StatusCode, result);
        }

        [Authorize]
        [HttpGet("get-messages/{roomId}")]
        public async Task<IActionResult> GetMessagesAsync([FromRoute] string roomId)
        {
            string? userIdClaim = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            var userId = int.TryParse(userIdClaim, out int id);

            var result = await _chatService.GetMessagesAsync(id, roomId);

            if (result.StatusCode != 200)
            {
                return StatusCode(result.StatusCode, result);
            }

            return Ok(result);
        }

        [HttpGet("get-rooms/{accId}")]
        public async Task<IActionResult> GetChatsRoomAsync([FromRoute] int accId)
        {
            var result = await _chatService.GetChatRoomsAsync(accId);

            if (result.StatusCode != 200)
            {
                return StatusCode(result.StatusCode, result);
            }

            return Ok(result);
        }

        [HttpDelete("{roomId}")]
        public async Task<IActionResult> DeleteChatRoomAsync([FromRoute] string roomId)
        {
            var result = await _chatService.DeleteChatRoomAsync(roomId);

            if (result.StatusCode != 200)
            {
                return StatusCode(result.StatusCode, result);
            }

            return Ok(result);
        }
    }
}
