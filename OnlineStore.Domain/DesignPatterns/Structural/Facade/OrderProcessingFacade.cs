using System;

namespace OnlineStore.Domain.DesignPatterns.Structural.Facade
{
    // Subsistem 1: Verificare Stoc
    public class InventoryService
    {
        public bool CheckStock(string productId, int quantity)
        {
            Console.WriteLine($"[Inventory] Verificare stoc pentru produsul {productId}. Cantitate solicitată: {quantity}.");
            return true; // Simplificat
        }

        public void DeductStock(string productId, int quantity)
        {
            Console.WriteLine($"[Inventory] Stoc redus cu {quantity} pentru produsul {productId}.");
        }
    }

    // Subsistem 2: Procesare Plată
    public class PaymentService
    {
        public bool ProcessPayment(string customerId, decimal amount)
        {
            Console.WriteLine($"[Payment] Procesare plată de {amount} lei pentru clientul {customerId}.");
            return true;
        }
    }

    // Subsistem 3: Trimitere Confirmări (Notificări)
    public class NotificationService
    {
        public void SendOrderConfirmation(string customerEmail, string orderId)
        {
            Console.WriteLine($"[Notification] Email de confirmare trimis la {customerEmail} pentru comanda {orderId}.");
        }
    }

    // Facade: Oferă o metodă simplă pentru a plasa o comandă
    public class OrderProcessingFacade
    {
        private readonly InventoryService _inventory;
        private readonly PaymentService _payment;
        private readonly NotificationService _notification;

        public OrderProcessingFacade()
        {
            _inventory = new InventoryService();
            _payment = new PaymentService();
            _notification = new NotificationService();
        }

        public bool PlaceOrder(string productId, int quantity, string customerId, string email, decimal totalAmount)
        {
            Console.WriteLine("--- Începere procesare comandă via Facade ---");

            // 1. Verificare stoc
            if (!_inventory.CheckStock(productId, quantity))
            {
                Console.WriteLine("Comanda eșuată: Stoc insuficient.");
                return false;
            }

            // 2. Procesare plată
            if (!_payment.ProcessPayment(customerId, totalAmount))
            {
                Console.WriteLine("Comanda eșuată: Plata nu a fost acceptată.");
                return false;
            }

            // 3. Reducere stoc
            _inventory.DeductStock(productId, quantity);

            // 4. Trimitere confirmare
            string orderId = Guid.NewGuid().ToString().Substring(0, 8);
            _notification.SendOrderConfirmation(email, orderId);

            Console.WriteLine("--- Comandă plasată cu succes ---");
            return true;
        }
    }
}
