using Microsoft.AspNetCore.Mvc;
using OnlineStore.Application.Repositories;
using OnlineStore.Application.Services;
using OnlineStore.Domain.Builders;
using OnlineStore.Domain.Entities;
using OnlineStore.Domain.Singleton;
using OnlineStore.Domain.Strategies;
using OnlineStore.WebUI.Extensions;
using OnlineStore.WebUI.Models;
using OnlineStore.Domain.DesignPatterns.Structural.Facade;
using OnlineStore.Domain.DesignPatterns.Structural.Adapter;
using OnlineStore.Domain.DesignPatterns.Structural.Bridge;
using OnlineStore.Domain.DesignPatterns.Behavioral.Command;
using OnlineStore.Domain.DesignPatterns.Behavioral.Memento;
using OnlineStore.Domain.Interfaces;

namespace OnlineStore.WebUI.Controllers
{
    public class CartController : Controller
    {
        private readonly DbProductRepository _productRepo;
        private readonly DbOrderRepository _orderRepo;
        private readonly DbUserRepository _userRepo;
        private readonly IEmailService _emailService;

        public CartController(DbProductRepository productRepo, DbOrderRepository orderRepo, DbUserRepository userRepo, IEmailService emailService)
        {
            _productRepo = productRepo;
            _orderRepo = orderRepo;
            _userRepo = userRepo;
            _emailService = emailService;
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
                var caretaker = GetCaretaker();
                var invoker = new CartActionInvoker(caretaker);
                
                var command = new AddProductCommand(cart, id, size, color);
                invoker.ExecuteCommand(command, cart);
                
                SaveCart(cart);
                SaveCaretaker(caretaker);
                
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
                var caretaker = GetCaretaker();
                var invoker = new CartActionInvoker(caretaker);
                
                var command = new UpdateQuantityCommand(cart, id, quantity, size, color);
                invoker.ExecuteCommand(command, cart);
                
                SaveCart(cart);
                SaveCaretaker(caretaker);
            }
            return RedirectToAction("Index");
        }

        [HttpPost]
        public IActionResult Remove(Guid id, string? size, string? color)
        {
            var cart = GetCart();
            var caretaker = GetCaretaker();
            var invoker = new CartActionInvoker(caretaker);
            
            var command = new RemoveProductCommand(cart, id, size, color);
            invoker.ExecuteCommand(command, cart);
            
            SaveCart(cart);
            SaveCaretaker(caretaker);
            
            return RedirectToAction("Index");
        }

        [HttpPost]
        public IActionResult Undo()
        {
            var cart = GetCart();
            var caretaker = GetCaretaker();
            var invoker = new CartActionInvoker(caretaker);

            if (caretaker.UndoStack.Count > 0)
            {
                invoker.Undo(cart);
                SaveCart(cart);
                SaveCaretaker(caretaker);
                TempData["Success"] = "Acțiunea a fost anulată.";
            }
            else
            {
                TempData["Error"] = "Nu mai există acțiuni de anulat.";
            }

            return RedirectToAction("Index");
        }

        [HttpPost]
        public IActionResult Redo()
        {
            var cart = GetCart();
            var caretaker = GetCaretaker();
            var invoker = new CartActionInvoker(caretaker);

            if (caretaker.RedoStack.Count > 0)
            {
                invoker.Redo(cart);
                SaveCart(cart);
                SaveCaretaker(caretaker);
                TempData["Success"] = "Acțiunea a fost refăcută.";
            }
            else
            {
                TempData["Error"] = "Nu mai există acțiuni de refăcut.";
            }

            return RedirectToAction("Index");
        }

        [HttpPost]
        public IActionResult Checkout(string paymentMethod, string storeType, string deliveryType, string shippingProvider)
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

            if (user == null)
            {
                TempData["Error"] = "Utilizator invalid.";
                return RedirectToAction("Index");
            }

            // Setup the Payment Adapter based on user selection
            IExternalPaymentProcessor paymentProcessor;
            if (paymentMethod == "Stripe")
            {
                paymentProcessor = new StripeAdapter(new StripeApi());
            }
            else
            {
                // Default to PayPal
                paymentProcessor = new PayPalAdapter(new PayPalApi());
            }

            // Create Facade using the requested Repositories and Payment Processor
            var orderFacade = new OrderProcessingFacade(_productRepo, _productRepo, _orderRepo, _userRepo, paymentProcessor, _emailService);

            // Execute Checkout via Facade
            if (orderFacade.Checkout(user, cart, out string message, out Order placedOrder))
            {
                // Abstract Factory Pattern — alege familia de servicii (Local vs Global)
                IStoreServicesFactory storeFactory = storeType == "Local"
                    ? new LocalStoreServicesFactory()
                    : new GlobalStoreServicesFactory();

                var factoryPayment = storeFactory.CreatePaymentProcessor();
                var factoryShipping = storeFactory.CreateShippingProvider();

                var shippingAddress = (user as Customer)?.ShippingAddress ?? "N/A";
                factoryPayment.ProcessPayment(placedOrder.Total);
                factoryShipping.ScheduleShipping(shippingAddress);

                TempData["StoreType"] = storeType == "Local"
                    ? "Local Store (cash / curier local)"
                    : "Global Store (PayPal / DHL)";

                // Bridge Pattern — separă tipul de livrare de furnizor
                IShippingImplementation bridgeImpl = shippingProvider == "Courier"
                    ? new CourierProvider()
                    : new PostalProvider();

                ShippingMethod shippingMethod = deliveryType == "Home"
                    ? new HomeDelivery(bridgeImpl)
                    : new PickupPointDelivery(bridgeImpl);

                var deliveryResult = shippingMethod.Deliver(
                    placedOrder.Id.ToString()[..8], shippingAddress);

                TempData["DeliveryInfo"] = deliveryResult;

                // Clear Cart
                cart.Clear();
                SaveCart(cart);

                TempData["Success"] = message;
                return View("OrderConfirmation", placedOrder);
            }
            else
            {
                TempData["Error"] = message;
                return RedirectToAction("Index");
            }
        }

        private ShoppingCart GetCart()
        {
            return HttpContext.Session.Get<ShoppingCart>("Cart") ?? new ShoppingCart(null);
        }

        private void SaveCart(ShoppingCart cart)
        {
            HttpContext.Session.Set("Cart", cart);
        }

        private CartCaretaker GetCaretaker()
        {
            return HttpContext.Session.Get<CartCaretaker>("CartCaretaker") ?? new CartCaretaker();
        }

        private void SaveCaretaker(CartCaretaker caretaker)
        {
            HttpContext.Session.Set("CartCaretaker", caretaker);
        }
    }
}
