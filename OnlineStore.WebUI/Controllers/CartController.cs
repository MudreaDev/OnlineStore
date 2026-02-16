using Microsoft.AspNetCore.Mvc;
using OnlineStore.Application.Repositories;
using OnlineStore.Application.Services;
using OnlineStore.Domain.Entities;
using OnlineStore.Domain.Strategies;
using OnlineStore.WebUI.Extensions;
using OnlineStore.WebUI.Models;

namespace OnlineStore.WebUI.Controllers
{
    public class CartController : Controller
    {
        private readonly DbProductRepository _productRepo;
        private readonly DbOrderRepository _orderRepo;
        private readonly DbUserRepository _userRepo;

        public CartController(DbProductRepository productRepo, DbOrderRepository orderRepo, DbUserRepository userRepo)
        {
            _productRepo = productRepo;
            _orderRepo = orderRepo;
            _userRepo = userRepo;
        }

        public IActionResult Index()
        {
            var cart = GetCart();
            var products = new List<Product>();
            foreach (var id in cart.ProductIds)
            {
                var p = _productRepo.GetById(id);
                if (p != null) products.Add(p);
            }

            var vm = new CartViewModel
            {
                Products = products,
                Total = products.Sum(p => p.Price)
            };

            return View(vm);
        }

        [HttpPost]
        public IActionResult Add(Guid id)
        {
            // Verify product exists
            var product = _productRepo.GetById(id);
            if (product != null)
            {
                var cart = GetCart();
                cart.AddProduct(id);
                SaveCart(cart);
            }
            return RedirectToAction("Index", "Home");
        }

        public IActionResult Remove(Guid id)
        {
            var cart = GetCart();
            cart.RemoveProduct(id);
            SaveCart(cart);
            return RedirectToAction("Index");
        }

        [HttpPost]
        public IActionResult Checkout()
        {
            var userIdStr = HttpContext.Session.GetString("UserId");
            if (string.IsNullOrEmpty(userIdStr))
            {
                return RedirectToAction("Login", "Account");
            }

            var userId = Guid.Parse(userIdStr);
            var user = _userRepo.GetById(userId);
            var cart = GetCart();

            var products = new List<Product>();
            foreach (var id in cart.ProductIds)
            {
                var p = _productRepo.GetById(id);
                if (p != null) products.Add(p);
            }

            if (user != null && products.Any())
            {
                // Create OrderService with a strategy
                var orderService = new OrderService(new FixedAmountDiscountStrategy(0));
                var order = orderService.PlaceOrder(user, products);

                _orderRepo.Add(order);

                // Clear Cart
                cart.Clear();
                SaveCart(cart);

                return View("OrderConfirmation", order);
            }

            return RedirectToAction("Index");
        }

        private ShoppingCart GetCart()
        {
            return HttpContext.Session.Get<ShoppingCart>("Cart") ?? new ShoppingCart(null);
        }

        private void SaveCart(ShoppingCart cart)
        {
            HttpContext.Session.Set("Cart", cart);
        }
    }
}
