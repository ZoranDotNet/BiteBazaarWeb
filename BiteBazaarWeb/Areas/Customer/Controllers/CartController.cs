﻿using BiteBazaarWeb.Data;
using BiteBazaarWeb.Models;
using BiteBazaarWeb.Services;
using BiteBazaarWeb.Utilities;
using BiteBazaarWeb.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace BiteBazaarWeb.Areas.Customer.Controllers
{
    [Area("Customer")]
    [Authorize]
    public class CartController : Controller
    {
        private readonly AppDbContext _context;
        private readonly ProductService _productService;
        private readonly CategoryService _categoryService;
        private readonly ProductImageService _productImageService;

        public CartController(AppDbContext context, ProductService productService, CategoryService categoryService, ProductImageService productImageService)
        {
            _context = context;
            _productService = productService;
            _categoryService = categoryService;
            _productImageService = productImageService;

        }
        public async Task<IActionResult> Index()
        {
            string userId = GetUser();

            //Hämtar den inloggades carts
            var cartItems = _context.Carts.Where(x => x.FkApplicationUserId == userId).ToList();

            //Loopar över carts, hämtar produkten från API, sätter properties på Product
            foreach (var cart in cartItems)
            {
                var item = await _productService.GetProductByIdAsync(cart.FkProductId);
                cart.Product = item;
                _context.Carts.Update(cart);
            }

            await _context.SaveChangesAsync();
            //return View(cartItems);
            return PartialView("_ShoppingCart", cartItems);
        }


        public async Task<IActionResult> Checkout()
        {

            string userId = GetUser();

            var carts = _context.Carts.Where(x => x.FkApplicationUserId == userId).ToList();

            if (!carts.Any())
            {
                TempData["error"] = "Inga varor i kundkorgen";
                return RedirectToAction(nameof(Index), "Home");
            }

            foreach (var item in carts)
            {
                var cartProduct = await _productService.GetProductByIdAsync(item.FkProductId);
                item.Product = cartProduct;
                _context.Carts.Update(item);
            }

            await _context.SaveChangesAsync();

            var user = _context.ApplicationUsers.FirstOrDefault(x => x.Id == userId);

            foreach (var cart in carts)
            {
                if (cart.Product.Quantity < cart.Count)
                {
                    TempData["error"] = $"Tyvärr finns {cart.Product.Title} endast i {cart.Product.Quantity} exemplar";
                    return PartialView("_Shoppingcart", carts);
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
        public async Task<IActionResult> Checkout(CreateOrderVM model)
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

            var carts = _context.Carts.Where(x => x.FkApplicationUserId == model.ApplicationUserId).ToList();

            foreach (var item in carts)
            {
                var cartProduct = await _productService.GetProductByIdAsync(item.FkProductId);
                item.Product = cartProduct;
                _context.Carts.Update(item);
            }

            await _context.SaveChangesAsync();


            foreach (var cart in carts)
            {
                cart.Product.Quantity -= cart.Count;
                await _productService.UpdateProductAsync(cart.FkProductId, cart.Product);
                var orderSpec = new OrderSpecification
                {
                    Count = cart.Count,
                    FkProductId = cart.FkProductId,
                    FkOrderId = order.OrderId,
                };


                //Vi kollar om varan var på REA eller inte. Vi sparat vilket pris vi betalat så vi har historik
                if (cart.Product.TempPrice > 0 && cart.Product.IsCampaign)
                {
                    orderSpec.PayedPrice = cart.Product.TempPrice;
                }
                else
                {
                    orderSpec.PayedPrice = cart.Product.Price;
                }
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
            string userId = GetUser();

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
            var orderspec = await _context.OrderSpecifications.Where(x => x.FkOrderId == OrderId).ToListAsync();

            if (!orderspec.Any())
            {
                TempData["error"] = "Något gick fel";
                return RedirectToAction(nameof(Index));
            }

            var products = await _productService.GetProductsAsync();

            foreach (var product in products)
            {
                foreach (var spec in orderspec)
                {
                    if (spec.FkProductId == product.ProductId)
                    {
                        spec.Product = product;
                    }
                }
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
            var orderspec = await _context.OrderSpecifications
                .Where(x => x.FkOrderId == OrderId).ToListAsync();
            if (!orderspec.Any())
            {
                TempData["error"] = "Något gick fel";
                return RedirectToAction(nameof(Index));
            }

            var products = await _productService.GetProductsAsync();

            foreach (var product in products)
            {
                foreach (var spec in orderspec)
                {
                    if (spec.FkProductId == product.ProductId)
                    {
                        spec.Product = product;
                    }
                }
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
            var cart = await _context.Carts.FirstOrDefaultAsync(x => x.CartId == id);

            var product = await _productService.GetProductByIdAsync(cart.FkProductId);
            cart.Product = product;

            if (cart.Product.Quantity > cart.Count)
            {
                cart.Count += 1;
                _context.Carts.Update(cart);
                await _context.SaveChangesAsync();
            }
            else
            {
                TempData["warning"] = "Finns inga fler i lager";
            }

            var carts = await UpdateCart();

            return PartialView("_ShoppingCart", carts);
        }

        public async Task<IActionResult> Minus(int id)
        {
            var cart = await _context.Carts.FirstOrDefaultAsync(x => x.CartId == id);
            var product = await _productService.GetProductByIdAsync(cart!.FkProductId);
            cart.Product = product;

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


            var carts = await UpdateCart();

            return PartialView("_ShoppingCart", carts);
        }

        public async Task<IActionResult> Delete(int id)
        {
            var cart = await _context.Carts.FirstOrDefaultAsync(x => x.CartId == id);

            _context.Carts.Remove(cart);
            var count = _context.Carts.Where(x => x.FkApplicationUserId == cart.FkApplicationUserId).Count();
            HttpContext.Session.SetInt32(SD.SessionCount, count - 1);

            await _context.SaveChangesAsync();

            var carts = await UpdateCart();
            return PartialView("_ShoppingCart", carts);
        }

        private string GetUser()
        {
            //Vem är inloggad
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var userId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;
            return userId;
        }

        private async Task<List<Cart>> UpdateCart()
        {
            string userId = GetUser();


            //Hämtar den inloggades carts
            var carts = _context.Carts.Where(x => x.FkApplicationUserId == userId).ToList();

            //Loopar över carts, hämtar produkten från API, sätter properties på Product
            foreach (var item in carts)
            {
                var cartProduct = await _productService.GetProductByIdAsync(item.FkProductId);
                item.Product = cartProduct;
                _context.Carts.Update(item);
            }

            await _context.SaveChangesAsync();
            return carts;
        }
    }
}
