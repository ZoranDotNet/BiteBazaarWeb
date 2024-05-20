using BiteBazaarWeb.Data;
using BiteBazaarWeb.Models;
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

        public HomeController(ILogger<HomeController> logger, AppDbContext context)
        {
            _logger = logger;
            _context = context;
        }

        public IActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> Products()
        {
            var products = await _context.Products.Include(p => p.Category).Include(x => x.Images).ToListAsync();

            return View(products);
        }

        public async Task<IActionResult> Details(int id)
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
            var cart = new Cart
            {
                FkProductId = product.ProductId,
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

            Cart cartFromDb = await _context.Carts.AsNoTracking().FirstOrDefaultAsync(x => x.FkApplicationUserId == userId
            && x.FkProductId == cart.FkProductId);

            Product product = await _context.Products.AsNoTracking().FirstOrDefaultAsync(x => x.ProductId == cart.FkProductId);
            if (product != null)
            {    //Vi kollar att det finns tillräckligt många produkter innan vi lägger det i korgen
                //Vi räknar inte av från lagret förrän dom genomför köpet. Kan lägga i korgen och sedan logga ut.
                if (product.Quantity < cart.Count)
                {
                    TempData["error"] = $"Finns endast {product.Quantity} kvar i Lager";
                    return RedirectToAction("Details", cart.FkProductId);
                }
            }
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
                //Vi drar av från lagret när köpet genomförs!!
                //vi skapar ny korg om den inte finns
                _context.Carts.Add(cart);
                _context.SaveChanges();
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
