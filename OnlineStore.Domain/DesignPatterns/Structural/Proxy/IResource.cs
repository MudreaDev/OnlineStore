namespace OnlineStore.Domain.DesignPatterns.Structural.Proxy
{
    // The Subject interface declares common operations for both RealSubject and Proxy
    public interface IResource
    {
        string Access(string userRole);
    }
}
