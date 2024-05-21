using System.ComponentModel.DataAnnotations;

namespace BiteBazaarWeb.Models
{
    public class ProductImage
    {
        public int ProductImageId { get; set; }

        [StringLength(250)]
        public string URL { get; set; }
        public int FkProductId { get; set; }
        public Product? Product { get; set; }


    }
}
