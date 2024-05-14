using BiteBazaarWeb.Data;
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

        //skapa order


    }
}
