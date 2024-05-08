using BiteBazaarWeb.Models;
using BiteBazaarWeb.Services;
using Microsoft.AspNetCore.Mvc;

namespace BiteBazaarWeb.Controllers
{
    public class ProductsController : Controller
    {
        private readonly ProductService _service;

        public ProductsController(ProductService service)
        {
            _service = service;
        }
        public async Task<IActionResult> Index()
        {
            var products = await _service.GetProductsAsync();
            if (products == null || !products.Any())
            {
                return View(new List<Product>());
            }

            return View(products);
        }

        public async Task<IActionResult> Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Product product)
        {
            if (ModelState.IsValid)
            {
                await _service.AddProductAsync(product);
                return RedirectToAction(nameof(Index));
            }

            return View(product);
        }

        public IActionResult GetById()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> GetById(int id)
        {
            var product = await _service.GetProductByIdAsync(id);
            if (product == null)
            {
                return View("ByIdResult");
            }
            return View("ByIdResult", product);
        }

        public async Task<IActionResult> Edit(int id)
        {
            var product = await _service.GetProductByIdAsync(id);

            return View(product);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(int id, Product product)
        {
            if (ModelState.IsValid)
            {
                await _service.UpdateProductAsync(id, product);
                return RedirectToAction(nameof(Index));
            }

            return View(product);
        }


        [HttpPost]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await _service.DeleteProductAsync(id);
            return RedirectToAction(nameof(Index));
        }

    }
}
