using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AuthWebAPI.Domain.UserDto;

namespace AuthWebAPI.Application.Interfaces
{
    public interface IChatService
    {
        Task SaveMessageAsync(string SenderEmail, string ReceiverEmail, string Chats);
    }
}
