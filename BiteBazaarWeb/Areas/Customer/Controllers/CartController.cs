using BiteBazaarWeb.Data;
using BiteBazaarWeb.Models;
using BiteBazaarWeb.Utilities;
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

            if (!carts.Any())
            {
                TempData["error"] = "Inga varor i kundkorgen";
                return RedirectToAction(nameof(Index), "Home");
            }
            var user = _context.Users.OfType<ApplicationUser>().FirstOrDefault(x => x.Id == userId);

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
            if (!model.Terms)
            {
                TempData["error"] = "Du måste godkänna Allmänna Köpvillkor";
                return RedirectToAction(nameof(Checkout));
            }


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
            HttpContext.Session.Clear();

            return View("ConfirmationOrder", "Cart");
        }

        public IActionResult Terms()
        {
            return View();
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

        public async Task<IActionResult> OrderHistoryAdmin()
        {
            var orders = await _context.Orders.ToListAsync();
            if (orders == null)
            {
                TempData["error"] = "Hittade inga ordrar";
                return RedirectToAction(nameof(Index));
            }
            return View(orders);
        }
        public async Task<IActionResult> HistoryDetailsAdmin(int OrderId)
        {
            var orderspec = await _context.OrderSpecifications.Include(x => x.Product)
                .Where(x => x.FkOrderId == OrderId).ToListAsync();
            if (!orderspec.Any())
            {
                TempData["error"] = "Något gick fel";
                return RedirectToAction(nameof(Index));
            }
            var order = _context.Orders.FirstOrDefault(x => x.OrderId == OrderId);
            var model = new OrderHistoryVM
            {
                Specifications = orderspec,
                Order = order
            };

            return View(model);
        }

        public async Task<IActionResult> ShipOrder(int id)
        {
            var order = await _context.Orders.FirstOrDefaultAsync(x => x.OrderId == id);
            order.Status = OrderStatus.Skickad;
            order.ShippingDate = DateTime.Now;

            _context.Orders.Update(order);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(OrderHistoryAdmin));
        }

        public async Task<IActionResult> Plus(int id)
        {
            var cart = await _context.Carts.Include(x => x.Product).FirstOrDefaultAsync(x => x.CartId == id);

            if (cart.Product.Quantity > cart.Count)
            {
                cart.Count += 1;
                _context.Carts.Update(cart);
                await _context.SaveChangesAsync();
            }
            else
            {
                TempData["error"] = "För få varor i lager";
            }


            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Minus(int id)
        {
            var cart = await _context.Carts.Include(x => x.Product).FirstOrDefaultAsync(x => x.CartId == id);

            if (cart.Count == 1)
            {
                _context.Carts.Remove(cart);
                var count = _context.Carts.Where(x => x.FkApplicationUserId == cart.FkApplicationUserId).Count();
                HttpContext.Session.SetInt32(SD.SessionCount, count - 1);
            }
            else
            {
                cart.Count -= 1;
                _context.Carts.Update(cart);

            }
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Delete(int id)
        {
            var cart = await _context.Carts.Include(x => x.Product).FirstOrDefaultAsync(x => x.CartId == id);

            _context.Carts.Remove(cart);

            var count = _context.Carts.Where(x => x.FkApplicationUserId == cart.FkApplicationUserId).Count();
            HttpContext.Session.SetInt32(SD.SessionCount, count - 1);


            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }


    }
}
