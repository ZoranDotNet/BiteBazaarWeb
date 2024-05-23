using BiteBazaarWeb.Models;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace BiteBazaarWeb.ViewModels
{
    public class UpdateRoleVM
    {
        public ApplicationUser ApplicationUser { get; set; }
        public IEnumerable<SelectListItem> RoleList { get; set; }
    }
}
