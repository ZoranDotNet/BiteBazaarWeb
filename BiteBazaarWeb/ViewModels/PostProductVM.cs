using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace BiteBazaarWeb.ViewModels
{
    public class PostProductVM
    {
        public string Title { get; set; }

        public string Description { get; set; }

        public decimal Price { get; set; }

        public int FkCategoryId { get; set; }

        public int Quantity { get; set; }

        public bool IsCampaign { get; set; } = false;
        [ValidateNever]
        public decimal? CampaignPercent { get; set; } = 1;
        [ValidateNever]
        public decimal? TempPrice { get; set; }
        public DateTime? CampaignStart { get; set; }
        public DateTime? CampaignEnd { get; set; }

        public string ImageURL { get; set; }
    }
}
