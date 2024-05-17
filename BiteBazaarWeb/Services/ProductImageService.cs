using BiteBazaarWeb.Models;
using Newtonsoft.Json;
using System.Text;

namespace BiteBazaarWeb.Services
{
    public class ProductImageService
    {
        private readonly HttpClient _client;

        public ProductImageService(IHttpClientFactory clientFactory)
        {
            _client = clientFactory.CreateClient("API Client");
        }
        // Get All Images
        public async Task<List<ProductImage>> GetProductImagesAsync()
        {
            try
            {
                var response = await _client.GetAsync("/products/images");
                if (!response.IsSuccessStatusCode)
                {
                    return new List<ProductImage>();
                }
                var jsonstring = await response.Content.ReadAsStringAsync();
                var products = JsonConvert.DeserializeObject<List<ProductImage>>(jsonstring);
                return products;
            }
            catch (Exception ex)
            {
                return new List<ProductImage>();
            }

        }
        // Create
        public async Task AddProductImageAsync(ProductImage image)
        {
            var json = JsonConvert.SerializeObject(image);
            var data = new StringContent(json, Encoding.UTF8, "application/json");
            var response = await _client.PostAsync("/products/images", data);
            response.EnsureSuccessStatusCode();
        }
        // GetById
        public async Task<ProductImage> GetProductImageByIdAsync(int id)
        {

            var response = await _client.GetAsync($"/products/images/{id}");
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<ProductImage>();
            }
            else
            {
                //throw new InvalidOperationException($"Api failed with statuscode {response.StatusCode}");
                return null;
            }

        }

        // Delete Image
        public async Task DeleteProductImageAsync(int id)
        {
            var response = await _client.DeleteAsync($"/products/images/{id}");
            response.EnsureSuccessStatusCode();
        }
    }
}
