using BiteBazaarWeb.Models;
using BiteBazaarWeb.Services;
using BiteBazaarWeb.Utilities;
using BiteBazaarWeb.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using X.PagedList;

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
        public async Task<IActionResult> Index(int? page, int? pageSize)
        {
            int defaultPageSize = 15;
            int pageNumber = page ?? 1;
            int currentPageSize = pageSize ?? defaultPageSize;

            var products = await _productService.GetProductsAsync();
            if (products == null || !products.Any())
            {
                return View(new List<Product>());
            }

            var pagedProducts = products.ToPagedList(pageNumber, currentPageSize);

            return View(pagedProducts);
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
                    CampaignStart = model.Product.CampaignStart,
                    CampaignEnd = model.Product.CampaignEnd,
                    CampaignPercent = model.Product.CampaignPercent / 100,
                    TempPrice = model.Product.TempPrice,
                    FkCategoryId = model.Product.FkCategoryId,
                    Quantity = model.Product.Quantity,
                    ImageURL = model.ProductImage.URL
                };

                if (product.CampaignStart <= DateTime.UtcNow && product.CampaignEnd >= DateTime.UtcNow)
                {
                    if (product.TempPrice == 0)
                    {
                        product.TempPrice = product.Price * (1 - product.CampaignPercent);
                    }
                }


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
            model.Product.CampaignPercent = model.Product.CampaignPercent * 100;
            model.Product.TempPrice = 0;
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
            productFromAPI.Quantity = model.Product.Quantity;
            productFromAPI.CampaignStart = model.Product.CampaignStart;
            productFromAPI.CampaignEnd = model.Product.CampaignEnd;
            productFromAPI.CampaignPercent = model.Product.CampaignPercent / 100;
            productFromAPI.TempPrice = model.Product.TempPrice;
            productFromAPI.FkCategoryId = model.Product.FkCategoryId;

            if (productFromAPI.CampaignStart <= DateTime.UtcNow && productFromAPI.CampaignEnd >= DateTime.UtcNow)
            {
                if (productFromAPI.TempPrice == 0)
                {
                    productFromAPI.TempPrice = productFromAPI.Price * (1 - productFromAPI.CampaignPercent);
                }
            }
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
