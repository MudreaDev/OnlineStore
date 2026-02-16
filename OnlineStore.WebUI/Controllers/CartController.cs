using Microsoft.AspNetCore.Mvc;
using OnlineStore.Application.Repositories;
using OnlineStore.Application.Services;
using OnlineStore.Domain.Entities;
using OnlineStore.Domain.Strategies;
using OnlineStore.WebUI.Extensions;

namespace OnlineStore.WebUI.Controllers
{
    public class CartController : Controller
    {
        private readonly InMemoryProductRepository _productRepo;
        private readonly InMemoryOrderRepository _orderRepo;
        private readonly InMemoryUserRepository _userRepo;

        public CartController(InMemoryProductRepository productRepo, InMemoryOrderRepository orderRepo, InMemoryUserRepository userRepo)
        {
            _productRepo = productRepo;
            _orderRepo = orderRepo;
            _userRepo = userRepo;
        }

        public IActionResult Index()
        {
            var cart = HttpContext.Session.Get<ShoppingCart>("Cart") ?? new ShoppingCart(null);
            return View(cart);
        }

        [HttpPost]
        public IActionResult Add(Guid id)
        {
            var product = _productRepo.GetById(id);
            if (product != null)
            {
                var cart = HttpContext.Session.Get<ShoppingCart>("Cart") ?? new ShoppingCart(null); // User is optional in cart structure for now
                cart.AddProduct(product);
                HttpContext.Session.Set("Cart", cart);
            }
            return RedirectToAction("Index", "Home");
        }

        public IActionResult Remove(Guid id)
        {
            var cart = HttpContext.Session.Get<ShoppingCart>("Cart");
            if (cart != null)
            {
                cart.RemoveProduct(id);
                HttpContext.Session.Set("Cart", cart);
            }
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
            var cart = HttpContext.Session.Get<ShoppingCart>("Cart");

            if (user != null && cart != null && cart.Products.Any())
            {
                // Create OrderService with a strategy
                var orderService = new OrderService(new FixedAmountDiscountStrategy(0)); // No discount for web demo currently
                var order = orderService.PlaceOrder(user, cart.Products);

                _orderRepo.Add(order);

                // Clear Cart
                HttpContext.Session.Remove("Cart");

                return View("OrderConfirmation", order);
            }

            return RedirectToAction("Index");
        }
    }
}
