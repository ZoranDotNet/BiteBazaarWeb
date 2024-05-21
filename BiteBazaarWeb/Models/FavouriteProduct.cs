using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace BiteBazaarWeb.Models
{
    public class FavouriteProduct
    {
        [Key]
        public int FavouriteProductId { get; set; }
        public int FkProductId { get; set; }
        [NotMapped]
        public Product? Product { get; set; }
        public string FkApplicationUserId { get; set; }
        [ValidateNever]
        [NotMapped]
        public ApplicationUser ApplicationUser { get; set; }
    }
}
