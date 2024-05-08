using BiteBazaarWeb.Models;
using BiteBazaarWeb.Services;
using Microsoft.AspNetCore.Mvc;

namespace BiteBazaarWeb.Controllers
{
    public class CategoriesController : Controller
    {
        private readonly CategoryService _apiService;

        public CategoriesController(CategoryService apiService)
        {
            _apiService = apiService;
        }

        public async Task<IActionResult> Index()
        {
            var categories = await _apiService.GetCategoriesAsync();
            if (categories == null || !categories.Any())
            {
                return View(new List<Category>());
            }

            return View(categories);
        }

        public async Task<IActionResult> Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Category category)
        {
            if (ModelState.IsValid)
            {
                await _apiService.AddCategoryAsync(category);
                return RedirectToAction(nameof(Index));
            }

            return View(category);
        }

        public IActionResult GetById()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> GetById(int id)
        {
            var category = await _apiService.GetCategoryByIdAsync(id);
            if (category == null)
            {
                return View("ByIdResult");
            }
            return View("ByIdResult", category);
        }

        public async Task<IActionResult> Edit(int id)
        {
            var category = await _apiService.GetCategoryByIdAsync(id);

            return View(category);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(int id, Category category)
        {
            if (ModelState.IsValid)
            {
                await _apiService.UpdateCategoryAsync(id, category);
                return RedirectToAction(nameof(Index));
            }

            return View(category);
        }


        [HttpPost]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await _apiService.DeleteCategoryAsync(id);
            return RedirectToAction(nameof(Index));
        }


    }
}
