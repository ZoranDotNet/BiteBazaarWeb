using BiteBazaarWeb.Data;
using BiteBazaarWeb.Models;
using BiteBazaarWeb.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace BiteBazaarWeb.Areas.Customer.Controllers
{
    [Area("Customer")]
    public class CartController : Controller
    {
        private readonly AppDbContext _context;

        public CartController(AppDbContext context)
        {
            _context = context;

        }
        public IActionResult Index()
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var userId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;

            var cart = _context.Carts.Where(x => x.FkApplicationUserId == userId).Include(x => x.Product)
                .Include(x => x.Product.Images).ToList();
            //hämta produkt från API hur?? 

            return View(cart);
        }

        //skapa order checka ut
        public IActionResult Checkout()
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var userId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;

            var carts = _context.Carts.Where(x => x.FkApplicationUserId == userId).Include(x => x.Product)
                .ThenInclude(x => x.Images).ToList();

            var user = _context.ApplicationUsers.FirstOrDefault(x => x.Id == userId);

            foreach (var cart in carts)
            {
                if (cart.Product.Quantity < cart.Count)
                {
                    TempData["error"] = $"Tyvärr finns {cart.Product.Title} endast i {cart.Product.Quantity} examplar";
                    return RedirectToAction(nameof(Index));
                };
            }


            var model = new CreateOrderVM
            {
                ApplicationUserId = userId,
                Carts = carts,
                ApplicationUser = user
            };

            return View(model);
        }
        [HttpPost]
        public IActionResult Checkout(CreateOrderVM model)
        {
            var order = new Order
            {
                FkApplicationUserId = model.ApplicationUserId,
                OrderTotal = model.OrderTotal,
                Name = model.ApplicationUser.Name,
                StreetAddress = model.ApplicationUser.StreetAddress,
                ZipCode = model.ApplicationUser.ZipCode,
                City = model.ApplicationUser.City,

            };

            _context.Orders.Add(order);
            _context.SaveChanges();

            var carts = _context.Carts.Where(x => x.FkApplicationUserId == model.ApplicationUserId).Include(x => x.Product).ToList();

            foreach (var cart in carts)
            {
                cart.Product.Quantity -= cart.Count;
                _context.Products.Update(cart.Product);
                var orderSpec = new OrderSpecification
                {
                    Count = cart.Count,
                    FkProductId = cart.FkProductId,
                    FkOrderId = order.OrderId
                };
                _context.OrderSpecifications.Add(orderSpec);
            }

            _context.Carts.RemoveRange(carts);
            _context.SaveChanges();


            return View("ConfirmationOrder", "Cart");
        }

        public async Task<IActionResult> History()
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var userId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;

            var orders = await _context.Orders.Where(x => x.FkApplicationUserId == userId).ToListAsync();
            if (orders == null)
            {
                TempData["error"] = "Hittade inga ordrar";
                return RedirectToAction(nameof(Index));
            }

            return View(orders);
        }

        public async Task<IActionResult> HistoryDetails(int OrderId)
        {
            var orderspec = await _context.OrderSpecifications.Include(x => x.Product).Where(x => x.FkOrderId == OrderId).ToListAsync();
            if (!orderspec.Any())
            {
                TempData["error"] = "Något gick fel";
                return RedirectToAction(nameof(Index));
            }

            return View(orderspec);
        }

    }
}
