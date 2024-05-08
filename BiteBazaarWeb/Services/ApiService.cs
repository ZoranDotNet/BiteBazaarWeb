using BiteBazaarWeb.Models;
using Newtonsoft.Json;
using System.Text;

namespace BiteBazaarWeb.Services
{
    public class ApiService
    {
        private readonly HttpClient _client;

        public ApiService(IHttpClientFactory clientFactory)
        {
            _client = clientFactory.CreateClient("API Client");
        }


        //Get All Categories
        public async Task<List<Category>> GetCategoriesAsync()
        {
            try
            {
                var response = await _client.GetAsync("categories");
                if (!response.IsSuccessStatusCode)
                {
                    return new List<Category>();
                }
                var jsonstring = await response.Content.ReadAsStringAsync();
                var categories = JsonConvert.DeserializeObject<List<Category>>(jsonstring);
                return categories;
            }
            catch (Exception ex)
            {
                return new List<Category>();
            }

        }


        //Create
        public async Task AddCategoryAsync(Category category)
        {
            var json = JsonConvert.SerializeObject(category);
            var data = new StringContent(json, Encoding.UTF8, "application/json");
            var response = await _client.PostAsync("categories", data);
            response.EnsureSuccessStatusCode();
        }


        //GetById
        public async Task<Category> GetCategoryByIdAsync(int id)
        {

            var response = await _client.GetAsync($"categories/{id}");
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<Category>();
            }
            else
            {
                //throw new InvalidOperationException($"Api failed with statuscode {response.StatusCode}");
                return null;
            }

        }


        //Update Category
        public async Task UpdateCategoryAsync(int id, Category category)
        {
            var json = JsonConvert.SerializeObject(category);
            var data = new StringContent(json, Encoding.UTF8, "application/json");
            var response = await _client.PutAsync($"categories/{id}", data);

            response.EnsureSuccessStatusCode();
        }




        //Delete Category
        public async Task DeleteCategoryAsync(int id)
        {
            var response = await _client.DeleteAsync($"categories/{id}");
            response.EnsureSuccessStatusCode();
        }




    }
}
