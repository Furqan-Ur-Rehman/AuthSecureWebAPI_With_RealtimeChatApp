using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace AuthWebAPI.Domain.Entities
{
    public class User
    {
        [Key]
        public Guid Id { get; set; }

        [Required]
        [StringLength(50, ErrorMessage = "Name should be or less than 50 Characters.")]
        public string Username { get; set; } = default!;
        [Required]
        [EmailAddress]
        public string Email { get; set; } = null!;
        [Required]
        [StringLength(255, ErrorMessage = "Password should be or less than 255 Characters.")]
        [DataType(DataType.Password)]
        public string PasswordHash { get; set; } = default!;

        public string Roles { get; set; } = "User";
        public string RefreshToken { get; set; } = string.Empty;
        public DateTime RefreshTokenExpiry { get; set; } = default!;
    }
}
