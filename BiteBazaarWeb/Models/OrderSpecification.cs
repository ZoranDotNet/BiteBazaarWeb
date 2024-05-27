using System.ComponentModel.DataAnnotations.Schema;

namespace BiteBazaarWeb.Models
{
    public class OrderSpecification
    {
        public int OrderSpecificationId { get; set; }

        [ForeignKey("Order")]
        public int FkOrderId { get; set; }
        public Order? Order { get; set; }

        [Column(TypeName = "decimal(10,2)")]
        public decimal PayedPrice { get; set; }
        public int Count { get; set; }
        public int FkProductId { get; set; }

        [NotMapped]
        public Product? Product { get; set; }
    }
}
