namespace OnlineStore.Domain.DesignPatterns.Structural.Bridge
{
    // Concrete Implementor: Courier Service (e.g., Fan Courier, DHL)
    public class CourierProvider : IShippingImplementation
    {
        public string Ship(string packageId, string address)
        {
            return $"[Curier] Livrare rapidă pentru pachetul {packageId} la adresa: {address}";
        }
    }
}
