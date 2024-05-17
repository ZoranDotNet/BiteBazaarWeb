using System.ComponentModel.DataAnnotations;

namespace BiteBazaarWeb.Models
{
    public class Category
    {
        [Key]
        public int CategoryId { get; set; }

        [StringLength(50)]
        public string Title { get; set; } = null!;
        [StringLength(250)]
        public string Description { get; set; }

    }
}
