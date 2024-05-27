using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BiteBazaarWeb.Models
{
    public class Order
    {
        public int OrderId { get; set; }
        public DateTime OrderDate { get; set; } = DateTime.Now;
        public DateTime ShippingDate { get; set; }

        [Column(TypeName = "decimal(10,2)")]
        public decimal OrderTotal { get; set; }

        public OrderStatus Status { get; set; } = OrderStatus.Skapad;

        public string FkApplicationUserId { get; set; }
        public ApplicationUser? ApplicationUser { get; set; }

        [Required]
        [StringLength(100)]
        public string Name { get; set; }

        [Required]
        [StringLength(100)]
        public string StreetAddress { get; set; }

        [Required]
        [StringLength(20)]
        public string ZipCode { get; set; }

        [Required]
        [StringLength(100)]
        public string City { get; set; }
        public ICollection<OrderSpecification>? OrderSpecifications { get; set; }

    }

    public enum OrderStatus
    {
        Skapad,
        Skickad
    }
}
