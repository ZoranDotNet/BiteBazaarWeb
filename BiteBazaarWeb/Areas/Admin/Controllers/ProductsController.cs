using BiteBazaarWeb.Models;
using BiteBazaarWeb.Services;
using BiteBazaarWeb.Utilities;
using BiteBazaarWeb.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace BiteBazaarWeb.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = (SD.Role_Admin))]
    public class ProductsController : Controller
    {

        private readonly ProductService _productService;
        private readonly CategoryService _categoryService;
        private readonly ProductImageService _productImageService;
        public ProductsController(ProductService productService, CategoryService categoryService, ProductImageService productImageService)
        {
            _productService = productService;
            _categoryService = categoryService;
            _productImageService = productImageService;
        }


        // GET: Products
        public async Task<IActionResult> Index()
        {
            var products = await _productService.GetProductsAsync();
            if (products == null || !products.Any())
            {
                return View(new List<Product>());
            }
            return View(products);
        }


        // GET: Products/Create
        public async Task<IActionResult> Create()
        {
            ViewData["FkCategoryId"] = new SelectList(await _categoryService.GetCategoriesAsync(), "CategoryId", "Title");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateProductVM model)
        {


            if (ModelState.IsValid)
            {
                var product = new PostProductVM
                {
                    Title = model.Product.Title,
                    Description = model.Product.Description,
                    Price = model.Product.Price,
                    FkCategoryId = model.Product.FkCategoryId,
                    Quantity = model.Product.Quantity,
                    ImageURL = model.ProductImage.URL
                };


                await _productService.AddProductAsync(product);
                TempData["success"] = "Ny Produkt tillagd";
                return RedirectToAction(nameof(Index));

            }

            ViewData["FkCategoryId"] = new SelectList(await _categoryService.GetCategoriesAsync(), "CategoryId", "CategoryId", model.Product.FkCategoryId);

            return View(model);
        }

        // GET: Products/Edit/5
        public async Task<IActionResult> Edit(int id) //Hur blir det med att loopa fram bild svaren?
        {
            if (id == null)
            {
                return NotFound();
            }

            var product = await _productService.GetProductByIdAsync(id);
            if (product == null)
            {
                return NotFound();
            }
            ViewData["FkCategoryId"] = new SelectList(await _categoryService.GetCategoriesAsync(), "CategoryId", "Title", product.FkCategoryId);

            var model = new CreateProductVM
            {
                Product = product
            };

            return View(model);
        }

        // POST: Products/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, CreateProductVM model)
        {
            var productFromAPI = await _productService.GetProductByIdAsync(id);

            if (productFromAPI == null)
            {
                return NotFound();
            }

            productFromAPI.Title = model.Product.Title;
            productFromAPI.Description = model.Product.Description;
            productFromAPI.Price = model.Product.Price;
            productFromAPI.FkCategoryId = model.Product.FkCategoryId;
            productFromAPI.Quantity = model.Product.Quantity;


            await _productService.UpdateProductAsync(id, productFromAPI);

            TempData["success"] = "Produkten är sparad";
            return RedirectToAction(nameof(Index));


        }

        // GET: Products/Delete/5
        public async Task<IActionResult> Delete(int id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var product = await _productService.GetProductByIdAsync(id);
            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }

        // POST: Products/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var product = await _productService.GetProductByIdAsync(id);
            if (product != null)
            {
                await _productService.DeleteProductAsync(id);
            }
            TempData["success"] = "Produkten raderad";
            return RedirectToAction(nameof(Index));
        }


    }
}
