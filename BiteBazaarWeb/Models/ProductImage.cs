using System.ComponentModel.DataAnnotations.Schema;

namespace BiteBazaarWeb.Models
{
    public class ProductImage
    {
        public int ProductImageId { get; set; }
        public string URL { get; set; }

        [ForeignKey("Product")]
        public int FkProductId { get; set; }
        public Product? Product { get; set; }
    }
}
