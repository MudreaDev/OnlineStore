namespace OnlineStore.Domain.DesignPatterns.Structural.Bridge
{
    // The Implementor interface defines the implementation-specific operations
    // (In this case, the carrier who actually ships the package)
    public interface IShippingImplementation
    {
        string Ship(string packageId, string address);
    }
}
