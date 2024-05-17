using BiteBazaarWeb.Data;
using BiteBazaarWeb.Models;
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

        public UserController(AppDbContext context)
        {
            _context = context;
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

    }
}
