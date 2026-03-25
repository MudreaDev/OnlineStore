namespace OnlineStore.Domain.DesignPatterns.Structural.Bridge
{
    // Refined Abstraction: Home Delivery
    public class HomeDelivery : ShippingMethod
    {
        public HomeDelivery(IShippingImplementation implementation) : base(implementation) { }

        public override string Deliver(string packageId, string destination)
        {
            return _implementation.Ship(packageId, "Adresă de domiciliu: " + destination);
        }
    }
}
