namespace OnlineStore.Domain.DesignPatterns.Structural.Bridge
{
    // Refined Abstraction: Pickup Point Delivery
    public class PickupPointDelivery : ShippingMethod
    {
        public PickupPointDelivery(IShippingImplementation implementation) : base(implementation) { }

        public override string Deliver(string packageId, string destination)
        {
            return _implementation.Ship(packageId, "Punct de ridicare: " + destination);
        }
    }
}
