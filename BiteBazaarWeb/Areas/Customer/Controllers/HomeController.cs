using BiteBazaarWeb.Data;
using BiteBazaarWeb.Models;
using BiteBazaarWeb.Services;
using BiteBazaarWeb.Utilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using System.Security.Claims;

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

        public async Task<IActionResult> Products()
        {
            var products = await _productService.GetProductsAsync();

            return View(products);
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
                FkProductId = product.ProductId,
                Product = product,
                Count = 1
            };
            return View(cart);

        }

    //    [HttpPost]
    //    [Authorize]
    //    public async Task<IActionResult> Details(Cart cart)
    //    {
    //        var claimsIdentity = (ClaimsIdentity)User.Identity;
    //        var userId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;
    //        cart.FkApplicationUserId = userId;

    //        Cart cartFromDb = await _context.Carts.AsNoTracking().FirstOrDefaultAsync(x => x.FkApplicationUserId == userId
    //        && x.FkProductId == cart.FkProductId);

    //        Product product = await _context.Products.AsNoTracking().FirstOrDefaultAsync(x => x.ProductId == cart.FkProductId);
    //        if (product != null)
    //        {    //Vi kollar att det finns tillräckligt många produkter innan vi lägger det i korgen
    //            //Vi räknar inte av från lagret förrän dom genomför köpet. Kan lägga i korgen och sedan logga ut.
    //            if (product.Quantity < cart.Count)
    //            {
    //                TempData["error"] = $"Finns endast {product.Quantity} kvar i Lager";
    //                return RedirectToAction("Details", cart.FkProductId);
    //            }
    //        }
    //        if (cartFromDb != null)
    //        {
    //            //Vi drar av från lagret när köpet genomförs!!
    //            //Om varukorgen finns i DB uppdaterar antalet på befintlig varukorg
    //            cartFromDb.Count += cart.Count;
    //            _context.Carts.Update(cartFromDb);
    //            _context.SaveChanges();
    //        }
    //        else
    //        {
    //            //Vi drar av från lagret när köpet genomförs!!
    //            //vi skapar ny korg om den inte finns
    //            _context.Carts.Add(cart);
    //            _context.SaveChanges();
    //            var count = _context.Carts.Where(x => x.FkApplicationUserId == userId).Count();
    //            HttpContext.Session.SetInt32(SD.SessionCount, count);

    //        }

    //        TempData["success"] = "Varukorg uppdaterad";

    //        return RedirectToAction(nameof(Products));
    //    }

    //    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    //    public IActionResult Error()
    //    {
    //        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    //    }
    //}


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
                return RedirectToAction("Details", new { id = cart.FkProductId });
            }

            // Kontrollera att det finns tillräckligt många produkter innan vi lägger dem i varukorgen
            if (product.Quantity < cart.Count)
            {
                TempData["error"] = $"Finns endast {product.Quantity} kvar i Lager";
                return RedirectToAction("Details", new { id = cart.FkProductId });
            }

            // Kolla om varukorgen redan finns i databasen
            var cartFromDb = await _context.Carts.AsNoTracking().FirstOrDefaultAsync(x => x.FkApplicationUserId == userId
            && x.FkProductId == cart.FkProductId);

            if (cartFromDb != null)
            {
                // Uppdatera antalet på befintlig varukorg
                cartFromDb.Count += cart.Count;
                _context.Carts.Update(cartFromDb);
            }
            else
            {
                // Skapa en ny varukorg om den inte redan finns
                _context.Carts.Add(cart);
                var count = _context.Carts.Where(x => x.FkApplicationUserId == userId).Count();
                
            }

            await _context.SaveChangesAsync();
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
