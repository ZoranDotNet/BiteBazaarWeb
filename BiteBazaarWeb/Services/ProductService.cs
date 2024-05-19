using BiteBazaarWeb.Models;
using BiteBazaarWeb.ViewModels;
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


        //Get All Products
        public async Task<List<Product>> GetProductsAsync()
        {
            try
            {
                var response = await _client.GetAsync("products");
                if (!response.IsSuccessStatusCode)
                {
                    Console.WriteLine($"API Error: {response.StatusCode}");
                    return new List<Product>();
                }
                var jsonstring = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"JSON Response: {jsonstring}");
                var products = JsonConvert.DeserializeObject<List<Product>>(jsonstring);
                return products;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception: {ex.Message}");
                return new List<Product>();
            }

        }


        //Create Products
        public async Task AddProductAsync(PostProductVM product)
        {
            
            var json = JsonConvert.SerializeObject(product);
            Console.WriteLine($"Product: {json}");
            var data = new StringContent(json, Encoding.UTF8, "application/json");
            var response = await _client.PostAsync("products", data);
            Console.WriteLine($"API response: {response.StatusCode}");
            response.EnsureSuccessStatusCode();
        }


        //GetById
        public async Task<Product> GetProductByIdAsync(int id)
        {

            var response = await _client.GetAsync($"products/{id}");
            if (response.IsSuccessStatusCode)
            {
                Console.WriteLine($"API PRODUCT via ID: {response.StatusCode}");
                return await response.Content.ReadFromJsonAsync<Product>();
            }
            else
            {
                //throw new InvalidOperationException($"Api failed with statuscode {response.StatusCode}");
                return null;
            }

        }


        //Update Products
        public async Task UpdateProductAsync(int id, Product product)
        {
            var json = JsonConvert.SerializeObject(product);
            var data = new StringContent(json, Encoding.UTF8, "application/json");
            var response = await _client.PutAsync($"products/{id}", data);

            response.EnsureSuccessStatusCode();
        }




        //Delete Products
        public async Task DeleteProductAsync(int id)
        {
            var response = await _client.DeleteAsync($"products/{id}");
            response.EnsureSuccessStatusCode();
        }




    }
}
