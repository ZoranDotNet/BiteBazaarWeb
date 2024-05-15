using BiteBazaarWeb.Models;

namespace BiteBazaarWeb.ViewModels
{
    public class OrderHistoryVM
    {
        public List<OrderSpecification> Specifications { get; set; }
        public Order Order { get; set; }
    }
}
