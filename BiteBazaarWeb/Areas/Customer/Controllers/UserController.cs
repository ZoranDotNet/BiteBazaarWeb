using BiteBazaarWeb.Data;
using BiteBazaarWeb.Models;
using BiteBazaarWeb.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;


namespace BiteBazaarWeb.Areas.Customer.Controllers
{
    [Area("Customer")]
    [Authorize]
    public class UserController : Controller
    {
        private readonly AppDbContext _context;
        private readonly ProductService _productService;

        public UserController(AppDbContext context, ProductService productService)
        {
            _context = context;
            _productService = productService;
        }

        public async Task<IActionResult> UpdateAddress()
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var userId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;

            var user = _context.Users.OfType<ApplicationUser>().FirstOrDefault(x => x.Id == userId);

            return View(user);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateAddress(ApplicationUser model)
        {
            if (ModelState.IsValid)
            {
                var user = await _context.ApplicationUsers.FirstOrDefaultAsync(x => x.Id == model.Id);
                if (user == null)
                {
                    return NotFound();
                }

                user.Name = model.Name;
                user.StreetAddress = model.StreetAddress;
                user.ZipCode = model.ZipCode;
                user.City = model.City;

                _context.ApplicationUsers.Update(user);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index), "Home");
            }

            return View(model);
        }

        public async Task<IActionResult> AddToList(int id)
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var userId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;

            var favProduct = new FavouriteProduct
            {
                FkApplicationUserId = userId,
                FkProductId = id
            };

            var item = await _context.FavoriteProducts.FirstOrDefaultAsync(x => x.FkProductId == id);

            if (item != null)
            {
                TempData["error"] = "Finns redan som favorit";
                return RedirectToAction("Products", "Home");
            }

            _context.FavoriteProducts.Add(favProduct);
            await _context.SaveChangesAsync();
            TempData["success"] = "Tillagd bland favoriter";

            return RedirectToAction("Products", "Home");
        }

        public async Task<IActionResult> ShowList()
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var userId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;

            var favorites = await _context.FavoriteProducts.Where(x => x.FkApplicationUserId == userId).ToListAsync();

            if (favorites == null || favorites.Count == 0)
            {
                TempData["error"] = "Finns inga favoriter sparade";
                return RedirectToAction("Index", "Home");
            }
            var products = await _productService.GetProductsAsync();

            foreach (var product in products)
            {
                foreach (var item in favorites)
                {
                    if (item.FkProductId == product.ProductId)
                    {
                        item.Product = product;
                    }
                }
            }


            return View(favorites);
        }

        public async Task<IActionResult> RemoveItem(int id)
        {
            var favouriteItem = await _context.FavoriteProducts.FirstOrDefaultAsync(x => x.FavouriteProductId == id);

            _context.FavoriteProducts.Remove(favouriteItem);
            await _context.SaveChangesAsync();

            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var userId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;

            var favorites = await _context.FavoriteProducts.Where(x => x.FkApplicationUserId == userId).ToListAsync();

            if (favorites.Count == 0)
            {
                TempData["error"] = "Inga favoriter kvar";
                return RedirectToAction("Index", "Home");
            }

            return RedirectToAction(nameof(ShowList));
        }
    

    }
}
