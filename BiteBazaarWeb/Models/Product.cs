using System.ComponentModel.DataAnnotations.Schema;

namespace BiteBazaarWeb.Models
{
    public class Product
    {
        public int ProductId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }

        [ForeignKey("Category")]
        public int FkCategoryId { get; set; }
        public Category? Category { get; set; }

        public ICollection<ProductImage>? ProductImages { get; set; }

    }
}
