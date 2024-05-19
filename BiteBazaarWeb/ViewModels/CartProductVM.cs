using BiteBazaarWeb.Models;

namespace BiteBazaarWeb.ViewModels
{
    public class CartProductVM
    {
        public int CartId { get; set; }
        public Product Product { get; set; }
        public int Count { get; set; }
    }
}
