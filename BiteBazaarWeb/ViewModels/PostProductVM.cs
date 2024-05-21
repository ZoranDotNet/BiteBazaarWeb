namespace BiteBazaarWeb.ViewModels
{
    public class PostProductVM
    {
        public string Title { get; set; }

        public string Description { get; set; }

        public decimal Price { get; set; }

        public int FkCategoryId { get; set; }

        public int Quantity { get; set; }
        public string ImageURL { get; set; }
    }
}
