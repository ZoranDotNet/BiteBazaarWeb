using BiteBazaarWeb.Data;
using BiteBazaarWeb.Models;
using BiteBazaarWeb.Utilities;
using BiteBazaarWeb.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace BiteBazaarWeb.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = (SD.Role_Admin))]
    public class AdminController : Controller
    {
        private readonly AppDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        public AdminController(AppDbContext context, RoleManager<IdentityRole> roleManager, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
            _roleManager = roleManager;
        }
        public async Task<IActionResult> ManageUsers()
        {
            var users = await _context.Set<ApplicationUser>().ToListAsync();

            foreach (var user in users)
            {
                user.Role = _userManager.GetRolesAsync(user).GetAwaiter().GetResult().FirstOrDefault()!;
            }

            return View(users);
        }

        public async Task<IActionResult> UpdatedUsers()
        {
            var users = await _context.Set<ApplicationUser>().ToListAsync();

            foreach (var user in users)
            {
                user.Role = _userManager.GetRolesAsync(user).GetAwaiter().GetResult().FirstOrDefault()!;
            }

            return PartialView("_Users", users);
        }

        [HttpPost]
        public async Task<IActionResult> LockUnlock(string id)
        {
            var user = _context.ApplicationUsers.FirstOrDefault(x => x.Id == id);
            if (user == null)
            {
                TempData["error"] = "Kunde inte hitta Användaren!";
                return Json(new { success = false, message = "Kunde inte hitta Användaren!" });
                //return RedirectToAction(nameof(ManageUsers));
            }

            //öppnar låst konto
            if (user.LockoutEnd != null && user.LockoutEnd > DateTime.UtcNow)
            {
                user.LockoutEnd = DateTime.Now;
            }
            else
            {
                user.LockoutEnd = DateTime.Now.AddYears(50); //låser kontot 50 år framåt
            }
            _context.ApplicationUsers.Update(user);
            await _context.SaveChangesAsync();
            //return RedirectToAction(nameof(ManageUsers));
            return Json(new { success = true, message = "Ändrad!" });
        }

        public async Task<IActionResult> UpdateRole(string userId)
        {
            var user = await _context.ApplicationUsers.FirstOrDefaultAsync(x => x.Id == userId);
            if (user == null)
            {
                TempData["error"] = "Kunde inte hitta användaren";
                return RedirectToAction(nameof(ManageUsers));
            }

            var userRole = await _userManager.GetRolesAsync(user);
            user.Role = userRole.FirstOrDefault();
            var roles = _roleManager.Roles.Select(role => role.Name);
            UpdateRoleVM model = new UpdateRoleVM()
            {
                ApplicationUser = user,
                RoleList = roles.Select(role => new SelectListItem
                {
                    Text = role,
                    Value = role
                })
            };

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> UpdateRole(UpdateRoleVM model)
        {
            ApplicationUser user = await _context.ApplicationUsers.FirstOrDefaultAsync(x => x.Id == model.ApplicationUser.Id);

            string oldRole = _userManager.GetRolesAsync(user).GetAwaiter().GetResult().FirstOrDefault()!;


            if (!model.ApplicationUser.Role.Equals(oldRole))
            {
                if (model.ApplicationUser.Role == SD.Role_Admin)
                {
                    user.Role = SD.Role_Admin;
                }
                else
                {
                    user.Role = SD.Role_Customer;
                }

                _userManager.RemoveFromRoleAsync(user, oldRole).GetAwaiter().GetResult();
                _userManager.AddToRoleAsync(user, model.ApplicationUser.Role).GetAwaiter().GetResult();
            }



            return RedirectToAction(nameof(ManageUsers));
        }

        public async Task<IActionResult> SearchUser(string searchString)
        {
            List<ApplicationUser> users = new();

            if (!searchString.IsNullOrEmpty())
            {
                users = await _context.ApplicationUsers.Where(x => x.Name.Contains(searchString)).ToListAsync();

                foreach (var user in users)
                {
                    user.Role = _userManager.GetRolesAsync(user).GetAwaiter().GetResult().FirstOrDefault()!;
                }
            }

            return PartialView("_Users", users);

        }

    }
}
