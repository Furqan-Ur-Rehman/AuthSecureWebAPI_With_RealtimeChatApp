using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuthWebAPI.Domain.UserDto
{
    public class UserDto
    {
        [Required]
        [StringLength(50, ErrorMessage = "Name should be or less than 50 Characters.")]
        public string Username { get; set; } = default!;

        [Required]
        [EmailAddress]
        public string Email { get; set; } = null!;
        [Required]
        [StringLength(255, ErrorMessage = "Password should be or less than 255 Characters.")]
        [DataType(DataType.Password)]
        public string Password { get; set; } = default!;

    }
}
