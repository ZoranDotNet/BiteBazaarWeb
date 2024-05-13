using BiteBazaarWeb.Data;
using BiteBazaarWeb.Models;
using BiteBazaarWeb.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace BiteBazaarWeb.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class ProductsController : Controller
    {
        private readonly AppDbContext _context;

        public ProductsController(AppDbContext context)
        {
            _context = context;
        }

        // GET: Products
        public async Task<IActionResult> Index()
        {
            var appDbContext = _context.Products.Include(p => p.Category).Include(x => x.Images);
            return View(await appDbContext.ToListAsync());
        }

        // GET: Products/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var product = await _context.Products
                .Include(p => p.Category).Include(x => x.Images)
                .FirstOrDefaultAsync(m => m.ProductId == id);
            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }

        // GET: Products/Create
        public IActionResult Create()
        {
            ViewData["FkCategoryId"] = new SelectList(_context.Categories, "CategoryId", "Title");
            return View();
        }

        // POST: Products/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateProductVM model)
        {
            if (ModelState.IsValid)
            {
                var product = new Product
                {
                    Title = model.Product.Title,
                    Description = model.Product.Description,
                    FkCategoryId = model.Product.FkCategoryId,
                    Price = model.Product.Price,
                };

                _context.Add(product);
                await _context.SaveChangesAsync();


                var image = new ProductImage
                {
                    URL = model.ProductImage.URL,
                    FkProductId = product.ProductId,
                };

                _context.ProductImages.Add(image);
                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }
            ViewData["FkCategoryId"] = new SelectList(_context.Categories, "CategoryId", "CategoryId", model.Product.FkCategoryId);



            return View(model);
        }

        // GET: Products/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var product = await _context.Products.Include(x => x.Images).FirstOrDefaultAsync(x => x.ProductId == id);
            if (product == null)
            {
                return NotFound();
            }
            ViewData["FkCategoryId"] = new SelectList(_context.Categories, "CategoryId", "Title", product.FkCategoryId);

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
        public async Task<IActionResult> Edit(CreateProductVM model)
        {
            var productFromDb = await _context.Products.Include(x => x.Category)
                .FirstOrDefaultAsync(x => x.ProductId == model.Product.ProductId);

            if (productFromDb == null)
            {
                return NotFound();
            }

            productFromDb.Title = model.Product.Title;
            productFromDb.Description = model.Product.Description;
            productFromDb.Price = model.Product.Price;
            productFromDb.FkCategoryId = model.Product.FkCategoryId;

            _context.Products.Update(productFromDb);
            await _context.SaveChangesAsync();


            return RedirectToAction(nameof(Index));

            //ViewData["FkCategoryId"] = new SelectList(_context.Categories, "CategoryId", "Title", model.Product.FkCategoryId);
            //return View(model);
        }

        // GET: Products/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var product = await _context.Products
                .Include(p => p.Category)
                .FirstOrDefaultAsync(m => m.ProductId == id);
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
            var product = await _context.Products.FindAsync(id);
            if (product != null)
            {
                _context.Products.Remove(product);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ProductExists(int id)
        {
            return _context.Products.Any(e => e.ProductId == id);
        }
    }
}
