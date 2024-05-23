using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BiteBazaarWeb.Models
{
    public class ApplicationUser : IdentityUser
    {
        [Required]
        [StringLength(100)]
        [PersonalData]
        public string Name { get; set; }

        [StringLength(100)]
        [PersonalData]
        public string StreetAddress { get; set; }

        [Column(TypeName = "varchar(10)")]
        [PersonalData]
        public string ZipCode { get; set; }

        [StringLength(50)]
        [PersonalData]
        public string City { get; set; }

        [NotMapped]
        public string Role { get; set; }
    }
}
