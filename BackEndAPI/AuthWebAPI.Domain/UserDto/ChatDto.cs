using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuthWebAPI.Domain.UserDto
{
    public class ChatDto
    {
        [Required]
        [EmailAddress]
        public string SenderEmail { get; set; } = null!;

        public Guid ReceiverId { get; set; }

        [Required]
        [EmailAddress]
        public string ReceiverEmail { get; set; } = null!;

        [StringLength(500)]
        public string Chats { get; set; } = string.Empty;

        //public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    }
}
