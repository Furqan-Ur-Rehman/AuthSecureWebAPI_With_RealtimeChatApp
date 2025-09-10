using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuthWebAPI.Domain.Entities
{
    public class Product
    {
        [Key]
        public int ProId { get; set; }

        [Required]
        [StringLength(30, ErrorMessage = "Product name should be or less than 30 Characters.")]
        public string ProductName { get; set; } = null!;

        [Required]
        [StringLength(40, ErrorMessage = "Item name should be or less than 30 Characters.")]
        public string ProductDescription { get; set; } = null!;

        [Required]
        public int ProductPrice { get; set; }
    }
}
