using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuthWebAPI.Domain.Entities
{
    public class ChatGroup
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;

        public ICollection<GroupMembership> Members { get; set; } = null!;
    }
}
