using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuthWebAPI.Domain.Entities
{
    public class GroupMembership
    {
        public Guid Id { get; set; }

        public Guid UserId { get; set; }
        public User Email { get; set; } = null!;

        public Guid GroupId { get; set; }
        public ChatGroup Group { get; set; } = null!;
    }
}
