using OnlineStore.Domain.Interfaces;

namespace OnlineStore.Application.Services
{
    public class GlobalPaymentProcessor : IPaymentProcessor
    {
        public void ProcessPayment(decimal amount) => 
            Console.WriteLine($"[Global] Processing international payment of {amount:C} via PayPal/Stripe.");
    }

    public class GlobalShippingProvider : IShippingProvider
    {
        public void ScheduleShipping(string address) => 
            Console.WriteLine($"[Global] Shipping scheduled via DHL/FedEx to: {address}. Expected delivery: 3-5 days.");
    }

    public class GlobalStoreServicesFactory : IStoreServicesFactory
    {
        public IPaymentProcessor CreatePaymentProcessor() => new GlobalPaymentProcessor();
        public IShippingProvider CreateShippingProvider() => new GlobalShippingProvider();
    }
}
