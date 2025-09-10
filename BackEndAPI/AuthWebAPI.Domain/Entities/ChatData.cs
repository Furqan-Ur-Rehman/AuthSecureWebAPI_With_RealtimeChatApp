using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuthWebAPI.Domain.Entities
{
    public class ChatData
    {
        [Key]
        public Guid ChatID { get; set; }

        public Guid SenderId { get; set; }
        [Required]
        [EmailAddress]
        //public User? SenderEmail { get; set; }
        public string SenderEmail { get; set; } = string.Empty;

        public Guid ReceiverId { get; set; }
        [Required]
        [EmailAddress]
        public string ReceiverEmail { get; set; } = string.Empty;

        public Guid? GroupId { get; set; } // For group message
        public ChatGroup? Group { get; set; }

        [StringLength(500)]
        public string Chats { get; set; } = string.Empty;

        public DateTime SendTime { get; set; } = default!;
    }
}
