using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BiteBazaarWeb.Models
{
    public class ApplicationUser : IdentityUser
    {
        [Required]
        [StringLength(100)]
        public string Name { get; set; }

        [StringLength(100)]
        public string StreetAddress { get; set; }

        [Column(TypeName = "varchar(10)")]
        public string ZipCode { get; set; }

        [StringLength(50)]
        public string City { get; set; }

        [Column(TypeName = "varchar(15)")]
        public string PhoneNumber { get; set; }
    }
}
