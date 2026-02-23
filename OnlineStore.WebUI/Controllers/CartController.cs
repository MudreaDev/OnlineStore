using Microsoft.AspNetCore.Mvc;
using OnlineStore.Application.Repositories;
using OnlineStore.Application.Services;
using OnlineStore.Domain.Builders;
using OnlineStore.Domain.Entities;
using OnlineStore.Domain.Singleton;
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
            var products = new List<(Product Product, int Quantity)>();
            decimal total = 0;

            foreach (var item in cart.ProductIds)
            {
                var p = _productRepo.GetById(item.Key);
                if (p != null)
                {
                    products.Add((p, item.Value));
                    total += p.Price * item.Value;
                }
            }

            ViewBag.CartItems = products;
            ViewBag.Total = total;

            return View();
        }

        [HttpPost]
        public IActionResult Add(Guid id)
        {
            // Verify product exists and has stock
            var product = _productRepo.GetById(id);
            if (product != null)
            {
                if (product.Stock <= 0)
                {
                    TempData["Error"] = "Produsul nu mai este în stoc.";
                    return RedirectToAction("Index", "Home");
                }

                var cart = GetCart();
                cart.AddProduct(id);
                SaveCart(cart);
                TempData["Success"] = $"Produsul {product.Name} a fost adăugat în coș.";
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
                TempData["Error"] = "Trebuie să fiți autentificat pentru a plasa o comandă.";
                return RedirectToAction("Login", "Account");
            }

            var userId = Guid.Parse(userIdStr);
            var user = _userRepo.GetById(userId);
            var cart = GetCart();

            if (!cart.ProductIds.Any())
            {
                TempData["Error"] = "Coșul este gol.";
                return RedirectToAction("Index");
            }

            // Pattern 3: Singleton - Verificăm limita maximă de produse dintr-o comandă setată global
            int totalItems = cart.ProductIds.Values.Sum();
            if (totalItems > ApplicationConfigurationManager.Instance.MaxItemsPerOrder)
            {
                TempData["Error"] = $"Comanda nu poate depăși {ApplicationConfigurationManager.Instance.MaxItemsPerOrder} produse.";
                return RedirectToAction("Index");
            }

            var productsToOrder = new List<Product>();
            foreach (var item in cart.ProductIds)
            {
                var p = _productRepo.GetById(item.Key);
                if (p == null) continue;

                if (p.Stock < item.Value)
                {
                    TempData["Error"] = $"Stoc insuficient pentru {p.Name}. Disponibil: {p.Stock}";
                    return RedirectToAction("Index");
                }

                for (int i = 0; i < item.Value; i++)
                {
                    productsToOrder.Add(p);
                }
            }

            if (user != null && productsToOrder.Any())
            {
                // Pattern 1: Builder - Folosim Directorul pentru a construi comanda
                var builder = new OrderBuilder();
                var director = new OrderDirector(builder);

                Order order;
                if (productsToOrder.Count > 3)
                {
                    order = director.BuildPremiumOrder(user, productsToOrder);
                }
                else
                {
                    order = director.BuildStandardOrder(user, productsToOrder);
                }

                _orderRepo.Add(order);

                // Decrement stock
                foreach (var item in cart.ProductIds)
                {
                    var p = _productRepo.GetById(item.Key);
                    if (p != null)
                    {
                        p.Stock -= item.Value;
                        _productRepo.Update(p);
                    }
                }

                if (user is Customer customer)
                {
                    if (customer.OrderHistory == null) customer.OrderHistory = new List<Order>();
                    customer.OrderHistory.Add(order);
                    _userRepo.Update(user);
                }

                // Clear Cart
                cart.Clear();
                SaveCart(cart);

                TempData["Success"] = "Comanda a fost plasată cu succes!";
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
