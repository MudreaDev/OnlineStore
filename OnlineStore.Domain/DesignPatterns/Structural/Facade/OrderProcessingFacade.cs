using System;
using System.Linq;
using System.Collections.Generic;
using OnlineStore.Domain.Entities;
using OnlineStore.Domain.DesignPatterns.Structural.Adapter;
using OnlineStore.Domain.Singleton;
using OnlineStore.Domain.Interfaces;
using OnlineStore.Domain.Builders;

namespace OnlineStore.Domain.DesignPatterns.Structural.Facade
{
    // Subsistem 1: Verificare și Reducere Stoc (integrat cu IReadableRepository/IWriteableRepository)
    public class InventoryService
    {
        private readonly IReadableRepository<Product> _productReadRepo;
        private readonly IWriteableRepository<Product> _productWriteRepo;

        public InventoryService(IReadableRepository<Product> productReadRepo, IWriteableRepository<Product> productWriteRepo)
        {
            _productReadRepo = productReadRepo;
            _productWriteRepo = productWriteRepo;
        }

        public bool CheckStockAndDeduct(ShoppingCart cart, out string errorMessage)
        {
            errorMessage = string.Empty;

            int totalItems = cart.Items.Sum(i => i.Quantity);
            if (totalItems > ApplicationConfigurationManager.Instance.MaxItemsPerOrder)
            {
                errorMessage = $"Comanda nu poate depăși {ApplicationConfigurationManager.Instance.MaxItemsPerOrder} produse.";
                return false;
            }

            foreach (var item in cart.Items)
            {
                var p = _productReadRepo.GetById(item.ProductId);
                if (p == null) continue;

                if (p.Stock < item.Quantity)
                {
                    errorMessage = $"Stoc insuficient pentru {p.Name}. Disponibil: {p.Stock}";
                    return false;
                }
            }

            // Dacă am ajuns aici, avem stoc disponibil pentru toate produsele.
            // Executăm scăderea.
            foreach (var item in cart.Items)
            {
                var p = _productReadRepo.GetById(item.ProductId);
                if (p != null)
                {
                    p.Stock -= item.Quantity;
                    _productWriteRepo.Update(p);
                }
            }

            return true;
        }
    }

    // Subsistem 2: Procesare Plată (integrat cu Adapter)
    public class PaymentService
    {
        private readonly IExternalPaymentProcessor _paymentProcessor;

        public PaymentService(IExternalPaymentProcessor paymentProcessor)
        {
            _paymentProcessor = paymentProcessor;
        }

        public bool ProcessPayment(string orderId, decimal amount)
        {
            Console.WriteLine($"[Payment] Inițiere plată de {amount} via Adapter.");
            // Utilizăm paternul Adapter!
            return _paymentProcessor.ProcessPayment(amount, "RON", orderId);
        }
    }

    // Subsistem 3: Salvare Comandă
    public class OrderRecordService
    {
        private readonly IWriteableRepository<Order> _orderWriteRepo;
        private readonly IWriteableRepository<User> _userWriteRepo;
        private readonly IReadableRepository<Product> _productReadRepo;

        public OrderRecordService(IWriteableRepository<Order> orderWriteRepo, IWriteableRepository<User> userWriteRepo, IReadableRepository<Product> productReadRepo)
        {
            _orderWriteRepo = orderWriteRepo;
            _userWriteRepo = userWriteRepo;
            _productReadRepo = productReadRepo;
        }

        public Order CreateAndSaveOrder(User user, ShoppingCart cart)
        {
            var orderItems = new List<OrderItem>();
            foreach (var item in cart.Items)
            {
                var p = _productReadRepo.GetById(item.ProductId);
                if (p == null) continue;
                orderItems.Add(new OrderItem(p.Id, p.Name, p.Price, item.Quantity, item.Size, item.Color));
            }

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

            _orderWriteRepo.Add(order);

            if (user is Customer customer)
            {
                if (customer.OrderHistory == null) customer.OrderHistory = new List<Order>();
                customer.OrderHistory.Add(order);
                _userWriteRepo.Update(user);
            }

            return order;
        }
    }

    // Facade: O singură metodă pentru checkout, care coordonează subsistemele
    public class OrderProcessingFacade
    {
        private readonly InventoryService _inventory;
        private readonly PaymentService _payment;
        private readonly OrderRecordService _orderRecord;

        public OrderProcessingFacade(
            IReadableRepository<Product> productReadRepo,
            IWriteableRepository<Product> productWriteRepo,
            IWriteableRepository<Order> orderWriteRepo,
            IWriteableRepository<User> userWriteRepo,
            IExternalPaymentProcessor paymentProcessor)
        {
            _inventory = new InventoryService(productReadRepo, productWriteRepo);
            _payment = new PaymentService(paymentProcessor);
            _orderRecord = new OrderRecordService(orderWriteRepo, userWriteRepo, productReadRepo);
        }

        public bool Checkout(User user, ShoppingCart cart, out string message, out Order placedOrder)
        {
            placedOrder = null!;

            // 1. Verificare și Reducere Stoc (și verificare limită)
            if (!_inventory.CheckStockAndDeduct(cart, out message))
            {
                return false;
            }

            // 2. Creare și Salvare Comandă (inclusiv Builder Pattern din lab anterior)
            placedOrder = _orderRecord.CreateAndSaveOrder(user, cart);

            // 3. Procesare Plată cu Adapter
            if (!_payment.ProcessPayment(placedOrder.Id.ToString(), placedOrder.Total))
            {
                message = "Plata a fost respinsă de procesator.";
                // Observație: Pentru simplificare nu dăm un rollback la stoc/baza de date...
                return false;
            }

            message = "Comanda a fost finalizată și plătită cu succes!";
            return true;
        }
    }
}

