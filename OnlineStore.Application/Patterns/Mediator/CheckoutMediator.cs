using OnlineStore.Application.Services;
using OnlineStore.Application.Repositories;
using OnlineStore.Domain.Entities;
using OnlineStore.Domain.Interfaces;
using OnlineStore.Domain.DesignPatterns.Structural.Adapter;
using OnlineStore.Domain.DesignPatterns.Structural.Bridge;
using OnlineStore.Domain.Factories;
using System;
using System.Collections.Generic;
using System.Linq;

namespace OnlineStore.Application.Patterns.Mediator
{
    /// <summary>
    /// Pattern Mediator — acționează ca un punct central de control pentru procesul de checkout.
    /// Rol: Reduce cuplarea între serviciile de comenzi, stoc, plăți și notificări.
    /// În loc ca serviciile să comunice direct între ele, acestea interacționează prin mediator.
    /// </summary>
    public class CheckoutMediator : ICheckoutMediator
    {
        private readonly OrderService _orderService;
        private readonly OrderValidationService _validationService;
        private readonly IEmailService _emailService;
        private readonly IStockService _stockService;
        private readonly IPaymentService _paymentService;
        private readonly IReadableRepository<Product> _productRepository;
        private readonly IWriteableRepository<Order> _orderRepository;

        public CheckoutMediator(
            OrderService orderService, 
            OrderValidationService validationService,
            IEmailService emailService,
            IStockService stockService,
            IPaymentService paymentService,
            IReadableRepository<Product> productRepository,
            IWriteableRepository<Order> orderRepository)
        {
            _orderService = orderService;
            _validationService = validationService;
            _emailService = emailService;
            _stockService = stockService;
            _paymentService = paymentService;
            _productRepository = productRepository;
            _orderRepository = orderRepository;
        }

        public async Task<(bool Success, string Message, Order? Order)> CheckoutAsync(
            User user, 
            ShoppingCart cart, 
            string paymentMethod, 
            string storeType, 
            string deliveryType, 
            string shippingProvider, 
            string shippingAddress, 
            string phoneNumber)
        {
            try
            {

                // 1. Prepare Order Items
                var orderItems = new List<OrderItem>();
                foreach (var cartItem in cart.Items)
                {
                    var product = _productRepository.GetById(cartItem.ProductId);
                    if (product != null)
                    {
                        orderItems.Add(new OrderItem(product.Id, product.Name, product.Price, cartItem.Quantity));
                    }
                }

                // 2. Perform Validation using Chain of Responsibility (via Service)
                var validationError = _validationService.ValidateOrder(user, orderItems);
                if (validationError != null)
                {
                    return (false, validationError, null);
                }

                // 3. Setup Payment Adapter (Adapter Pattern)
                IExternalPaymentProcessor paymentProcessor = paymentMethod == "Stripe"
                    ? new StripeAdapter(new StripeApi())
                    : new PayPalAdapter(new PayPalApi());

                // 4. Place Order via OrderService
                var order = _orderService.PlaceOrder(user, orderItems, shippingAddress, phoneNumber);
                
                // Salvează comanda în baza de date pentru persistență!
                _orderRepository.Add(order);

                // 5. Process Payment
                _paymentService.Process(order.Total);

                // 6. Abstract Factory Pattern — alege familia de servicii (Local vs Global)
                IStoreServicesFactory storeFactory = storeType == "Local"
                    ? new LocalStoreServicesFactory()
                    : new GlobalStoreServicesFactory();

                var factoryPayment = storeFactory.CreatePaymentProcessor();
                var factoryShipping = storeFactory.CreateShippingProvider();

                factoryPayment.ProcessPayment(order.Total);
                factoryShipping.ScheduleShipping(shippingAddress);

                // 7. Bridge Pattern — separă tipul de livrare de furnizor
                IShippingImplementation bridgeImpl = shippingProvider == "Courier"
                    ? new CourierProvider()
                    : new PostalProvider();

                ShippingMethod shippingMethodImpl = deliveryType == "Home"
                    ? new HomeDelivery(bridgeImpl)
                    : new PickupPointDelivery(bridgeImpl);

                var deliveryResult = shippingMethodImpl.Deliver(order.Id.ToString()[..8], shippingAddress);

                // 8. Update Stock
                foreach (var item in cart.Items)
                {
                    _stockService.UpdateStock(item.ProductId, -item.Quantity);
                }

                // 9. Notify via Email Service
                Notify(this, "OrderPlaced");
                await _emailService.SendOrderConfirmationAsync(user.Email, order.Id.ToString(), order.Total);

                // 10. Clear Cart
                cart.Clear();

                return (true, $"Comanda {order.Id} a fost plasată cu succes! {deliveryResult}", order);
            }
            catch (Exception ex)
            {
                return (false, $"Eroare la procesarea comenzii: {ex.Message}", null);
            }
        }

        public void Notify(object sender, string eventCode)
        {
            if (eventCode == "OrderPlaced")
            {
                // Notificare eveniment
            }
        }
    }
}
