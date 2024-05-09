using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BiteBazaarWeb.Models
{
    public class Product
    {
        [Key]
        public int ProductId { get; set; }

        [StringLength(30)]
        public string Title { get; set; }

        [StringLength(250)]
        public string Description { get; set; }

        [Column(TypeName = "decimal(10,2)")]
        public decimal Price { get; set; }

        [ForeignKey("Category")]
        public int FkCategoryId { get; set; }

        public Category? Category { get; set; }
        public ICollection<ProductImage>? Images { get; set; }

    }
}
