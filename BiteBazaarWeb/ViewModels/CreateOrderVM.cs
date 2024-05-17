using BiteBazaarWeb.Models;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace BiteBazaarWeb.ViewModels
{
    public class CreateOrderVM
    {
        public string ApplicationUserId { get; set; }
        [ValidateNever]
        public ApplicationUser? ApplicationUser { get; set; }
        public List<Cart> Carts { get; set; }

        public decimal OrderTotal { get; set; }

        public bool Terms { get; set; } = false;
    }
}
