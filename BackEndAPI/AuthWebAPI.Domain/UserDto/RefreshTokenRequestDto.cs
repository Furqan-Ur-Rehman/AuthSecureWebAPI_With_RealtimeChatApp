using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuthWebAPI.Domain.UserDto
{
    public class RefreshTokenRequestDto
    {
        //public Guid Id { get; set; }
        [Required]
        [EmailAddress]
        public string Email { get; set; } = null!;
        //public string? RefreshToken { get; set; }

        //public string? HashToken { get; set; }
    }
}
