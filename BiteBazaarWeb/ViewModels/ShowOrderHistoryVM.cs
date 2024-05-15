using BiteBazaarWeb.Models;

namespace BiteBazaarWeb.ViewModels
{
    public class ShowOrderHistoryVM
    {

        public List<Order> OrderList { get; set; }

        public List<List<OrderSpecification>> SpecificationList { get; set; }

    }
}
