using OnlineStore.Domain.Interfaces;

namespace OnlineStore.Application.Services
{
    public class LocalPaymentProcessor : IPaymentProcessor
    {
        public void ProcessPayment(decimal amount) => 
            Console.WriteLine($"[Local] Processing cash or card payment of {amount:C} at the physical counter.");
    }

    public class LocalShippingProvider : IShippingProvider
    {
        public void ScheduleShipping(string address) => 
            Console.WriteLine($"[Local] Shipping scheduled via local courier to: {address}. Expected delivery: 24h.");
    }

    public class LocalStoreServicesFactory : IStoreServicesFactory
    {
        public IPaymentProcessor CreatePaymentProcessor() => new LocalPaymentProcessor();
        public IShippingProvider CreateShippingProvider() => new LocalShippingProvider();
    }
}
