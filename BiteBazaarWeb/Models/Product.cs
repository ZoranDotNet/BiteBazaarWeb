using System.ComponentModel.DataAnnotations;

namespace BiteBazaarWeb.Models
{
    public class Product
    {
        public int ProductId { get; set; }

        [StringLength(30)]
        public string Title { get; set; }

        [StringLength(250)]
        public string Description { get; set; }

        public int Quantity { get; set; }

        public decimal Price { get; set; }

        public int FkCategoryId { get; set; }

        public Category? Category { get; set; }
        public ICollection<ProductImage>? Images { get; set; }
        public bool IsCampaign { get; set; } = false;

        public decimal CampaignPercent { get; set; } = 1;

        public decimal TempPrice { get; set; } = 0;
        public DateTime? CampaignStart { get; set; }
        public DateTime? CampaignEnd { get; set; }
    }
}
