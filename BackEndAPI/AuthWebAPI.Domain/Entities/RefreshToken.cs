using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuthWebAPI.Domain.Entities
{
    public class RefreshToken
    {
        public Guid Id { get; set; }
        public string TokenHash { get; set; } = null!;
        public DateTime CreatedAtUtc { get; set; }
        public DateTime ExpiresAtUtc { get; set; }
        public string CreatedByIp { get; set; } = null!;
        public string? ReplacedByTokenHash { get; set; }
        public bool Revoked { get; set; }
        public DateTime? RevokedAtUtc { get; set; }
        public string? RevokedByIp { get; set; }

        // Relationship
        public Guid UserId { get; set; }
        [ForeignKey("UserId")]

        //public string Email { get; set; } = null!;

        //public string Roles { get; set; } = null!;
        public User User { get; set; } = null!;
    }
}
