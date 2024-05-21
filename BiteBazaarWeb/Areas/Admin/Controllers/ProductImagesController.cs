using BiteBazaarWeb.Models;
using BiteBazaarWeb.Services;
using BiteBazaarWeb.Utilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BiteBazaarWeb.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = (SD.Role_Admin))]
    public class ProductImagesController : Controller
    {

        private readonly ProductService _productService;
        private readonly CategoryService _categoryService;
        private readonly ProductImageService _productImageService;
        public ProductImagesController(ProductService productService, CategoryService categoryService, ProductImageService productImageService)
        {
            _productService = productService;
            _categoryService = categoryService;
            _productImageService = productImageService;
        }

        // GET: ProductImages
        public async Task<IActionResult> Index()
        {
            var productsWithImages = await _productImageService.GetProductImagesAsync();
            return View(productsWithImages);
        }

        // GET: ProductImages/Create
        public async Task<IActionResult> Create()
        {
            //ViewData["FkProductId"] = new SelectList(await _productService.GetProductsAsync(), "ProductId", "Title");
            return View();
        }

        // POST: ProductImages/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("URL,FkProductId")] ProductImage productImage, int id)
        {
            if (ModelState.IsValid)
            {
                productImage.FkProductId = id;
                await _productImageService.AddProductImageAsync(productImage);
                TempData["success"] = "Ny bild tillagd";
                return RedirectToAction(nameof(Index), "Products");
            }
            return View(productImage);
        }


        // GET: ProductImages/Delete/5
        public async Task<IActionResult> Delete(int id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var productImage = await _productImageService.GetProductImageByIdAsync(id);
            if (productImage == null)
            {
                return NotFound();
            }

            return View(productImage);
        }

        // POST: ProductImages/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await _productImageService.DeleteProductImageAsync(id);
            TempData["success"] = "Bilden raderad";
            return RedirectToAction("Index", "Products");

        }


    }
}
