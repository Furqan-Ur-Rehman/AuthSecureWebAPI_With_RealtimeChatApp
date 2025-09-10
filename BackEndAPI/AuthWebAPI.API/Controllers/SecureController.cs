using AuthWebAPI.API.Chathubs;
using AuthWebAPI.Application.Interfaces;
using AuthWebAPI.Domain.Entities;
using AuthWebAPI.Domain.UserDto;
using AuthWebAPI.Persistance.Context;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;

namespace AuthWebAPI.API.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class SecureController(IHubContext<ChatHub> _hubContext,  IChatService _chatService) : ControllerBase
    {
        //WebAPIDbContext context,

        [HttpGet("data")]
        public IActionResult GetSecureData()
        {
            return Ok("Protected data accessed.");
        }

        [HttpPost("chat")]
        public async Task<IActionResult> SendMessage([FromBody] ChatDto msg)
        {
            try
            {
                
                // Save in DB
                await _chatService.SaveMessageAsync(msg.SenderEmail, msg.ReceiverEmail, msg.Chats);

                // 2️⃣ Send to receiver if online
                //if (ChatHub.UserConnections.TryGetValue(msg.ReceiverEmail, out var connectionId))
                //{
                //}
                    await _hubContext.Clients.Client(msg.ReceiverEmail)
                        .SendAsync("ReceiveMessage", msg.SenderEmail, msg.ReceiverEmail, msg.Chats);

                return Ok(new { status = "Message sent", msg.SenderEmail, msg.ReceiverEmail, msg.Chats });
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        

        //[HttpPost("create-group")]
        //public async Task<IActionResult> CreateGroup([FromBody] string groupName)
        //{
        //    var group = new ChatGroup { Name = groupName };
        //    context.ChatGroups.Add(group);
        //    await context.SaveChangesAsync();
        //    return Ok(group);
        //}

        //[HttpGet("messages/private/{fromId}/{toId}")]
        //public async Task<IActionResult> GetPrivateMessages(Guid fromId, Guid toId)
        //{
        //    var messages = await context.Chats
        //        .Where(m => (m.SenderId == fromId && m.ReceiverId == toId) ||
        //                    (m.SenderId == toId && m.ReceiverId == fromId))
        //        .OrderBy(m => m.SendTime)
        //        .ToListAsync();

        //    return Ok(messages);
        //}

        //[HttpGet("messages/group/{groupId}")]
        //public async Task<IActionResult> GetGroupMessages(Guid groupId)
        //{
        //    var messages = await context.Chats
        //        .Where(m => m.GroupId == groupId)
        //        .OrderBy(m => m.SendTime)
        //        .ToListAsync();

        //    return Ok(messages);
        //}

        //[HttpGet("ReceiveChats/{SenderEmail}/{ReceiverEmail}/{Chats}")]
        //public ActionResult<ChatDto> ReceiveMessage(string SenderEmail, string ReceiverEmail, string Chats)
        //{
        //    var chatdto = new ChatDto
        //    {
        //        SenderEmail = SenderEmail,
        //        ReceiverEmail = ReceiverEmail,
        //        Chats = Chats,
        //    };
        //    return Ok(chatdto);
            //var senderMsg = hubContext.Clients.;
            //if (senderId.Equals(Guid.Empty)) return;

            //var chatMessage = new ChatData
            //{
            //    SenderId = senderId,
            //    ReceiverId = ReceiverId,
            //    Chats = message
            //};

            //context.Chats.Add(chatMessage);
            //await context.SaveChangesAsync();

            //var Receiver_Connection = _connections.FirstOrDefault(c => c.Value == ReceiverId).Key;
            //if (!string.IsNullOrEmpty(Receiver_Connection))
            //{
            //    await Clients.Client(Receiver_Connection).SendAsync("ReceivePrivateMessage", senderId, message);
            //}
            //await Clients.Caller.SendAsync("PrivateMessageSent", ReceiverId, message);
        //}
    }
}
