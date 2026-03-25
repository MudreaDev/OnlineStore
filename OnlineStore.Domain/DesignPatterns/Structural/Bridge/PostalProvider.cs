namespace OnlineStore.Domain.DesignPatterns.Structural.Bridge
{
    // Concrete Implementor: Postal Service (e.g., Poșta Română)
    public class PostalProvider : IShippingImplementation
    {
        public string Ship(string packageId, string address)
        {
            return $"[Poștă] Livrare standard pentru pachetul {packageId} la adresa: {address}";
        }
    }
}
