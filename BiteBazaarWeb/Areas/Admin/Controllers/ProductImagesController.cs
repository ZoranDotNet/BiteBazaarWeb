using BiteBazaarWeb.Data;
using BiteBazaarWeb.Models;
using BiteBazaarWeb.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace BiteBazaarWeb.Areas.Admin.Controllers
{
    [Area("Admin")]
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

        // GET: ProductImages/Details/5 ------------ VI BEhöVER EJ DENNA?
        public async Task<IActionResult> Details(int id)
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

        // GET: ProductImages/Create
        public async Task<IActionResult> Create()
        {
            ViewData["FkProductId"] = new SelectList(await _productService.GetProductsAsync(), "ProductId", "Title");
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
            //ViewData["FkProductId"] = new SelectList(_context.Products, "ProductId", "Title", productImage.FkProductId);
            return View(productImage);
        }

        //// GET: ProductImages/Edit/5
        //public async Task<IActionResult> Edit(int? id) ----------------- VI BEHÖVER EJ DENNA?
        //{
        //    if (id == null)
        //    {
        //        return NotFound();
        //    }

        //    var productImage = await _context.ProductImages.FindAsync(id);
        //    if (productImage == null)
        //    {
        //        return NotFound();
        //    }
        //    ViewData["FkProductId"] = new SelectList(_context.Products, "ProductId", "ProductId", productImage.FkProductId);
        //    return View(productImage);
        //}

        //// POST: ProductImages/Edit/5
        //// To protect from overposting attacks, enable the specific properties you want to bind to.
        //// For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> Edit(int id, [Bind("ProductImageId,URL,FkProductId")] ProductImage productImage)
        //{
        //    if (id != productImage.ProductImageId)
        //    {
        //        return NotFound();
        //    }

        //    if (ModelState.IsValid)
        //    {
        //        try
        //        {
        //            _context.Update(productImage);
        //            await _context.SaveChangesAsync();
        //        }
        //        catch (DbUpdateConcurrencyException)
        //        {
        //            if (!ProductImageExists(productImage.ProductImageId))
        //            {
        //                return NotFound();
        //            }
        //            else
        //            {
        //                throw;
        //            }
        //        }
        //        return RedirectToAction(nameof(Index));
        //    }
        //    ViewData["FkProductId"] = new SelectList(_context.Products, "ProductId", "ProductId", productImage.FkProductId);
        //    return View(productImage);
        //}

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
            var productImage = await _productImageService.GetProductImageByIdAsync(id);

            var allImages = await _productImageService.GetProductImagesAsync();
            var thisProductImages = allImages.Where(i => i.FkProductId == productImage.FkProductId).ToList();


            if (thisProductImages.Count > 1)
            {
                await _productImageService.DeleteProductImageAsync(id);
                TempData["success"] = "Bilden raderad";
                return RedirectToAction("Index", "Products");
            }
            else
            {
                TempData["error"] = "Går ej radera sista bilden";
                return RedirectToAction("Delete", new { id });
            }
            
        }

        //private bool ProductImageExists(int id)
        //{
        //    return _context.ProductImages.Any(e => e.ProductImageId == id);
        //}
    }
}
