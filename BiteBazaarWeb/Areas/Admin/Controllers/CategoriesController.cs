using BiteBazaarWeb.Data;
using BiteBazaarWeb.Models;
using BiteBazaarWeb.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BiteBazaarWeb.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class CategoriesController : Controller
    {
        //private readonly AppDbContext _context;

        //public CategoriesController(AppDbContext context)
        //{
        //    _context = context;
        //}

        private readonly CategoryService _apiService;
        public CategoriesController(CategoryService apiService)
        {
            _apiService = apiService;
        }

        // GET: Categories
        public async Task<IActionResult> Index()
        {
            var categories = await _apiService.GetCategoriesAsync();

            if (categories == null || !categories.Any())
            {
                return View(new List<Category>());
            }

            return View(categories);
        }

        // GET: Categories/Details/5
        public async Task<IActionResult> Details(int id)
        {

            var category = await _apiService.GetCategoryByIdAsync(id);

            if (category == null)
            {
                return NotFound();
            }

            return View(category);
        }

        // GET: Categories/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Categories/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("CategoryId,Title")] Category category)
        {
            if (ModelState.IsValid)
            {
                await _apiService.AddCategoryAsync(category);
                TempData["success"] = "Ny Kategori tillagd";
                return RedirectToAction(nameof(Index));
            }
            return View(category);
        }

        // GET: Categories/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            var category = await _apiService.GetCategoryByIdAsync(id);
            if (category == null)
            {
                return NotFound();
            }
            return View(category);
        }

        // POST: Categories/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("CategoryId,Title")] Category category)
        {
            if (id != category.CategoryId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {

                //await _apiService.UpdateCategoryAsync(id, category);
                //TempData["success"] = "Uppdaterad";
                //return RedirectToAction(nameof(Index));
                try
                {
                    await _apiService.UpdateCategoryAsync(id, category);
                    TempData["success"] = "Uppdaterad";
                }
                catch (Exception)
                {
                    if (!await CategoryExists(category.CategoryId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(category);
        }

        // GET: Categories/Delete/5
        public async Task<IActionResult> Delete(int id)
        {
            if (id == null) //Varför har vi dubbelt?
            {
                return NotFound();
            }

            var category = await _apiService.GetCategoryByIdAsync(id);
            

            if (category == null)
            {
                return NotFound();
            }

            return View(category);
        }

        // POST: Categories/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var category = await _apiService.GetCategoryByIdAsync(id);
            if (category != null)
            {
                await _apiService.DeleteCategoryAsync(category.CategoryId);
            }

            TempData["success"] = "Kategori Raderad";
            return RedirectToAction(nameof(Index));
        }

        private async Task<bool> CategoryExists(int id)
        {
            var category = await _apiService.GetCategoryByIdAsync(id);

            if (category == null)
            {
                return false;
            }
            else
            {
                return true;
            }
            
            //.Categories.Any(e => e.CategoryId == id);
        }
    }
}
