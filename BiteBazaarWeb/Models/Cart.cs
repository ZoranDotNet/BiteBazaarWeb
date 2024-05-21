using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations.Schema;

namespace BiteBazaarWeb.Models
{
    public class Cart
    {
        public int CartId { get; set; }
        public int FkProductId { get; set; }
        [NotMapped]
        public Product? Product { get; set; }
        public int Count { get; set; }

        [ForeignKey("ApplicationUser")]
        public string FkApplicationUserId { get; set; }
        [ValidateNever]
        public ApplicationUser ApplicationUser { get; set; }


    }
}
