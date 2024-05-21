using BiteBazaarWeb.Data;
using BiteBazaarWeb.Models;
using BiteBazaarWeb.Services;
using BiteBazaarWeb.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace BiteBazaarWeb.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class ProductsController : Controller
    {
        //private readonly AppDbContext _context;

        //public ProductsController(AppDbContext context)
        //{
        //    _context = context;
        //}

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

        //// POST: Products/Create
        //// To protect from overposting attacks, enable the specific properties you want to bind to.
        //// For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> Create(CreateProductVM model)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        var product = new Product
        //        {
        //            Title = model.Product.Title,
        //            Description = model.Product.Description,
        //            FkCategoryId = model.Product.FkCategoryId,
        //            Price = model.Product.Price,
        //            Quantity = model.Product.Quantity,
        //        };

        //        _context.Products.Add(product);
        //        await _context.SaveChangesAsync();



        //        var image = new ProductImage
        //        {
        //            URL = model.ProductImage.URL,
        //            FkProductId = product.ProductId,
        //        };

        //        _context.ProductImages.Add(image);
        //        await _context.SaveChangesAsync();
        //        TempData["success"] = "Ny Produkt tillagd";
        //        return RedirectToAction(nameof(Index));
        //    }
        //    ViewData["FkCategoryId"] = new SelectList(_context.Categories, "CategoryId", "CategoryId", model.Product.FkCategoryId);



        //    return View(model);
        //}

        // POST: Products/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
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

            //ViewData["FkCategoryId"] = new SelectList(await _categoryService.GetCategoriesAsync(), "CategoryId", "CategoryId", model.Product.FkCategoryId);

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

            //ViewData["FkCategoryId"] = new SelectList(_context.Categories, "CategoryId", "Title", model.Product.FkCategoryId);
            //return View(model);
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

        //private bool ProductExists(int id)
        //{
        //    return _context.Products.Any(e => e.ProductId == id);
        //}
    }
}
