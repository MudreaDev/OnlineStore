namespace OnlineStore.Domain.Interfaces
{
    public interface IStoreServicesFactory
    {
        IPaymentProcessor CreatePaymentProcessor();
        IShippingProvider CreateShippingProvider();
    }
}
