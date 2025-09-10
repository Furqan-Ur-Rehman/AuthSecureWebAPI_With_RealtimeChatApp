using System.Data.Common;
using System.Text.RegularExpressions;
using AuthWebAPI.Application.Interfaces;
using AuthWebAPI.Domain.Entities;
using AuthWebAPI.Domain.UserDto;
using AuthWebAPI.Persistance.Context;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace AuthWebAPI.API.Chathubs
{
    [Authorize]
    public class ChatHub : Hub
    {
        private readonly WebAPIDbContext _db;
        private readonly IChatService _chatService;
        public ChatHub(WebAPIDbContext db, IChatService chatService)
        {
            _db = db;
            _chatService = chatService;
        }

        public static readonly Dictionary<string, string> UserConnections = []; // email -> connectionId
        private static readonly Lock _lock = new();
        public override Task OnConnectedAsync()
        {
            var httpContext = Context.GetHttpContext();
            var email = httpContext!.Request.Query["email"].ToString();
            if (!string.IsNullOrEmpty(email))
            {
                lock (_lock)
                {
                    UserConnections[email!] = Context.ConnectionId;
                }
                Console.WriteLine($"User {email} connected with {Context.ConnectionId}");
            }
            return base.OnConnectedAsync();
        }

        public override Task OnDisconnectedAsync(Exception? exception)
        {
            var email = UserConnections.FirstOrDefault(x => x.Value == Context.ConnectionId).Key;
            if (email != null)
            {
                lock (_lock)
                {
                    UserConnections.Remove(email);
                }
                Console.WriteLine($"User {email} disconnected.");
            }
            return base.OnDisconnectedAsync(exception);
        }

        // Send a message to specific user
        public async Task SendMessage(string SenderEmail, string ReceiverEmail, string Chats)
        {
            if (UserConnections.TryGetValue(ReceiverEmail, out var connectionId))
            {
                await _chatService.SaveMessageAsync(SenderEmail, ReceiverEmail, Chats);
                var Current_DateTime1 = DateTime.UtcNow;
                await Clients.Client(connectionId).SendAsync("ReceiveMessage", Chats, SenderEmail, ReceiverEmail, Current_DateTime1);
            }

            // Optionally send to the sender as well (so they see their own message instantly)
            var Current_Datetime2 = DateTime.UtcNow;
            await Clients.Caller
                .SendAsync("ReceiveMessage", Chats, SenderEmail, ReceiverEmail, Current_Datetime2);
        }

        //public async Task SendMessage(string SenderEmail, string ReceiverEmail, string Chats)
        //{
        //    await _chatService.SaveMessageAsync(SenderEmail, ReceiverEmail, Chats);
        //    var Current_DateTime = DateTime.UtcNow;
        //    await Clients.All.SendAsync("ReceiveMessage", Chats, SenderEmail, ReceiverEmail, Current_DateTime);
        //}


        

    } 
    
}

