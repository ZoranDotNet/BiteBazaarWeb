using BiteBazaarWeb.Data;
using BiteBazaarWeb.Services;
using BiteBazaarWeb.Utilities;
using BiteBazaarWeb.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BiteBazaarWeb.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = SD.Role_Admin)]
    public class DashboardController : Controller
    {
        private readonly AppDbContext _context;
        private readonly ProductService _productService;

        public DashboardController(AppDbContext context, ProductService productService)
        {
            _context = context;
            _productService = productService;
        }
        public IActionResult Index()
        {
            return View();
        }


        public IActionResult ShowResults(DateTime fromDate, DateTime toDate)
        {
            if (ModelState.IsValid)
            {
                var orders = _context.Orders.Where(x => x.OrderDate >= fromDate && x.OrderDate <= toDate);
                if (!orders.Any())
                {
                    TempData["error"] = "Inga försäljningar har skett under denna period";
                }
                decimal total = 0;

                foreach (var order in orders)
                {
                    total += order.OrderTotal;
                }
                int count = orders.Count();

                return Json(new { total = total, count = count });
                //return PartialView("_Statistics", orders);
            }

            return View();
        }

        public async Task<IActionResult> ShowLowInventory()
        {
            var products = await _productService.GetProductsAsync();

            var lowCount = products.Where(x => x.Quantity <= 5).OrderBy(x => x.Quantity);

            return Json(new { lowCount });
        }

        public async Task<IActionResult> TopProducts(DateTime fromDate, DateTime toDate)
        {
            var orders = _context.Orders.Include(x => x.OrderSpecifications).Where(x => x.OrderDate >= fromDate && x.OrderDate <= toDate);
            if (!orders.Any())
            {
                TempData["error"] = "Inga försäljningar har skett under denna period";
            }

            var products = await _productService.GetProductsAsync();

            var model = new List<ProductSpecificationVM>();

            foreach (var order in orders)
            {
                foreach (var item in order.OrderSpecifications)
                {
                    foreach (var product in products)
                    {
                        if (item.FkProductId == product.ProductId)
                        {
                            var obj = new ProductSpecificationVM
                            {
                                Id = product.ProductId,
                                Title = product.Title,
                                Price = product.Price,
                                Count = item.Count,
                            };
                            model.Add(obj);
                        }
                    }
                }
            }
            var response = model.OrderByDescending(x => x.Count).Take(10);

            return Json(response);
        }


    }
}
