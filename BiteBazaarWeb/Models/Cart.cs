using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations.Schema;

namespace BiteBazaarWeb.Models
{
    public class Cart
    {
        public int CartId { get; set; }
        // Products är 1-to-many, man lägger till en produkt i taget. varje gång blir det en ny rad i DB. 
        [ForeignKey("Product")]
        public int FkProductId { get; set; }
        public Product Product { get; set; }
        public int Count { get; set; }

        [ForeignKey("ApplicationUser")]
        public string FkApplicationUserId { get; set; }
        [ValidateNever]
        public ApplicationUser ApplicationUser { get; set; }


    }
}
