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


        //Create Products
        public async Task AddProductAsync(PostProductVM product)
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

        //Get Products By CategoryId
        public async Task<List<Product>> GetProductsByCategoryIdAsync(int id)
        {
            try
            {
                var response = await _client.GetAsync($"/category/{id}/products");
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

        //Get Products by search for title
        public async Task<List<Product>> GetProductsBySearchAsync(string search)
        {
            try
            {
                var response = await _client.GetAsync($"/products/search/{search}");
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


    }
}
