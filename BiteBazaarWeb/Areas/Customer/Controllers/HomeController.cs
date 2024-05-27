using BiteBazaarWeb.Data;
using BiteBazaarWeb.Models;
using BiteBazaarWeb.Services;
using BiteBazaarWeb.Utilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Diagnostics;
using System.Security.Claims;
using X.PagedList;

namespace BiteBazaarWeb.Areas.Customer.Controllers
{
    [Area("Customer")]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly AppDbContext _context;
        private readonly ProductService _productService;
        private readonly CategoryService _categoryService;
        private readonly ProductImageService _productImageService;

        public HomeController(ILogger<HomeController> logger, AppDbContext context,
            ProductService productService, CategoryService categoryService, ProductImageService productImageService)
        {
            _logger = logger;
            _context = context;

            _productService = productService;
            _categoryService = categoryService;
            _productImageService = productImageService;

        }

        public IActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> Products(int? page, int? pageSize)
        {
            int defaultPageSize = 20;
            int pageNumber = page ?? 1;
            int currentPageSize = pageSize ?? defaultPageSize;

            var productList = await _productService.GetProductsAsync();

            //Vi måste kolla om en ev kampanj har börjat.
            foreach (var item in productList)
            {
                if (item.CampaignStart <= DateTime.UtcNow && item.CampaignEnd >= DateTime.UtcNow)
                {
                    item.IsCampaign = true;
                    await _productService.UpdateProductAsync(item.ProductId, item);
                }
            }
            var products = await _productService.GetProductsAsync();


            var pagedProducts = productList.ToPagedList(pageNumber, currentPageSize);

            ViewData["FkCategoryId"] = new SelectList(await _categoryService.GetCategoriesAsync(), "CategoryId", "Title");

            return View(pagedProducts);
        }


        public async Task<IActionResult> FilterProducts(int filter)
        {
            List<Product> products = new();

            if (filter != 0)
            {
                products = await _productService.GetProductsByCategoryIdAsync(filter);
            }

            return PartialView("_ProductList", products);

        }
        public async Task<IActionResult> GetCategoryInfo(int categoryId)
        {
            if (categoryId == 0)
            {
                return RedirectToAction(nameof(Index));
            }

            var category = await _categoryService.GetCategoryByIdAsync(categoryId);
            if (category == null)
            {
                return RedirectToAction(nameof(Index));
            }

            return Json(new { Title = category.Title, Description = category.Description });
        }

        public async Task<IActionResult> SearchProducts(string searchString)
        {
            List<Product> products = new();

            if (!searchString.IsNullOrEmpty())
            {
                products = await _productService.GetProductsBySearchAsync(searchString);
            }

            return PartialView("_ProductList", products);

        }

        public async Task<IActionResult> Details(int id)
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
            var cart = new Cart
            {
                FkProductId = id,
                Product = product,
                Count = 1
            };
            return View(cart);
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Details(Cart cart)
        {

            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var userId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;
            cart.FkApplicationUserId = userId;

            // Hämta produkten från API:et
            var product = await _productService.GetProductByIdAsync(cart.FkProductId);
            if (product == null)
            {
                TempData["error"] = "Produkten kunde inte hittas.";
                return RedirectToAction("Details", cart.FkProductId);
            }

            var existingCart = _context.Carts.FirstOrDefault(x => x.FkApplicationUserId == userId && x.FkProductId == cart.FkProductId);
            if (existingCart != null)
            {
                int nrOfItems = existingCart.Count;
                if (product.Quantity < (cart.Count + nrOfItems))
                {
                    TempData["error"] = $"Finns endast {product.Quantity} kvar i Lager";
                    return RedirectToAction("Details", cart.FkProductId);
                }
            }


            // Kontrollera att det finns tillräckligt många produkter innan vi lägger dem i varukorgen
            if (product.Quantity < cart.Count)
            {
                TempData["error"] = $"Finns endast {product.Quantity} kvar i Lager";
                return RedirectToAction("Details", cart.FkProductId);
            }

            // Kolla om varukorgen redan finns i databasen
            var cartFromDb = await _context.Carts.AsNoTracking().FirstOrDefaultAsync(x => x.FkApplicationUserId == userId
            && x.FkProductId == cart.FkProductId);


            if (cartFromDb != null)
            {
                //Vi drar av från lagret när köpet genomförs!!
                //Om varukorgen finns i DB uppdateras antalet på befintlig varukorg
                cartFromDb.Count += cart.Count;
                _context.Carts.Update(cartFromDb);
                _context.SaveChanges();
            }
            else
            {
                // Skapa en ny varukorg om den inte redan finns
                _context.Carts.Add(cart);

                await _context.SaveChangesAsync();

                var count = _context.Carts.Where(x => x.FkApplicationUserId == userId).Count();
                HttpContext.Session.SetInt32(SD.SessionCount, count);

            }

            TempData["success"] = "Varukorg uppdaterad";

            return RedirectToAction(nameof(Products));
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
