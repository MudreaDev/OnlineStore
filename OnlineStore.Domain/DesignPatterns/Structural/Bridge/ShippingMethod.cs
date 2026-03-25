namespace OnlineStore.Domain.DesignPatterns.Structural.Bridge
{
    // The Abstraction defines the high-level delivery type chosen by the customer
    public abstract class ShippingMethod
    {
        protected readonly IShippingImplementation _implementation;

        protected ShippingMethod(IShippingImplementation implementation)
        {
            _implementation = implementation;
        }

        public abstract string Deliver(string packageId, string destination);
    }
}
