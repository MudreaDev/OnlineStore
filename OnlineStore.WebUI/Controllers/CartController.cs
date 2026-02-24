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
            var products = new List<(Product Product, int Quantity, string? Size, string? Color)>();
            decimal total = 0;

            foreach (var item in cart.Items)
            {
                var p = _productRepo.GetById(item.ProductId);
                if (p != null)
                {
                    products.Add((p, item.Quantity, item.Size, item.Color));
                    total += p.Price * item.Quantity;
                }
            }

            ViewBag.CartItems = products;
            ViewBag.Total = total;

            return View();
        }

        [HttpPost]
        public IActionResult Add(Guid id, string? size, string? color)
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
                cart.AddProduct(id, size, color);
                SaveCart(cart);
                TempData["Success"] = $"Produsul {product.Name} a fost adăugat în coș.";
            }
            return RedirectToAction("Index", "Home");
        }

        [HttpPost]
        public IActionResult UpdateQuantity(Guid id, int quantity, string? size, string? color)
        {
            var cart = GetCart();
            var item = cart.Items.FirstOrDefault(i => i.ProductId == id && i.Size == size && i.Color == color);
            if (item != null)
            {
                if (quantity > 0)
                {
                    item.Quantity = quantity;
                }
                else
                {
                    cart.Items.Remove(item);
                }
                SaveCart(cart);
            }
            return RedirectToAction("Index");
        }

        [HttpPost]
        public IActionResult Remove(Guid id, string? size, string? color)
        {
            var cart = GetCart();
            cart.RemoveProduct(id, size, color);
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

            if (!cart.Items.Any())
            {
                TempData["Error"] = "Coșul este gol.";
                return RedirectToAction("Index");
            }

            // Pattern 3: Singleton - Verificăm limita maximă de produse dintr-o comandă setată global
            int totalItems = cart.Items.Sum(i => i.Quantity);
            if (totalItems > ApplicationConfigurationManager.Instance.MaxItemsPerOrder)
            {
                TempData["Error"] = $"Comanda nu poate depăși {ApplicationConfigurationManager.Instance.MaxItemsPerOrder} produse.";
                return RedirectToAction("Index");
            }

            var orderItems = new List<OrderItem>();
            foreach (var item in cart.Items)
            {
                var p = _productRepo.GetById(item.ProductId);
                if (p == null) continue;

                if (p.Stock < item.Quantity)
                {
                    TempData["Error"] = $"Stoc insuficient pentru {p.Name}. Disponibil: {p.Stock}";
                    return RedirectToAction("Index");
                }

                orderItems.Add(new OrderItem(p.Id, p.Name, p.Price, item.Quantity, item.Size, item.Color));
            }

            if (user != null && orderItems.Any())
            {
                // Pattern 1: Builder - Folosim Directorul pentru a construi comanda
                var builder = new OrderBuilder();
                var director = new OrderDirector(builder);

                Order order;
                if (orderItems.Sum(i => i.Quantity) > 3)
                {
                    order = director.BuildPremiumOrder(user, orderItems);
                }
                else
                {
                    order = director.BuildStandardOrder(user, orderItems);
                }

                _orderRepo.Add(order);

                // Decrement stock
                foreach (var item in cart.Items)
                {
                    var p = _productRepo.GetById(item.ProductId);
                    if (p != null)
                    {
                        p.Stock -= item.Quantity;
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
