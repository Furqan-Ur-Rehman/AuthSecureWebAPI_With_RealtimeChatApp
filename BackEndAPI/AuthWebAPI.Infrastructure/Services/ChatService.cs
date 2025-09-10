using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AuthWebAPI.Application.Interfaces;
using AuthWebAPI.Domain.Entities;
using AuthWebAPI.Persistance.Context;
using Microsoft.EntityFrameworkCore;

namespace AuthWebAPI.Infrastructure.Services
{
    public class ChatService(WebAPIDbContext _context) : IChatService
    {
        public async Task SaveMessageAsync(string senderEmail, string receiverEmail, string message)
        {
            var senderId = await _context.Users
                .Where(u => u.Email == senderEmail)
                .Select(u => u.Id)
                .FirstOrDefaultAsync();

            var receiverId = await _context.Users
                .Where(u => u.Email == receiverEmail)
                .Select(u => u.Id)
                .FirstOrDefaultAsync();

            if (senderId.Equals(Guid.Empty) || receiverId.Equals(Guid.Empty))
                throw new Exception("Sender or receiver not found");

            var chatMessage = new ChatData
            {
                SenderId = senderId,
                SenderEmail = senderEmail,
                ReceiverId = receiverId,
                ReceiverEmail = receiverEmail,
                Chats = message,
                SendTime = DateTime.UtcNow
            };

            _context.Chats.Add(chatMessage);
            await _context.SaveChangesAsync();
        }
    }
}
