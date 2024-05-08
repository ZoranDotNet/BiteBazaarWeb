using BiteBazaarWeb.Models;
using Newtonsoft.Json;
using System.Text;

namespace BiteBazaarWeb.Services
{
    public class ProductService
    {
        private readonly HttpClient _client;

        public ProductService(IHttpClientFactory clientFactory)
        {
            _client = clientFactory.CreateClient("API Client");
        }


        //Get All Categories
        public async Task<List<Product>> GetProductsAsync()
        {
            try
            {
                var response = await _client.GetAsync("products");
                if (!response.IsSuccessStatusCode)
                {
                    return new List<Product>();
                }
                var jsonstring = await response.Content.ReadAsStringAsync();
                var products = JsonConvert.DeserializeObject<List<Product>>(jsonstring);
                return products;
            }
            catch (Exception ex)
            {
                return new List<Product>();
            }

        }


        //Create
        public async Task AddProductAsync(Product product)
        {
            var json = JsonConvert.SerializeObject(product);
            var data = new StringContent(json, Encoding.UTF8, "application/json");
            var response = await _client.PostAsync("products", data);
            response.EnsureSuccessStatusCode();
        }


        //GetById
        public async Task<Product> GetProductByIdAsync(int id)
        {

            var response = await _client.GetAsync($"products/{id}");
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<Product>();
            }
            else
            {
                //throw new InvalidOperationException($"Api failed with statuscode {response.StatusCode}");
                return null;
            }

        }


        //Update Category
        public async Task UpdateProductAsync(int id, Product product)
        {
            var json = JsonConvert.SerializeObject(product);
            var data = new StringContent(json, Encoding.UTF8, "application/json");
            var response = await _client.PutAsync($"products/{id}", data);

            response.EnsureSuccessStatusCode();
        }




        //Delete Category
        public async Task DeleteProductAsync(int id)
        {
            var response = await _client.DeleteAsync($"products/{id}");
            response.EnsureSuccessStatusCode();
        }




    }
}
