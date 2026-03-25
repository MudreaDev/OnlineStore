namespace OnlineStore.Domain.DesignPatterns.Structural.Proxy
{
    // The RealSubject contains the core business logic
    public class RealResource : IResource
    {
        public string Access(string userRole)
        {
            return "Accesat resursă confidențială.";
        }
    }
}
