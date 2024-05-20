using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BiteBazaarWeb.Models
{
    public class ProductImage
    {
        public int ProductImageId { get; set; }

        [StringLength(250)]
        public string URL { get; set; }

        //[ForeignKey("Product")]
        public int FkProductId { get; set; }
        public Product? Product { get; set; }


    }
}
